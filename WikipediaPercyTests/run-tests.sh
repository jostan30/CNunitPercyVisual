#!/bin/bash
# run-tests.sh — Run Wikipedia Percy tests with visual snapshots
# Usage: ./run-tests.sh
#
# This script:
#   1. Installs Percy CLI if not already present (via npm)
#   2. Sets the Percy token
#   3. Runs dotnet test wrapped with percy exec

set -e

PERCY_CLI="$HOME/.browserstack/percy"
PERCY_TOKEN_VALUE="app_13815b667557cc48....."

# ── Step 1: Ensure Percy CLI is available ────────────────────────────────────
if [ ! -f "$PERCY_CLI" ]; then
  echo "Percy CLI not found at $PERCY_CLI"
  echo "Installing via npm..."

  if ! command -v npm &>/dev/null; then
    echo "ERROR: npm is not installed. Please install Node.js from https://nodejs.org and re-run this script."
    exit 1
  fi

  npm install --global @percy/cli
  # After npm install, BrowserStack SDK also places its own copy on first build.
  # Fall back to the npm-installed percy if the BrowserStack path still doesn't exist.
  if [ ! -f "$PERCY_CLI" ]; then
    PERCY_CLI=$(which percy 2>/dev/null || true)
    if [ -z "$PERCY_CLI" ]; then
      echo "ERROR: Percy CLI installation failed. Please run 'npm install -g @percy/cli' manually."
      exit 1
    fi
  fi

  echo "Percy CLI installed at: $PERCY_CLI"
fi

# ── Step 2: Ensure project is built ─────────────────────────────────────────
echo "Building project..."
dotnet build --nologo -v quiet

# ── Step 3: Set Percy token and run tests ────────────────────────────────────
export PERCY_TOKEN="$PERCY_TOKEN_VALUE"
echo "Starting tests with Percy visual capture..."
"$PERCY_CLI" exec -- dotnet test "$@"