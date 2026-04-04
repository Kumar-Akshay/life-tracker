# CLAUDE.md — LifeTrackerPro Codebase Guide

This document describes the architecture, conventions, and development workflows for AI assistants working on this codebase.

---

## Project Overview

LifeTrackerPro is a full-stack personal life tracking application built with **.NET 9** (ASP.NET Core Minimal API backend) and **Blazor WebAssembly** frontend. It enables users to log 24-hour time allocation, mood/productivity scores, spending, and personal notes.

---

## Repository Structure

```
life-tracker/
├── LifeTrackerPro.sln
├── README.md
├── PLAN.md                         # MVP implementation tracker (phase checklist)
├── .github/workflows/ci.yml        # GitHub Actions CI/CD
├── src/
│   ├── LifeTrackerPro.Domain/      # Entities, enums, base classes (no dependencies)
│   ├── LifeTrackerPro.Shared/      # DTOs, constants (shared between API and Client)
│   ├── LifeTrackerPro.Application/ # CQRS handlers, validators, AutoMapper profiles
│   ├── LifeTrackerPro.Infrastructure/ # EF Core, JWT, encryption, datetime services
│   ├── LifeTrackerPro.API/         # ASP.NET Core Minimal API host
│   └── LifeTrackerPro.Client/      # Blazor WebAssembly frontend
└── tests/
    └── LifeTrackerPro.UnitTests/   # xUnit tests
```

### Layer Dependency Rules (Clean Architecture)

```
Domain ← Application ← Infrastructure
Domain ← Shared ← Application
                  Infrastructure ← API
                  Application    ← API
Shared ← Client
```

- **Domain**: Pure C#, zero external dependencies.
- **Application**: References Domain + Shared only. No infrastructure concerns.
- **Infrastructure**: Implements interfaces defined in Application.
- **API**: Wires everything together; references Application + Infrastructure + Shared.
- **Client**: References Shared only (DTOs/constants).

---

## Tech Stack

| Concern | Technology | Version |
|---|---|---|
| Runtime | .NET | 9.0 |
| Backend | ASP.NET Core Minimal API | 9.0 |
| Frontend | Blazor WebAssembly + MudBlazor | 9.0 / 9.0.0 |
| ORM | Entity Framework Core | 9.0.1 |
| Database | PostgreSQL 17 (prod) / SQLite (dev) | — |
| Auth | ASP.NET Identity + JWT Bearer | 9.0.1 / 6.9.0 |
| CQRS | MediatR | 14.0.0 |
| Validation | FluentValidation | 12.1.1 |
| Mapping | AutoMapper | 16.0.0 |
| Testing | xUnit + Moq + FluentAssertions | 2.9.2 / 4.20.72 / 8.8.0 |
| API Docs | Swagger / Swashbuckle | 6.9.0 |

---

## Build & Run

### Prerequisites
- .NET 9 SDK
- PostgreSQL 17 (or SQLite for local dev — detected automatically)

### Commands

```bash
# Restore and build the entire solution
dotnet build LifeTrackerPro.sln

# Run all unit tests
dotnet test tests/LifeTrackerPro.UnitTests/LifeTrackerPro.UnitTests.csproj

# Run the API (development — auto-migrates DB, exposes Swagger at /swagger)
dotnet run --project src/LifeTrackerPro.API

# Run the Blazor client
dotnet run --project src/LifeTrackerPro.Client

# Publish API
dotnet publish src/LifeTrackerPro.API/LifeTrackerPro.API.csproj --configuration Release

# Publish Client (outputs static files to wwwroot)
dotnet publish src/LifeTrackerPro.Client/LifeTrackerPro.Client.csproj --configuration Release
```

### Database

The app auto-detects the connection string:
- If `ConnectionStrings:DefaultConnection` contains `postgresql://` or `Host=`, it uses Npgsql.
- Otherwise it falls back to SQLite (`lifetrackerpro.db`).

In development mode (`app.Environment.IsDevelopment()`), `database.MigrateAsync()` runs on startup.

---

## Architecture Patterns

### CQRS with MediatR

All business logic lives in `LifeTrackerPro.Application/Features/`. Each feature folder contains:

