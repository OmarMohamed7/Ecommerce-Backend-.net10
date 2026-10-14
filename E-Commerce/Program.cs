using System.Text;
using E_Commerce.Cache;
using E_Commerce.Common;
using E_Commerce.Db;
using E_Commerce.Features.Audit;
using E_Commerce.Features.Auth;
using E_Commerce.Features.Cart;
using E_Commerce.Features.Order;
using E_Commerce.Features.Products;
using E_Commerce.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

// Load .env first so its values are part of the configuration
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Fail fast with a clear message instead of running with a missing or placeholder secret
foreach (var required in new[]
         {
             "ConnectionStrings:DefaultConnection", "ConnectionStrings:Redis", "Jwt:Key", "Admin:Password"
         })
{
    var value = builder.Configuration[required];

    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException(
            $"Configuration '{required}' is missing. Copy .env.example to .env and fill it in.");

    if (value.Contains("CHANGE_ME"))
        throw new InvalidOperationException(
            $"Configuration '{required}' still contains the CHANGE_ME placeholder. Edit your .env.");
}

if (builder.Configuration["Jwt:Key"]!.Length < 32)
    throw new InvalidOperationException("Jwt__Key must be at least 32 characters.");

// Logger
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .WriteTo.File(
            "logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            shared: true);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// FluentValidation owns validation; stop MVC from adding implicit [Required] to non-nullable strings
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ECommerce";
});

// Register a db context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Cache Service
builder.Services.AddSingleton<CacheService>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
});

// This picks up every validator
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

// Add a scoped service for products (must be scoped because it depends on the scoped AppDbContext)
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new ApiJwtBearerEvents();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    };
});

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(ProductProfile));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT only; Swagger adds the \"Bearer \" prefix."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

await DbSeeder.SeedAdminASync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();