#!/bin/bash
# release.sh - Create a release tag with automatic version update in Huoreimu.json
# Usage: ./release.sh <version> [--force]
#   version: e.g. 1.0.0 or 1.0.0b (without leading 'v')
#   --force: skip dirty workspace check
#
# Example:
#   ./release.sh 1.0.0        # creates tag v1.0.0 on master
#   ./release.sh 1.0.0b       # creates tag v1.0.0b on beta_version

set -euo pipefail

FORCE=false
VERSION=""

for arg in "$@"; do
  case "$arg" in
    --force) FORCE=true ;;
    --help|-h)
      echo "Usage: $0 <version> [--force]"
      echo "  version: e.g. 1.0.0 or 1.0.0b"
      echo "  --force: skip dirty workspace check"
      exit 0
      ;;
    *)
      if [ -z "$VERSION" ]; then
        VERSION="$arg"
      else
        echo "Error: Unexpected argument: $arg"
        exit 1
      fi
      ;;
  esac
done

if [ -z "$VERSION" ]; then
  echo "Error: Version is required. Usage: $0 <version> [--force]"
  exit 1
fi

VERSION="${VERSION#v}"

if ! echo "$VERSION" | grep -qE '^[0-9]+(\.[0-9]+)*b?$'; then
  echo "Error: Invalid version format: $VERSION"
  echo "Expected: X.Y.Z or X.Y.Zb (e.g. 1.0.0, 1.0.0b)"
  exit 1
fi

TAG="v${VERSION}"
echo "=== Preparing release $TAG ==="

# Check workspace
if ! git diff --quiet HEAD; then
  if [ "$FORCE" = true ]; then
    echo "Warning: Workspace is dirty. Proceeding due to --force."
  elif [ -t 0 ]; then
    read -r -p "Warning: Workspace has uncommitted changes. Continue? [y/N] " response
    case "$response" in [yY]|[yY][eE][sS]) ;; *)
      echo "Aborted."
      exit 1 ;;
    esac
  else
    echo "Error: Workspace is dirty (non-interactive). Use --force or clean workspace."
    exit 1
  fi
fi

# Save original HEAD
ORIGINAL_HEAD=$(git rev-parse HEAD)

# Update Huoreimu.json
if ! jq --arg v "$VERSION" '.version = $v' Huoreimu.json > Huoreimu.json.tmp; then
  echo "Error: jq failed. Is jq installed? (apt install jq / brew install jq)"
  exit 1
fi
mv Huoreimu.json.tmp Huoreimu.json

if git diff --quiet HEAD; then
  echo "No changes (version already $VERSION). Nothing to commit."
  exit 0
fi

echo "  Updated Huoreimu.json version to $VERSION"

# Commit
git add Huoreimu.json
git commit -m "chore: update version to $VERSION"
COMMIT_SHA=$(git rev-parse HEAD)
echo "  Committed: ${COMMIT_SHA:0:8} - chore: update version to $VERSION"

# Tag
git tag "$TAG"
echo "  Created tag: $TAG"

# Push
echo "  Pushing tag $TAG..."
if git push origin "$TAG"; then
  echo ""
  echo "=== Success: tag $TAG pushed ==="
  echo "Don't forget to also push the version commit: git push origin $(git rev-parse --abbrev-ref HEAD)"
else
  PUSH_EXIT=$?
  echo "  Push failed. Rolling back..."
  git tag -d "$TAG" 2>/dev/null || true
  git reset --hard "$ORIGINAL_HEAD" 2>/dev/null
  echo "  Deleted tag $TAG, reset to ${ORIGINAL_HEAD:0:8}"
  echo ""
  echo "=== Push failed. Local changes reverted. ==="
  exit $PUSH_EXIT
fi
