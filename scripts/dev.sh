#!/usr/bin/env bash
# TastyBites local dev helper (macOS)
#   ./scripts/dev.sh setup     -> install .NET 7, Node 18, MySQL 8.4, ABP CLI; create DB; restore; migrate
#   ./scripts/dev.sh libs      -> download the login page styles into the API's wwwroot/libs
#   ./scripts/dev.sh backend   -> run the API on http://localhost:5001  (Swagger: /swagger)
#   ./scripts/dev.sh admin     -> run the admin app (admin.angular) on http://localhost:4200
#   ./scripts/dev.sh proxy     -> generate Angular API services from the running backend
#   ./scripts/dev.sh add-migration NAME -> create a database migration after changing an entity
#   ./scripts/dev.sh migrate   -> update the database and seed data (run after adding permissions or migrations)
#   ./scripts/dev.sh public    -> run the public site (fresh.angular) on http://localhost:4201
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
APP="$ROOT/TastyBites"
HOST="$APP/aspnet-core/src/TastyBites.HttpApi.Host"
MIGRATOR="$APP/aspnet-core/src/TastyBites.DbMigrator"
NG="$APP/fresh.angular"
ADMIN_NG="$APP/admin.angular"

DOTNET7_DIR="$HOME/.dotnet7"
DB_NAME="tasty-bites"
DB_USER="tasty-bites-db-user"
DB_PASS="Mypasswordy"

ok()   { printf "\033[32m✔ %s\033[0m\n" "$*"; }
step() { printf "\n\033[36m▶ %s\033[0m\n" "$*"; }
fail() { printf "\033[31m✘ %s\033[0m\n" "$*"; exit 1; }

use_dotnet7() {
  export DOTNET_ROOT="$DOTNET7_DIR"
  export PATH="$DOTNET7_DIR:$HOME/.dotnet/tools:$PATH"
  export DOTNET_CLI_TELEMETRY_OPTOUT=1
}

use_node18() {
  export NVM_DIR="$HOME/.nvm"
  [ -s "$NVM_DIR/nvm.sh" ] || fail "nvm not found in ~/.nvm. Install it: https://github.com/nvm-sh/nvm"
  # shellcheck disable=SC1091
  . "$NVM_DIR/nvm.sh"
  nvm install 18 >/dev/null
  nvm use 18 >/dev/null
}

setup() {
  step "Homebrew"
  command -v brew >/dev/null || fail "Homebrew missing. Install from https://brew.sh then re-run."
  ok "brew $(brew --version | head -1)"

  step ".NET 7 SDK (installed side-by-side in $DOTNET7_DIR, your other SDKs are untouched)"
  if [ ! -x "$DOTNET7_DIR/dotnet" ] || ! "$DOTNET7_DIR/dotnet" --list-sdks | grep -q '^7\.'; then
    curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
    bash /tmp/dotnet-install.sh --channel 7.0 --install-dir "$DOTNET7_DIR"
  fi
  use_dotnet7
  ok "dotnet $(dotnet --version)"

  step "Node 18 (via nvm, Angular 16 needs Node 16/18)"
  use_node18
  ok "node $(node -v)"

  step "MySQL (existing server on 127.0.0.1:3306)"
  MYSQL_BIN="$(command -v mysql || echo /opt/homebrew/opt/mysql@8.4/bin/mysql)"
  if "$MYSQL_BIN" -h127.0.0.1 -P3306 -u"$DB_USER" -p"$DB_PASS" -e "use \`$DB_NAME\`" 2>/dev/null; then
    ok "connected as '$DB_USER', database '$DB_NAME' exists"
  else
    fail "Can't reach MySQL on 127.0.0.1:3306 as '$DB_USER'. Start your MySQL (XAMPP/MAMP) and re-run."
  fi

  step "ABP CLI 7.4.5"
  if ! abp --version 2>/dev/null | grep -q '7\.4\.5'; then
    dotnet tool uninstall -g Volo.Abp.Cli >/dev/null 2>&1 || true
    dotnet tool install -g Volo.Abp.Cli --version 7.4.5
  fi
  ok "abp cli ready"

  step "Backend: restore + build"
  dotnet build "$APP/aspnet-core/TastyBites.sln" -v q -nologo
  ok "build ok"

  step "Backend: client libs for the login page (abp install-libs)"
  libs
  ok "wwwroot/libs installed"

  step "Database: run migrations + seed (DbMigrator)"
  (cd "$MIGRATOR" && dotnet run --no-build)
  ok "database migrated and seeded"

  step "Frontend: npm install"
  (cd "$NG" && npm install --no-audit --no-fund)
  ok "angular deps installed"

  printf "\n\033[32mSetup done.\033[0m Now open two terminals:\n  ./scripts/dev.sh backend\n  ./scripts/dev.sh admin\nLogin: admin / 1q2w3E*\n"
}

libs() {
  use_dotnet7
  use_node18
  (cd "$HOST" && abp install-libs)
  [ -n "$(ls -A "$HOST/wwwroot/libs" 2>/dev/null)" ] || fail "wwwroot/libs is still empty"
}

backend() {
  use_dotnet7
  cd "$HOST" && ASPNETCORE_ENVIRONMENT=Development dotnet run
}

admin() {
  use_node18
  cd "$ADMIN_NG"
  [ -d node_modules ] || npm install --no-audit --no-fund
  npx ng serve --open
}

add_migration() {
  # Creates an EF Core migration from model changes: ./scripts/dev.sh add-migration AddRecipeImageUrl
  [ -n "${1:-}" ] || fail "Give the migration a name, e.g. ./scripts/dev.sh add-migration AddRecipeImageUrl"
  use_dotnet7
  dotnet ef --version 2>/dev/null | grep -q '^7\.' || dotnet tool update -g dotnet-ef --version 7.0.20
  (cd "$APP/aspnet-core/src/TastyBites.EntityFrameworkCore" && dotnet ef migrations add "$1")
}

migrate() {
  # Applies new migrations and seeds data, e.g. grants new permissions to the admin role
  use_dotnet7
  (cd "$MIGRATOR" && dotnet run)
}

proxy() {
  # Reads the running API (localhost:5001) and generates typed Angular services into admin.angular/src/app/proxy
  use_dotnet7
  use_node18
  curl -sf http://localhost:5001/api/abp/api-definition >/dev/null || fail "Backend isn't running. Start it first: ./scripts/dev.sh backend"
  cd "$ADMIN_NG" && abp generate-proxy -t ng
}

public() {
  use_node18
  cd "$NG"
  [ -d node_modules ] || npm install --no-audit --no-fund
  npx ng serve --open
}

case "${1:-}" in
  setup) setup ;;
  libs) libs ;;
  backend) backend ;;
  admin) admin ;;
  proxy) proxy ;;
  migrate) migrate ;;
  add-migration) shift; add_migration "$@" ;;
  public) public ;;
  *) echo "usage: ./scripts/dev.sh {setup|libs|backend|admin|public|proxy|add-migration NAME|migrate}"; exit 1 ;;
esac
