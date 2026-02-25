# LifeTracker Pro

A full-stack life tracking application built with **.NET 9** and **Blazor WebAssembly**.

Track every hour of your life. Visualize patterns. Build better habits.

## Architecture

Clean Architecture with 7 projects:

```
LifeTrackerPro.sln
├── src/
│   ├── LifeTrackerPro.Domain           # Entities, enums, base classes
│   ├── LifeTrackerPro.Shared           # DTOs, constants
│   ├── LifeTrackerPro.Application      # CQRS (MediatR), validators, mappings
│   ├── LifeTrackerPro.Infrastructure   # EF Core, Identity, JWT, encryption
│   ├── LifeTrackerPro.API              # Minimal API endpoints, middleware
│   └── LifeTrackerPro.Client           # Blazor WASM + MudBlazor + PWA
└── tests/
    └── LifeTrackerPro.UnitTests        # xUnit + Moq + FluentAssertions
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 9 Minimal API |
| Frontend | Blazor WebAssembly + MudBlazor 9 |
| Database | PostgreSQL 17 + EF Core 9 |
| Auth | ASP.NET Core Identity + JWT |
| CQRS | MediatR 14 |
| Validation | FluentValidation 12 |
| Mapping | AutoMapper 16 |
| Testing | xUnit + Moq + FluentAssertions |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 17](https://www.postgresql.org/download/)

## Getting Started

```bash
# Clone
git clone https://github.com/<your-org>/LifeTracker.git
cd LifeTracker

# Create database
createdb lifetrackerpro

# Run API
cd src/LifeTrackerPro.API
dotnet run

# Run Client (separate terminal)
cd src/LifeTrackerPro.Client
dotnet run
```

## Build & Test

```bash
dotnet build LifeTrackerPro.sln
dotnet test
```

## License

MIT
