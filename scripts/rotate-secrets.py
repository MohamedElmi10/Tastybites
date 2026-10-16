#!/usr/bin/env python3
"""One-time: move secrets out of the tracked appsettings.json files and rotate them.

Run from the repo root:  python3 scripts/rotate-secrets.py
Never prints a secret value.

1. Reads the current DB password from HttpApi.Host/appsettings.json
2. Generates a new DB password, StringEncryption passphrase and OpenIddict signing key
3. Changes the DB user's password in MariaDB (SET PASSWORD on its own account)
4. Writes appsettings.secrets.json for HttpApi.Host and DbMigrator (gitignored, chmod 600)
5. Removes the secret values from the tracked appsettings.json files
"""
import base64
import json
import os
import re
import secrets
import string
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent / "TastyBites" / "aspnet-core"
HOST = ROOT / "src" / "TastyBites.HttpApi.Host"
MIGRATOR = ROOT / "src" / "TastyBites.DbMigrator"
CONSOLE_APP = ROOT / "test" / "TastyBites.HttpApi.Client.ConsoleTestApp"
MYSQL = "/Applications/XAMPP/xamppfiles/bin/mysql"


def load(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def save(path, data, private=False):
    path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    if private:
        os.chmod(path, 0o600)  # only your user can read it


def run_sql(user, pwd, sql):
    return subprocess.run(
        [MYSQL, "-u", user, "tasty-bites", "-N", "-e", sql],
        env={**os.environ, "MYSQL_PWD": pwd},
        capture_output=True,
        text=True,
    )


host_settings = load(HOST / "appsettings.json")
if "ConnectionStrings" not in host_settings:
    sys.exit("Already done: HttpApi.Host/appsettings.json has no connection string any more.")

old_cs = host_settings["ConnectionStrings"]["Default"]
user = re.search(r"Uid=([^;]*)", old_cs).group(1)
old_pwd = re.search(r"Pwd=([^;]*)", old_cs).group(1)

# Letters and digits only, so the password can't break the connection string (no ; or =)
alphabet = string.ascii_letters + string.digits
new_pwd = "".join(secrets.choice(alphabet) for _ in range(32))
passphrase = "".join(secrets.choice(alphabet) for _ in range(32))
signing_key = base64.b64encode(secrets.token_bytes(32)).decode()  # 256-bit key

result = run_sql(user, old_pwd, f"SET PASSWORD = PASSWORD('{new_pwd}');")
if result.returncode != 0:
    sys.exit("Could not change the DB password (is XAMPP MySQL running?): " + result.stderr.strip()[:200])
if run_sql(user, new_pwd, "SELECT 1;").returncode != 0:
    sys.exit("Password changed but login with the new one failed - check MariaDB before continuing.")
print("1/3 DB password rotated and verified")

new_cs = re.sub(r"Pwd=[^;]*", "Pwd=" + new_pwd, old_cs)
save(HOST / "appsettings.secrets.json", {
    "ConnectionStrings": {"Default": new_cs},
    "StringEncryption": {"DefaultPassPhrase": passphrase},
    "OpenIddict": {"SigningKey": signing_key},
}, private=True)
save(MIGRATOR / "appsettings.secrets.json", {"ConnectionStrings": {"Default": new_cs}}, private=True)
print("2/3 appsettings.secrets.json written for HttpApi.Host and DbMigrator")

host_settings.pop("ConnectionStrings", None)
host_settings.pop("StringEncryption", None)
# Visitors don't sign up; only the admin logs in
host_settings["Settings"] = {"Abp.Account.IsSelfRegistrationEnabled": "false"}
save(HOST / "appsettings.json", host_settings)

migrator_settings = load(MIGRATOR / "appsettings.json")
migrator_settings.pop("ConnectionStrings", None)
apps = migrator_settings.get("OpenIddict", {}).get("Applications", {})
apps.pop("TastyBites_Web", None)               # template leftovers: these apps don't exist
apps.pop("TastyBites_BlazorServerTiered", None)
save(MIGRATOR / "appsettings.json", migrator_settings)

console_settings = load(CONSOLE_APP / "appsettings.json")
console_settings["IdentityClients"]["Default"]["UserPassword"] = ""
save(CONSOLE_APP / "appsettings.json", console_settings)
print("3/3 secret values removed from the tracked appsettings.json files")
print("Done. Restart the backend and log in again (old login tokens are now invalid).")
