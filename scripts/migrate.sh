#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────
# LifeTrackerPro — EF Core Migration Helper
# ─────────────────────────────────────────────────────────────
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
INFRA_PROJECT="$REPO_ROOT/src/LifeTrackerPro.Infrastructure/LifeTrackerPro.Infrastructure.csproj"
API_PROJECT="$REPO_ROOT/src/LifeTrackerPro.API/LifeTrackerPro.API.csproj"
MIGRATIONS_DIR="$REPO_ROOT/src/LifeTrackerPro.Infrastructure/Migrations"

print_usage() {
    echo "Usage:"
    echo "  $0 add <MigrationName>   — create a new migration"
    echo "  $0 update                — apply pending migrations to the database"
    echo "  $0 list                  — list all migrations"
    echo "  $0 revert                — revert the last migration"
    echo ""
    echo "Environment variables:"
    echo "  ASPNETCORE_ENVIRONMENT  (default: Development)"
    echo "  ConnectionStrings__DefaultConnection  — override connection string"
}

ensure_ef_tool() {
    if ! dotnet ef --version &>/dev/null 2>&1; then
        echo "Installing dotnet-ef tool..."
        dotnet tool install --global dotnet-ef
        export PATH="$PATH:$HOME/.dotnet/tools"
    fi
}

run_ef() {
    dotnet ef "$@" \
        --project "$INFRA_PROJECT" \
        --startup-project "$API_PROJECT" \
        --output-dir Migrations
}

case "${1:-}" in
    add)
        if [[ -z "${2:-}" ]]; then
            echo "Error: migration name required"
            print_usage
            exit 1
        fi
        ensure_ef_tool
        echo "Creating migration: $2"
        run_ef migrations add "$2"
        echo "Migration '$2' created in $MIGRATIONS_DIR"
        ;;
    update)
        ensure_ef_tool
        echo "Applying migrations..."
        run_ef database update
        echo "Migrations applied."
        ;;
    list)
        ensure_ef_tool
        run_ef migrations list
        ;;
    revert)
        ensure_ef_tool
        echo "Reverting last migration..."
        run_ef migrations remove
        echo "Last migration removed."
        ;;
    *)
        print_usage
        exit 1
        ;;
esac
