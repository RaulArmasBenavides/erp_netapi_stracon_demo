# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `SupplierServiceNet/` (the directory containing the `.sln` file).

```bash
# Build
dotnet build

# Run API
dotnet run --project SupplierServiceNet/SupplierServiceNet.csproj

# Run with Docker (API on :8000, SQL Server on :14333)
docker compose up -d --build

# EF Core migrations (targets Infrastructure project)
dotnet ef migrations add <MigrationName> -p SupplierServiceNet.Infraestructure
dotnet ef database update -p SupplierServiceNet.Infraestructure
```

Local API: http://localhost:5125 | Swagger: http://localhost:5125/swagger

## Architecture

Clean Architecture with 4 layers:

```
SupplierServiceNet          ← API: Controllers, Middlewares, Extensions, Seed
SupplierServiceNet.Application  ← Business logic: Services, DTOs
SupplierServiceNet.Core         ← Domain: Entities, Interfaces (IRepository, IService)
SupplierServiceNet.Infraestructure  ← Data: EF Core DbContext, Repositories, UnitOfWork
```

**Note:** The infrastructure project folder name is `SupplierServiceNet.Infraestructure` (Spanish spelling — one 's').

### Key patterns

- **Unit of Work + Generic Repository** — `IUnitOfWork` wraps `ISupplierRepository`, `IUserRepository`, etc. All data access goes through `unitOfWork.Repository<T>` or typed repositories.
- **Domain entities with behavior** — `Supplier.Approve()`, `Supplier.CreatePurchaseRequest()` contain business logic, not just properties.
- **API Versioning** — URL segment strategy; all routes are `/api/v1/...`.
- **AutoMapper** — DTOs are mapped in Application layer; mapping profiles live in `SupplierServiceNet.Application`.
- **Dependency Injection wiring** — `ApplicationServicesExtensions` (API layer) + `DependencyInjection.cs` (Infrastructure layer) register all services. Add new services there.

### Data flow

```
Controller → IService (Application) → IUnitOfWork → Repository<T> → ApplicationDbContext (EF Core) → SQL Server
```

### Authentication

JWT Bearer. Token secret lives in `appsettings.json` under `ApiSettings:Secreta`. Roles: `Requester`, `Approver`. Seeded via `SeedData.InitializeAsync()` at startup.

### Database

- **ORM:** Entity Framework Core 7
- **Primary DB:** SQL Server (Docker: `Server=localhost,14333`, DB: `stracon_db`, user `sa`)
- `ApplicationDbContext` extends `IdentityDbContext<User>` — ASP.NET Identity tables coexist with domain tables.

### External integrations

- **Cloudinary** — photo storage for suppliers; configured via `CloudinaryOptions` in `appsettings.json`.

## Infrastructure folder spelling

The infrastructure project uses the Spanish spelling `Infraestructure` (not `Infrastructure`). Ensure all `using` directives, project references, and file paths use this spelling.
