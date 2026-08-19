# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

**SupplierServiceNet** is a .NET 8 ASP.NET Core REST API project for managing suppliers and purchase requests. It follows a clean/layered architecture and targets both SQL Server and PostgreSQL databases.

## Build & Run

### Prerequisites
- .NET 8 SDK
- SQL Server (or PostgreSQL) running locally at the configured connection string
- Docker (for containerized setup)

### Common Commands

**Build the solution:**
```powershell
dotnet build
```

**Run the API locally (development):**
```powershell
cd SupplierServiceNet
dotnet run
```
The API will start at `https://localhost:7008` (or check `launchSettings.json` for the actual port).

**Run unit/integration tests** (when test projects are added):
```powershell
dotnet test
```

**Apply Entity Framework migrations:**
```powershell
cd SupplierServiceNet.Infraestructure
dotnet ef migrations add MigrationName --startup-project ../SupplierServiceNet
dotnet ef database update --startup-project ../SupplierServiceNet
```

**Build Docker container:**
```powershell
docker-compose build
docker-compose up
```

## Architecture

The project follows a **clean/layered architecture** with five projects:

### 1. **SupplierServiceNet** (Presentation/API Layer)
- Location: `SupplierServiceNet/`
- Responsibilities:
  - HTTP endpoints and controllers (`Controllers/`)
  - Request/response handling
  - Middleware setup (`Middlewares/`)
  - Dependency injection configuration (`Extensions/`)
  - `Program.cs` orchestrates startup with service registration
- API versioning via URL segment (e.g., `/api/v1/...`)
- Controllers: `LoginController`, `UsersController`, `SuppliersController`

### 2. **SupplierServiceNet.Application** (Application/Business Logic)
- Location: `SupplierServiceNet.Application/`
- Responsibilities:
  - Application services (`Services/`)
  - Data Transfer Objects - DTOs (`Dtos/`)
  - Interfaces for services (`Interfaces/`)
  - Contains business logic orchestration
- **Key Services**: `UserService`, `SupplierService`, `CloudinaryService`
- No direct database access; all data operations flow through repositories

### 3. **SupplierServiceNet.Core** (Domain/Entities)
- Location: `SupplierServiceNet.Core/`
- Responsibilities:
  - Domain entities (`Entities/`)
  - Repository interfaces (`IRepositorio/`)
  - Cross-cutting concerns (`CrossCutting/`) like `ApiResponse`, `PagedResult`
- **Entities**: `User`, `Supplier`, `PurchaseRequest`
- Framework-agnostic; defines the domain model and contracts

### 4. **SupplierServiceNet.CrossCutting** (Cross-Cutting Concerns)
- Location: `SupplierServiceNet.CrossCutting/`
- Responsibilities:
  - Shared DTOs and options (`Options/`, `Dtos/`)
  - Helper utilities (`Helper/`)
- Contains infrastructure-agnostic shared utilities

### 5. **SupplierServiceNet.Infrastructure** (Data Access & Persistence)
- Location: `SupplierServiceNet.Infraestructure/`
- Responsibilities:
  - Entity Framework DbContext (`Data/ApplicationDbContext.cs`)
  - Repository implementations (`Repository/`)
  - Database migrations (`Migrations/`)
  - Unit of Work pattern (`Repository/WorkContainer/`)
- **Key Classes**:
  - `ApplicationDbContext` - extends `IdentityDbContext<User>`
  - `Repository<T>` - generic repository base
  - `IUnitOfWork` / `UnitOfWork` - transaction coordination
  - Specific repositories: `UserRepository`, `SupplierRepository`, `PurchaseRequestRepository`
- Supports multiple databases via Entity Framework providers (SQL Server, PostgreSQL, Oracle)

## Key Technologies & Frameworks

