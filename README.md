# E-Commerce API

An ASP.NET Core (.NET 10) Web API for managing products, with JWT authentication, PostgreSQL storage through EF Core, and Swagger documentation.

## Tech stack

- ASP.NET Core controllers, .NET 10
- Entity Framework Core 10 with PostgreSQL (Npgsql)
- FluentValidation for request validation
- JWT bearer authentication, BCrypt password hashing
- Swagger UI (Swashbuckle)
- Docker Compose for the local database

## Project structure

```
E-Commerce/
  Controllers/   HTTP endpoints (Products, Auth)
  Services/      Business logic (ProductService, AuthService)
  Models/        EF Core entities (Product, User)
  DTOs/          Request/response types, ApiResponse<T>, PagedResult<T>
  Validators/    FluentValidation validators, one per request DTO
  Db/            AppDbContext
  Migrations/    EF Core migrations
  Middleware/    Global exception handler
  Extensions/    Helpers (e.g. ValidationResult -> ApiError[])
  Scripts/       seed-products.sql
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker (for PostgreSQL)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### 1. Start the database

Create a `.env` file next to `E-Commerce/compose.yaml` (it is git-ignored):

```
POSTGRES_PASSWORD=choose-a-password
```

Then, from the `E-Commerce/` project folder:

```bash
docker compose up -d
```

PostgreSQL is exposed on `localhost:5433`, database `Ecommerce`, user `postgres`.

### 2. Configure secrets

Secrets are not stored in `appsettings*.json` (they contain placeholders). Set them with user-secrets from the `E-Commerce/` project folder:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=Ecommerce;Username=postgres;Password=<your POSTGRES_PASSWORD>"
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
```

User-secrets are only loaded when `ASPNETCORE_ENVIRONMENT=Development`, which the launch profiles already set. For production, provide the same keys as environment variables (`ConnectionStrings__DefaultConnection`, `Jwt__Key`).

Other JWT settings live in `appsettings.json`: `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresInMinutes`.

### 3. Apply migrations

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update
```

Run `ef` commands with `ASPNETCORE_ENVIRONMENT=Development`, otherwise the user-secrets are not loaded. After changing a model, create a migration with `dotnet ef migrations add <Name>` and apply it with `dotnet ef database update`.

### 4. (Optional) Seed sample products

Inserts a random 50 to 100 products:

```bash
psql -h localhost -p 5433 -U postgres -d Ecommerce -f Scripts/seed-products.sql
```

### 5. Run

```bash
dotnet run --launch-profile https
```

- API: `https://localhost:7180`
- Swagger UI: `https://localhost:7180/swagger`

Use the `https` address. Plain `http://localhost:5133` redirects to https, and browsers block the redirected POST from Swagger.

## Authentication

1. `POST /api/auth/register` or `POST /api/auth/login` returns an `accessToken`.
2. In Swagger, click **Authorize** and paste the token (without the `Bearer ` prefix).
3. Outside Swagger, send it as `Authorization: Bearer <token>`.

All `/api/products` endpoints require a valid token.

## Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | No | Create a user, returns a token |
| POST | `/api/auth/login` | No | Log in, returns a token |
| GET | `/api/products` | Yes | Paged, filtered, sorted list |
| GET | `/api/products/{id}` | Yes | Get one product |
| POST | `/api/products` | Yes | Create a product |
| PATCH | `/api/products/{id}` | Yes | Partially update a product |
| DELETE | `/api/products/{id}` | Yes | Delete a product |

### Listing query parameters

`GET /api/products` accepts:

| Parameter | Description |
|---|---|
| `page` | Page number, minimum 1 (default 1) |
| `pageSize` | Items per page, 1 to 100 (default 20) |
| `search` | Case-insensitive match on the product name |
| `minPrice`, `maxPrice` | Price range |
| `minQuantity`, `maxQuantity` | Stock range |
| `sortBy` | `price`, `stock` or `name` (default: by id) |
| `descending` | `true` for descending order |

## Response format

Every endpoint returns the same envelope. Only one of `data` or `errors` is present:

```json
{ "success": true, "data": { } }
```

```json
{ "success": false, "errors": [ { "field": "Name", "message": "Product name is required." } ] }
```

The list endpoint puts a paged result in `data`:

```json
{
  "success": true,
  "data": { "items": [], "totalCount": 120, "page": 1, "pageSize": 20, "totalPages": 6 }
}
```

Unhandled exceptions are caught by `ExceptionHandlerMiddleware` and returned in the same envelope.

## Notes

- Validation is done by FluentValidation. MVC's implicit `[Required]` on non-nullable strings is disabled so the validators produce the messages.
- Services are registered as scoped because they depend on the scoped `AppDbContext`.
- `appsettings.json` must never contain real secrets.
