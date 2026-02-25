# LifeTracker Pro — Phase 1 MVP Implementation Plan

## Task Tracker

- [x] **Step 0** — Prerequisites (SDK + PostgreSQL installed)
- [x] **Step 1** — Solution Structure (7 projects, all NuGet packages)
- [x] **Step 2** — Domain Layer (8 entities, 4 enums, base classes)
- [x] **Step 3** — Application Layer (CQRS commands/queries/handlers, validators, mappings, DI)
- [x] **Step 4** — Infrastructure Layer (DbContext, entity configs, JWT/Encryption/DateTime services, DI)
- [x] **Step 5** — API Layer (6 endpoint groups, middleware, Program.cs, Swagger)
- [x] **Step 6** — Blazor WASM Client (5 pages, 6 services, layout, theme, PWA)
- [x] **Step 7** — Database Seeding (12 default categories on registration)
- [x] **Step 8** — PWA Configuration (manifest, service worker, iOS meta tags)
- [x] **Step 9** — Unit Tests (28 tests — validators, handlers, domain, API response)
- [x] **Step 10** — CI/CD Pipeline (GitHub Actions: build, test, publish API & Client)
- [ ] **Step 11** — EF Core Migrations (run after PostgreSQL is available)
- [ ] **Step 12** — Integration Tests (API endpoint tests with WebApplicationFactory)
- [ ] **Step 13** — Docker Compose (API + Client + PostgreSQL)

---

## Prerequisites (Step 0) ✅

Install .NET 10 SDK and PostgreSQL 17:

```bash
brew install --cask dotnet-sdk        # .NET 10.0.103
brew install postgresql@17
brew services start postgresql@17
createdb lifetrackerpro
```

---

## Step 1: Solution Structure (Clean Architecture) ✅

Create the solution with 8 projects following the doc's recommended layout:

```
LifeTrackerPro.sln
├── src/
│   ├── LifeTrackerPro.Domain/           # Entities, enums, value objects
│   ├── LifeTrackerPro.Application/      # Use cases (CQRS), interfaces, DTOs, validators
│   ├── LifeTrackerPro.Infrastructure/   # EF Core, repositories, encryption, services
│   ├── LifeTrackerPro.API/              # .NET 10 Minimal API, auth, middleware
│   ├── LifeTrackerPro.Client/           # Blazor WASM, pages, components, PWA
│   └── LifeTrackerPro.Shared/           # Shared DTOs, constants, enums
└── tests/
    └── LifeTrackerPro.UnitTests/        # xUnit tests
```

