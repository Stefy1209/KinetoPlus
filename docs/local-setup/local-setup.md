# Local setup

How to run KinetoPlus on your machine for development and testing.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) with Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## 1. Start the database

The local database is a SQL Server 2025 container defined in `docker-compose.yml` at the repository root.

```bash
cp .env.example .env    # then set MSSQL_SA_PASSWORD in .env
docker compose up -d
docker compose logs -f db
```

Wait until the logs show `SQL Server is now ready for client connections`, then press `Ctrl+C`.

The password must have at least 8 characters and contain upper case, lower case, digits and symbols, otherwise the container stops right after starting. Avoid `$`, `;` and `#`.

## 2. Set the connection string

The connection string is stored with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), so it never ends up in git. Use the same password as in `.env`:

```bash
dotnet user-secrets set "ConnectionStrings:KinetoPlus" \
  "Server=localhost,1433;Database=KinetoPlus;User Id=sa;Password=<your-password>;TrustServerCertificate=True" \
  --project src/backend/WebAPI
```

`TrustServerCertificate=True` is needed because the container uses a self-signed certificate.

## 3. Create the database schema

```bash
dotnet tool restore
dotnet ef database update --project src/backend/WebAPI
```

## 4. Run the API

```bash
dotnet run --project src/backend/WebAPI
```

Example requests are in [`WebAPI.http`](../../src/backend/WebAPI/WebAPI.http).

## Stop the database

```bash
docker compose down       # stops the container, keeps the data
docker compose down -v    # stops the container and deletes the data
```
