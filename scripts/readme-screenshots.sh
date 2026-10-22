#!/usr/bin/env bash
# Retakes the public-site screenshots used in README.md, in light AND dark mode,
# from the live Netlify demo. Uses your installed Google Chrome (no browser download).
#   ./scripts/readme-screenshots.sh
set -euo pipefail

SITE="${SITE:-https://tastybitesproject.netlify.app}"
OUT="$(cd "$(dirname "$0")/.." && pwd)/docs/screenshots"
PW="npx -y playwright@1.47.2 screenshot --channel chrome --wait-for-timeout 2500"

export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && . "$NVM_DIR/nvm.sh" && nvm use 18 >/dev/null

for scheme in light dark; do
  suffix=""; [ "$scheme" = dark ] && suffix="-dark"
  echo "▶ $scheme"
  $PW --color-scheme "$scheme" --viewport-size "1600, 1000" "$SITE/" "$OUT/public-home$suffix.jpg"
  $PW --color-scheme "$scheme" --viewport-size "1600, 1000" --full-page "$SITE/recipes/11" "$OUT/public-recipe$suffix.jpg"
  $PW --color-scheme "$scheme" --device "Pixel 7" "$SITE/recipes/11" "$OUT/public-recipe-mobile$suffix.jpg"
done
echo "✔ Screenshots written to docs/screenshots/"