| Technology | Purpose | Version |
|-----------|---------|---------|
| ASP.NET Core | Web framework | 8.0 |
| Entity Framework Core | ORM | 7.0.11 |
| JWT Bearer | Authentication | 7.0.8 |
| Swagger/Swashbuckle | API documentation | 6.5.0 |
| AutoMapper | DTO mapping | 12.0.1 |
| Serilog | Structured logging | 7.0.0 |
| Cloudinary | Image storage | 1.28.0 |
| ASP.NET Identity | User management | 7.0.11 |
| Asp.Versioning | API versioning | 8.1.1 |

## Configuration & Secrets

**Connection String & API Settings** (`appsettings.json`):
- SQL Server connection: `"Server=localhost,14333;Database=stracon_db;..."`
- JWT secret key: `ApiSettings:Secreta`
- Cloudinary credentials: Cloud name, API key, API secret
- **⚠️ WARNING**: Secrets in `appsettings.json` should never be committed. Use `appsettings.Development.json` (git-ignored) for local development and environment variables/User Secrets in production.

**Logging** (Serilog):
- Configured to write to console and file (`/logs/log.txt`)
- Enriched with machine name and thread ID
- Minimum level: `Information`

**User Secrets** (for development):
```powershell
dotnet user-secrets init
dotnet user-secrets set "ApiSettings:Secreta" "your-secret-key"
dotnet user-secrets set "ConnectionStrings:ConexionSql" "your-connection-string"
```

## Authentication & Authorization

- **JWT Bearer** tokens for API authentication
- Default authorization policy requires authenticated users
- User creation via `LoginController` -> `UserService`
- Token validation parameters set in `ApplicationServicesExtensions.cs`:
  - No HTTPS requirement in development (`RequireHttpsMetadata = false`)
  - Token lifetime validation enabled
  - Clock skew set to zero for strict validation

## Database Schema & Entities

**Core Entities**:
1. **User** - Extends `IdentityUser` with `CreatedAt` field
2. **Supplier** - Supplier information and metadata
3. **PurchaseRequest** - Purchase order requests

**DbContext**: `ApplicationDbContext` extends `IdentityDbContext<User>` to integrate ASP.NET Identity tables.

**Database Seeding**: Auto-runs on startup via `SeedData.InitializeAsync()` in `Program.cs`.

## Dependency Injection Pattern

The project uses extension methods for service registration:
- `AddApplicationServices()` - Business logic services, authentication, authorization
- `AddPersistence()` - DbContext, Unit of Work, generic repository
- `AddCustomResponseCompression()` - Response compression middleware
- `AddCustomHealthChecks()` - Health check endpoints

All registered as **scoped** (per-request lifetime) for EF Core and repositories.

## API Documentation

**Swagger UI** is available at `/swagger/ui` in development mode.
- Configured via `SwaggerServicesExtensions.cs`
- API versioning integrated (group by version v1, v2, etc.)
- Schema generation from controller endpoints and DTOs

## Common Patterns & Practices

### Repository & Unit of Work
- Generic `Repository<T>` base class with CRUD operations
- `IUnitOfWork` for coordinating multiple repository operations and transactions
- All data access abstracted through interfaces (`IRepository<T>`, `IUnitOfWork`)

### DTOs & Mapping
- AutoMapper profiles defined in `Program.cs` (e.g., `PeliculasMapper`)
- DTOs separated from entities: `UserDto`, `DataUserDto`, etc.
- Maps configured with `.ReverseMap()` for bidirectional conversion

### Error Handling
- Custom exception middleware in `Middlewares/ExceptionMiddleware.cs`
- Standardized API responses via `ApiResponse<T>` in CrossCutting
- Paginated results via `PagedResult<T>`

### Logging
- Serilog integrated throughout the application
- Request logging via `UseSerilogRequestLogging()`
- Structured logging with context enrichment

### API Versioning
- URL segment versioning: `/api/v1/endpoint` format
- Default version set to 1.0 in `Program.cs`
- Configured via `Asp.Versioning` package

## File & Code Organization