```
Features/
└── DailyLogs/
    ├── Commands/
    │   └── UpsertDailyLogCommand.cs    # IRequest<ApiResponse<DailyLogDto>>
    ├── Handlers/
    │   └── UpsertDailyLogCommandHandler.cs  # IRequestHandler<...>
    ├── Queries/
    │   └── GetDailyLogQuery.cs
    └── Validators/
        └── UpsertDailyLogValidator.cs  # AbstractValidator<UpsertDailyLogCommand>
```

**Naming conventions:**
- Commands: `{Action}Command` — e.g., `RegisterCommand`, `UpsertDailyLogCommand`
- Queries: `Get{Entity}Query` — e.g., `GetDailyLogQuery`, `GetCategoriesQuery`
- Handlers: `{CommandOrQuery}Handler`
- Validators: `{Command}Validator`

### Validation Pipeline

`ValidationBehaviour<TRequest, TResponse>` (MediatR pipeline behavior) runs FluentValidation validators before every handler. Validation errors are collected and thrown as `ValidationException`, then caught by `GlobalExceptionMiddleware` and returned as:

```json
{ "success": false, "errors": ["Field X is required", ...] }
```

### API Response Envelope

All endpoints return `ApiResponse<T>`:

```csharp
ApiResponse<T> {
    bool Success,
    string? Message,
    T? Data,
    List<string>? Errors
}
```

### Multi-Tenancy via Global Query Filters

All user-scoped entities (`DailyLog`, `Category`, `Transaction`, `UserSettings`) have a `UserId` foreign key. The `LifeTrackerDbContext` applies global query filters using `ICurrentUserService.UserId` — queries automatically scope to the authenticated user.

---

## Domain Entities

| Entity | Key Fields |
|---|---|
| `ApplicationUser` | Extends `IdentityUser<Guid>`; adds FullName, Timezone, AvatarUrl |
| `DailyLog` | Date (DateOnly), HourSlots (JSONB string[24]), Weight, AmountSpent, MoodScore (1-5), ProductivityScore (1-10), Highlight, Notes |
| `Category` | Code, Name, Letter (single char), Color (hex), IconName, GroupName, SortOrder |
| `Transaction` | Date, Description, Amount, CategoryTag, CurrencyCode |
| `UserSettings` | TimeGranularity, ThemeMode, CurrencyCode, Timezone, WeekStartDay, MonthlyBudget |
| `RefreshToken` | Token, ExpiresAt, DeviceInfo, IsRevoked |
| `AuditLog` | Action, EntityType, EntityId, IpAddress, Timestamp |

**Base classes:**
- `BaseEntity`: `Id (Guid)`, `CreatedAt`, `UpdatedAt`
- `BaseUserEntity`: extends `BaseEntity` with `UserId` + `ApplicationUser` nav property

---

## API Endpoints

Base path: `/api`

| Group | Route | Auth | Methods |
|---|---|---|---|
| Auth | `/api/auth` | Public (except `/logout`) | POST register, login, refresh, logout |
| DailyLogs | `/api/daily-logs` | Required | GET (range), GET `/{date}`, PUT `/{date}` |
| Categories | `/api/categories` | Required | GET, POST, PUT `/{id}`, PUT `/{id}/archive`, PUT `/{id}/restore`, PUT `/reorder`, DELETE `/{id}` |
| Dashboard | `/api/dashboard` | Required | GET (period + date range query params) |
| Settings | `/api/settings` | Required | GET, PUT |
| Profile | `/api/profile` | Required | GET, PUT |

Route constants are in `LifeTrackerPro.Shared/Constants/ApiRoutes.cs`.

---

## Authentication

- **JWT Bearer** (HS256, 15-minute access token expiry)
- **Refresh tokens** stored in DB (7-day expiry), with device info and revocation tracking
- JWT claims: `sub` (UserId), `email`, `jti`, `fullName`, `timezone`
- `ICurrentUserService` extracts `UserId` from the `sub` claim

Client-side: `AuthStateProvider` manages token state in memory and fires `AuthenticationStateChanged` events.

**Password policy**: min 8 chars, requires digit + lowercase + uppercase, no special char required.

---

## Frontend (Blazor WebAssembly)

### Pages

