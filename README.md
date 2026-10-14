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

### 1. Create your `.env`

All settings and secrets live in one git-ignored file, `E-Commerce/.env`. The app reads it at startup and docker compose
reads it too. Start from the committed template:

```bash
cd E-Commerce
cp .env.example .env
```

Open `.env` and replace every `CHANGE_ME`:

| Key                                   | What to put                                                                        |
|---------------------------------------|------------------------------------------------------------------------------------|
| `POSTGRES_PASSWORD`, `REDIS_PASSWORD` | Any passwords (letters and numbers only). The connection strings below reuse them. |
| `Jwt__Key`                            | A random string of at least 32 characters: `openssl rand -base64 32`               |
| `Admin__Email`, `Admin__Password`     | The first admin account, created at the first startup                              |

A double underscore is a colon in the app's configuration (`Jwt__Key` is `Jwt:Key`). A real environment variable with
the same name always wins over `.env`, which is how you configure a server. The app refuses to start, and says which
value, if a required one is missing or still contains `CHANGE_ME`. Never commit `.env`.

### 2. Start PostgreSQL and Redis

```bash
docker compose up -d --wait
```

PostgreSQL is on `localhost:5433` (database `Ecommerce`, user `postgres`) and Redis on `localhost:6379`. Both need their
password from `.env`.

### 3. Apply migrations and run

```bash
dotnet ef database update
dotnet run --launch-profile https
```

- API: `https://localhost:7180`
- Swagger UI: `https://localhost:7180/swagger`

Use the `https` address. Plain `http://localhost:5133` redirects to https, and browsers block the redirected POST from Swagger.

After changing a model, create a migration with `dotnet ef migrations add <Name>` and apply it with
`dotnet ef database update`. Log in with `Admin__Email` and `Admin__Password` through `POST /api/auth/login`.

Other JWT settings live in `appsettings.json`: `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresInMinutes`.

### Optional: seed sample products

Inserts a random 50 to 100 products (it asks for the Postgres password):

```bash
psql -h localhost -p 5433 -U postgres -d Ecommerce -f Scripts/seed-products.sql
```

### Troubleshooting

| Problem                                                                        | Fix                                                                                                                                        |
|--------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| `Configuration '...' is missing` or `still contains the CHANGE_ME placeholder` | Fix that value in `.env`                                                                                                                   |
| `NOAUTH Authentication required`                                               | The password in `ConnectionStrings__Redis` does not match `REDIS_PASSWORD`. Fix `.env`, then `docker compose up -d --force-recreate redis` |
| `password authentication failed`                                               | The Postgres volume keeps the first password. Change it in Postgres, or `docker compose down -v` (deletes the data)                        |
| A changed value has no effect                                                  | Restart the app (`.env` is read once). Also check that no shell or IDE environment variable with the same name overrides it                |
| 401 on every request                                                           | Token missing or expired (60 minutes). Changing `Jwt__Key` invalidates all tokens                                                          |
| Port 5433, 6379 or 7180 in use                                                 | Stop the other program using it                                                                                                            |

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