**NuGet Packages:**
- Domain: none (pure C#)
- Application: MediatR, FluentValidation, AutoMapper
- Infrastructure: Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.AspNetCore.Identity.EntityFrameworkCore
- API: Microsoft.AspNetCore.Authentication.JwtBearer, Swashbuckle (Swagger)
- Client: Microsoft.AspNetCore.Components.WebAssembly, MudBlazor
- Shared: none (pure C#)

---

## Step 2: Domain Layer — Entities & Enums ✅

Create all Phase 1 entities matching the DB schema in section 4:

- **User** — Id (Guid), Email, FullName, PasswordHash, Timezone, AvatarUrl, etc.
- **DailyLog** — Id, UserId, Date, HourSlots (string/JSON), Weight, AmountSpent, Highlight, MoodScore, ProductivityScore, IsAutoFilled
- **Category** — Id, UserId, Code, Name, Letter, Color, IconName, SortOrder, IsActive, GroupName, Description, IsArchived
- **Transaction** — Id, UserId, Date, Description, Amount, CategoryTag, IsRecurring, CurrencyCode
- **UserSettings** — Id, UserId, TimeGranularity, ThemeMode, AiAutoFillEnabled, MonthlyBudget, etc.
- **RefreshToken** — Id, UserId, Token, ExpiresAt, DeviceInfo, IsRevoked
- **AuditLog** — Id, UserId, Action, EntityType, EntityId, Timestamp, IpAddress

Enums: GoalType, BookStatus, VideoStatus, NotableType, MoodScore, ThemeMode

---

## Step 3: Application Layer — CQRS + DTOs ✅

Organize by feature with MediatR commands/queries:

### Auth
- `RegisterCommand` / `LoginCommand` / `RefreshTokenCommand` / `LogoutCommand`
- `ForgotPasswordCommand` / `ResetPasswordCommand` / `VerifyEmailCommand`

### DailyLogs
- `GetDailyLogQuery(Date)` / `GetDailyLogsQuery(DateRange)`
- `UpsertDailyLogCommand(Date, HourSlots, Weight, Spent, Highlight)`

### Categories
- `GetCategoriesQuery` / `CreateCategoryCommand` / `UpdateCategoryCommand`
- `ArchiveCategoryCommand` / `RestoreCategoryCommand` / `ReorderCategoriesCommand`
- `DeleteCategoryCommand`

### Dashboard
- `GetDashboardQuery(Period, DateRange)` — aggregates hours/category, spend summary

### Settings
- `GetSettingsQuery` / `UpdateSettingsCommand`

### Profile
- `GetProfileQuery` / `UpdateProfileCommand`

**Shared DTOs** (in Shared project): DailyLogDto, CategoryDto, DashboardDto, UserSettingsDto, AuthResponseDto, etc.

**Validators** (FluentValidation): One per command (e.g., `UpsertDailyLogValidator` checks date not future, HourSlots valid JSON, etc.)

---

## Step 4: Infrastructure Layer — EF Core & Data Access ✅

### DbContext (`LifeTrackerDbContext`)
- Configure all entity mappings via `IEntityTypeConfiguration<T>`
- Global query filter: `.HasQueryFilter(x => x.UserId == _currentUserId)` on all user-scoped tables
- JSONB column mapping for HourSlots, Tags, Notes fields
- Value converters for encrypted fields (Weight, AmountSpent, Highlight)

### Migrations
- Initial migration with all Phase 1 tables
- Seed default 12 categories per new user registration

### Repositories / Services
- `ICurrentUserService` — extracts UserId from JWT claims
- `IEncryptionService` — AES-256-GCM for sensitive columns
- `IJwtService` — token generation/validation
- `IDateTimeService` — abstraction for testability

### Identity Integration
- ASP.NET Core Identity with custom `ApplicationUser` extending `IdentityUser<Guid>`
- Argon2id password hasher (or bcrypt via Identity default)

---

## Step 5: API Layer — Minimal API Endpoints ✅

Group endpoints into static classes with `MapGroup()`:

### Auth Endpoints (`/api/auth/`)
- `POST /register` — create account, return JWT
- `POST /login` — validate creds, return JWT + refresh token (HttpOnly cookie)
- `POST /refresh` — exchange refresh token
- `POST /logout` — revoke refresh token
- `POST /forgot-password` — send reset email (stub for MVP)
- `POST /reset-password` — set new password

### DailyLog Endpoints (`/api/daily-logs/`)
- `GET ?from=&to=` — date range query
- `GET /{date}` — single day
- `PUT /{date}` — create or update

### Category Endpoints (`/api/categories/`)
- `GET` — list all (active + archived)
- `POST` — create
- `PUT /{id}` — update
- `PUT /{id}/archive` — archive
- `PUT /{id}/restore` — restore
- `PUT /reorder` — batch sort order
- `DELETE /{id}` — hard delete (if no logs reference it)

### Dashboard Endpoint (`/api/dashboard/`)
- `GET ?period=&from=&to=` — aggregated analytics

### Settings & Profile
- `GET/PUT /api/settings`
- `GET/PUT /api/profile`
- `GET /api/sessions` / `DELETE /api/sessions/{id}`

### Middleware
- JWT authentication + authorization
- Global exception handler
- Request logging
- Rate limiting (100 req/min)
- CORS for Blazor WASM origin

### Swagger
- OpenAPI docs at `/swagger`

---

## Step 6: Blazor WASM Client — Pages & Components ✅

### Layout & Navigation
- `MainLayout.razor` — dark theme (#0c0a1a background), sidebar nav, top bar
- Sidebar: Dashboard, Daily Logger, Settings links (Phase 1 only)
- PWA manifest + service worker for installability

### Pages

**Login / Register** (`/login`, `/register`)
- Email/password forms, JWT stored in memory, refresh via HttpOnly cookie

**Daily Logger** (`/daily-log` or `/daily-log/{date}`)
- 24-hour time grid (1×24 for desktop, 2×12 for mobile)
- Each cell shows category letter + color
- Click cell → category picker (bottom sheet on mobile, popover on desktop)
- Weight, Amount Spent, Highlight inputs below grid
- Date navigation (prev/next day, date picker)
- Save button → `PUT /api/daily-logs/{date}`

**Dashboard** (`/` or `/dashboard`)
- KPI cards: Total logged hours, Total spent, Current streak, Days logged
- Pie chart: Hours per category (current period)
- Bar chart: Daily hours breakdown over time
- Spend summary card
- Period selector (Week / Month / Year)

**Settings** (`/settings`)
- **Categories section**: List with color swatches, tap/click to expand inline editor
  - Name, Letter Code, Color Picker, Icon, Group, Description, Sort Order, Archive/Restore
  - Drag-to-reorder
- **Tracking section**: Time granularity, theme toggle (dark/light)
- **Profile section**: Name, email, timezone, avatar
- **Data section**: Export button (stub for Phase 1)

### Shared Components
- `TimeGrid.razor` — the 24-hour category grid (core component)
- `CategoryPicker.razor` — bottom sheet / popover for selecting category
- `CategoryBadge.razor` — colored letter badge
- `DateNavigator.razor` — prev/next/today with date display
- `KpiCard.razor` — stat card with icon, value, label
- `ChartContainer.razor` — wrapper for charts (using MudBlazor charts or Chart.js interop)

### Services (Client-side)
- `AuthService` — login, register, token refresh, logout
- `DailyLogService` — CRUD via HttpClient
- `CategoryService` — CRUD via HttpClient
- `DashboardService` — fetch aggregated data
- `SettingsService` — get/update settings

### Theme / Design System
- MudBlazor with custom dark theme (background: #0c0a1a, accent: #6366F1 Indigo)
- Typography: DM Sans body, Space Mono for data
- Mobile-first responsive

---

## Step 7: Database Seeding & Excel Import (Stub) ✅

- On first user registration, seed 12 default categories (from the Excel mappings):
  Sleep(S), Work(W), Exercise(E), Commute(C), Chores(H), Social(O), Learning(L), Entertainment(N), Personal(P), Food(F), Grooming(G), Idle(I)
- Create a `/api/import/excel` stub endpoint (full implementation deferred but structure in place)

---

## Step 8: PWA Configuration ✅

- `manifest.json` — app name, icons, theme color, display: standalone
- Service worker — basic offline shell caching
- iOS meta tags for Add to Home Screen

---

## Summary of Files to Create (~80-90 files)

| Layer | Approx. Files | Key Items |
|-------|--------------|-----------|
| Domain | 8-10 | Entities, enums, base classes |
| Application | 25-30 | Commands, queries, handlers, DTOs, validators, interfaces |
| Infrastructure | 10-12 | DbContext, configs, services, migrations |
| API | 8-10 | Endpoint groups, middleware, Program.cs |
| Client | 20-25 | Pages, components, services, layout, theme |
| Shared | 5-8 | DTOs, constants, enums |
| Tests | 3-5 | Basic auth & daily log tests |

Execution will proceed step-by-step, building from the inner layers (Domain) outward (Client).