| Route | Page | Description |
|---|---|---|
| `/dashboard` | Dashboard | KPI cards, distribution charts, weekly trends |
| `/daily-log` | DailyLogger | 24-hour grid, category picker, day selector |
| `/settings` | Settings | Categories, tracking, AI, notifications, profile |
| `/login` | Login | Auth form |
| `/register` | Register | Registration form |
| `/spending`, `/goals`, `/life-pulse`, `/habits`, `/productivity`, `/videos`, `/books`, `/weekly-review` | Feature pages | Scaffolded, not yet implemented |

### UI Theme

- Dark background: `#0c0a1a`
- Primary accent: `#6366F1` (Indigo)
- Fonts: DM Sans (body), Space Mono (data/code)
- CSS variables: `--ink`, `--ink-muted`, `--ink-soft`, `--ink-faint`, `--border`, `--pampas`
- Component library: **MudBlazor 9**

### Client Services (inject via DI)

- `AuthService` — register, login, logout
- `AuthStateProvider` — current user state, token management
- `DailyLogService` — CRUD for daily logs
- `CategoryService` — category management
- `DashboardService` — analytics fetch
- `SettingsService` — get/update user settings

---

## Testing Conventions

Test project: `tests/LifeTrackerPro.UnitTests/`

- **Framework**: xUnit
- **Mocking**: Moq (`Mock<T>`, `.Setup()`, `.Verify()`)
- **Assertions**: FluentAssertions (`.Should().Be()`, `.Should().Contain()`, etc.)
- **DB**: EF Core InMemory provider via `TestDbContext`

**Test file locations:**
- `Handlers/` — Command/Query handler tests
- `Validators/` — FluentValidation validator tests
- `Domain/` — Domain entity tests

**Pattern**: Arrange-Act-Assert. Test class names mirror the class under test (e.g., `UpsertDailyLogCommandHandlerTests`).

---

## CI/CD (GitHub Actions)

File: `.github/workflows/ci.yml`

**On every push/PR:**
1. `dotnet restore`
2. `dotnet build --configuration Release`
3. `dotnet test` with TRX format and code coverage — artifacts uploaded

**On push to `main`:**
4. Publish API → artifact `./publish/api`
5. Publish Client → artifact `./publish/client/wwwroot`

---

## Key Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "JwtSettings": { "Key": "...", "Issuer": "...", "Audience": "...", "ExpiryMinutes": 15 },
  "EncryptionKey": "...",
  "ClientUrl": "https://localhost:7002",
  "DefaultCategories": [ ... ]
}
```

**12 default categories** seeded on user registration (from config, not DB seed):
Sleep, Work, Exercise, Commute, Chores, Social, Learning, Entertainment, Personal, Food, Grooming, Idle — each with color hex, sort order, group name, and single-letter code.

---

## Code Style Conventions

- **File-scoped namespaces**: `namespace LifeTrackerPro.Domain.Entities;`
- **Nullable reference types**: enabled (`#nullable enable` implicitly via csproj)
- **Implicit usings**: enabled
- **Records for DTOs**: immutable `record` types with `init`-only properties
- **Interfaces for abstractions**: `I{Name}Service` in Application layer, implemented in Infrastructure
- **Extension methods for DI**: `AddApplicationServices()`, `AddInfrastructureServices()`
- No `var` usage — explicit types preferred in handler/service code

### Custom Exceptions (Application layer)

| Exception | HTTP Status |
|---|---|
| `NotFoundException` | 404 |
| `ForbiddenAccessException` | 403 |
| `BadRequestException` | 400 |

All caught by `GlobalExceptionMiddleware` and mapped to `ApiResponse`.

---

## PWA

- Service worker for offline support
- `manifest.json` with icons (192×192, 512×512), `standalone` display, theme color `#0c0a1a`
- iOS meta tags for Add to Home Screen support

---

## Pending Work (from PLAN.md)

Steps 0–10 are complete. The following are still pending:

- **Step 11**: EF Core migrations (`dotnet ef migrations add Initial`)
- **Step 12**: Integration tests (API-level tests with TestServer/WebApplicationFactory)
- **Step 13**: Deployment configuration (Docker, reverse proxy, production secrets)

Feature pages scaffolded but not yet implemented: Spending, Goals, Life Pulse, Habits, Productivity, Videos, Books, Weekly Review.
