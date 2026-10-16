#!/usr/bin/env python3
"""Create fresh local secrets: for a new clone, a new Mac, or when appsettings.secrets.json is lost.

Run from the repo root:  python3 scripts/new-local-secrets.py
Never prints a secret value. Needs XAMPP MySQL running; uses its local `root` user (no password)
to set a new password for the app's DB user.

Writes (gitignored, chmod 600):
  TastyBites/aspnet-core/src/TastyBites.HttpApi.Host/appsettings.secrets.json
  TastyBites/aspnet-core/src/TastyBites.DbMigrator/appsettings.secrets.json
Afterwards: restart the backend and log in again. Back the two files up in your password manager.
"""
import base64
import json
import os
import secrets
import string
import subprocess
import sys
from pathlib import Path

SRC = Path(__file__).resolve().parent.parent / "TastyBites" / "aspnet-core" / "src"
MYSQL = "/Applications/XAMPP/xamppfiles/bin/mysql"
DB_NAME = "tasty-bites"
DB_USER = "tasty-bites-db-user"


def mysql(user, sql, pwd=""):
    return subprocess.run(
        [MYSQL, "-u", user, "-N", "-e", sql],
        env={**os.environ, "MYSQL_PWD": pwd},
        capture_output=True,
        text=True,
    )


def save(path, data):
    path.write_text(json.dumps(data, indent=2) + "\n", encoding="utf-8")
    os.chmod(path, 0o600)  # only your user can read it


# Letters and digits only, so the password can't break the connection string (no ; or =)
alphabet = string.ascii_letters + string.digits
db_pwd = "".join(secrets.choice(alphabet) for _ in range(32))
passphrase = "".join(secrets.choice(alphabet) for _ in range(32))
signing_key = base64.b64encode(secrets.token_bytes(32)).decode()  # 256-bit key

# The DB user exists for both 'localhost' and '%' (any host); keep both in sync
for host in ("localhost", "%"):
    result = mysql("root", f"SET PASSWORD FOR '{DB_USER}'@'{host}' = PASSWORD('{db_pwd}');")
    if result.returncode != 0:
        sys.exit(f"Could not set the DB password for @{host} (is XAMPP MySQL running?): {result.stderr.strip()[:200]}")
if mysql(DB_USER, f"USE `{DB_NAME}`; SELECT 1;", db_pwd).returncode != 0:
    sys.exit("Password set, but logging in with it failed - check MariaDB.")
print("1/2 new DB password set and verified")

connection_string = f"Server=localhost;Port=3306;Database={DB_NAME};Uid={DB_USER};Pwd={db_pwd};"
save(SRC / "TastyBites.HttpApi.Host" / "appsettings.secrets.json", {
    "ConnectionStrings": {"Default": connection_string},
    "StringEncryption": {"DefaultPassPhrase": passphrase},
    "OpenIddict": {"SigningKey": signing_key},
})
save(SRC / "TastyBites.DbMigrator" / "appsettings.secrets.json", {"ConnectionStrings": {"Default": connection_string}})
print("2/2 appsettings.secrets.json written for HttpApi.Host and DbMigrator")
print("Done. Restart the backend and log in again.")