```
SupplierServiceNet/
  ├── SupplierServiceNet/              # Main API project
  │   ├── Controllers/                 # HTTP endpoints
  │   ├── Extensions/                  # Service registration extensions
  │   ├── Middlewares/                 # Custom middleware
  │   ├── Seed/                        # Database seeding logic
  │   ├── Program.cs                   # Startup & service configuration
  │   └── appsettings*.json            # Configuration
  ├── SupplierServiceNet.Application/  # Business logic layer
  │   ├── Services/                    # Application services
  │   ├── Dtos/                        # Data transfer objects
  │   └── Interfaces/                  # Service contracts
  ├── SupplierServiceNet.Core/         # Domain/entities layer
  │   ├── Entities/                    # Domain models
  │   ├── IRepositorio/                # Repository interfaces
  │   └── Interfaces/                  # Domain service interfaces
  ├── SupplierServiceNet.CrossCutting/ # Cross-cutting concerns
  │   ├── Dtos/                        # Shared DTOs
  │   ├── Options/                     # Configuration options
  │   └── Helper/                      # Utility classes
  └── SupplierServiceNet.Infraestructure/  # Data access layer
      ├── Data/                        # DbContext
      ├── Repository/                  # Repository implementations
      ├── Migrations/                  # EF Core migrations
      └── DependencyInjection.cs       # Infrastructure service registration
```

## Code Style & Analysis

- **EditorConfig** (`.editorconfig`):
  - Enforces C# coding standards (StyleCop Analyzers)
  - Naming conventions: PascalCase for types/methods, `I`-prefix for interfaces
  - Expression-bodied members: properties and lambdas preferred
  - Nullable reference types enabled (Nullable=enable in most projects)
  - CS8618 nullability warning suppressed (Nullable=disable in main project)

- **StyleCop.Analyzers** checks adherence to C# style rules

## Development Workflow

1. **Feature Development**:
   - Add entity to `Core/Entities/` if needed
   - Create repository interface in `Core/IRepositorio/`
   - Implement repository in `Infrastructure/Repository/`
   - Create service interface in `Application/Services/`
   - Implement service with business logic
   - Create/update DTOs in `Application/Dtos/`
   - Add controller endpoint in `SupplierServiceNet/Controllers/`
   - Register services in `Program.cs` or relevant extension class

2. **Database Changes**:
   - Modify entity in `Core/Entities/`
   - Add EF Core migration: `dotnet ef migrations add DescriptiveName`
   - Review generated migration file
   - Update seed data in `Seed/` if needed
   - Migration auto-runs on startup

3. **Testing** (framework to be added):
   - Mock repositories via `IRepository<T>` and `IUnitOfWork` interfaces
   - Test services in isolation from infrastructure
   - Use real DbContext for integration tests (SQL Server/PostgreSQL)

## Known Configuration Details

- **Windows-only target**: Projects target `net8.0-windows7.0` (can be updated to `net8.0` for cross-platform)
- **SQL Server default**: Connection string points to local SQL Server. PostgreSQL provider also available via Npgsql.EntityFrameworkCore.PostgreSQL
- **Cloudinary integration**: Image uploads handled by `CloudinaryService` for supplier and default content
- **Development middleware**: Swagger only enabled in Development environment
- **CORS configuration**: Setup via `ConfigureCors()` in extensions
- **Health checks**: Endpoint at `/health`

## Quick Troubleshooting

| Issue | Solution |
|-------|----------|
| Migration won't apply | Ensure SQL Server is running at configured connection string. Check `appsettings.json` or `.UserSecrets`. |
| Authentication fails | Verify JWT secret key in config matches token generation. Check token expiry. |
| Cloudinary upload fails | Verify Cloudinary credentials in `appsettings.json` and network access. |
| Port already in use | Check `launchSettings.json` for configured ports. Change port if needed. |
| Database seeding errors | Review `Seed/SeedData.cs` for data constraints. Ensure migrations have run. |
