<#
.SYNOPSIS
    Create a release tag with automatic version update in Huoreimu.json.
.DESCRIPTION
    Updates the version field in Huoreimu.json, commits, tags, and pushes.
    Supports -Force to skip dirty workspace check and -WhatIf for dry run.
.PARAMETER Version
    Version number without leading 'v', e.g. 1.0.0 or 1.0.0b.
.PARAMETER Description
    Optional tag message, used as Steam changelog and GitHub Release body.
.PARAMETER Force
    Skip dirty workspace check (include uncommitted changes in version commit).
.PARAMETER WhatIf
    Show what would happen without making any changes.
.EXAMPLE
    .\release.ps1 1.0.0
    .\release.ps1 1.0.0b -Force
    .\release.ps1 1.0.0 "新卡牌、平衡调整"
    .\release.ps1 1.0.0 "新卡牌、平衡調整" -WhatIf
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Version,

    [Parameter(Position=1)]
    [string]$Description,

    [switch]$Force,
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

# Auto-cd to repo root
$root = git rev-parse --show-toplevel 2>$null
if (-not $root) {
    Write-Host "Error: Not inside a git repository."
    exit 1
}
Set-Location $root

# Strip leading 'v'
$Version = $Version.TrimStart('v')

# Validate format
if ($Version -notmatch '^[0-9]+(\.[0-9]+)*b?$') {
    Write-Host "Error: Invalid version format: $Version"
    Write-Host "Expected: X.Y.Z or X.Y.Zb (e.g. 1.0.0, 1.0.0b)"
    exit 1
}

$Tag = "v$Version"
Write-Host "=== Preparing release $Tag ==="

# --- Check workspace ---
$dirty = $false
$null = git diff --quiet HEAD -- Huoreimu.json 2>$null
if ($LASTEXITCODE -ne 0) { $dirty = $true }

if ($dirty) {
    if ($Force) {
        Write-Host "Warning: Workspace is dirty. Proceeding due to -Force."
    } else {
        try {
            $response = Read-Host -Prompt "Warning: Workspace has uncommitted changes. Continue? [y/N]"
        } catch {
            Write-Host "Error: Non-interactive terminal. Use -Force or clean workspace."
            exit 1
        }
        if ($response -notmatch '^[yY]') {
            Write-Host "Aborted."
            exit 1
        }
    }
}

# Save original HEAD
$originalHead = git rev-parse HEAD
Write-Host "  Original HEAD: $($originalHead.Substring(0,8))"

# --- Update Huoreimu.json ---
if ($WhatIf) {
    Write-Host "  [WhatIf] Would update Huoreimu.json version to $Version"
    Write-Host "  [WhatIf] Would commit 'chore: update version to $Version'"
    if ($Description) {
        Write-Host "  [WhatIf] Would create annotated tag $Tag with message: $Description"
    } else {
        Write-Host "  [WhatIf] Would create lightweight tag $Tag"
    }
    Write-Host "  [WhatIf] Would push tag $Tag"
    Write-Host ""
    Write-Host "=== WhatIf: No changes made ==="
    return
}

$content = [System.IO.File]::ReadAllText('Huoreimu.json', [System.Text.Encoding]::UTF8)
$content = $content -replace '"version":\s*"[^"]*"', ('"version": "' + $Version + '"')
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText('Huoreimu.json', $content, $utf8NoBom)

# Check if actually changed
$null = git diff --quiet HEAD 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "No changes (version already $Version). Nothing to commit."
    exit 0
}

Write-Host "  Updated Huoreimu.json version to $Version"

# Commit
git add Huoreimu.json
git commit -m "chore: update version to $Version"
$commitSha = git rev-parse HEAD
Write-Host "  Committed: $($commitSha.Substring(0,8)) - chore: update version to $Version"

# Tag (annotated with description if provided)
if ($Description) {
    git tag -a $Tag -m $Description
    Write-Host "  Created annotated tag: $Tag"
    Write-Host "  Tag message: $Description"
} else {
    git tag $Tag
    Write-Host "  Created lightweight tag: $Tag"
}

# Push
Write-Host "  Pushing tag $Tag..."
try {
    git push origin $Tag
} catch {
    Write-Host "  Push failed. Rolling back..."
    git tag -d $Tag 2>$null
    git reset --hard $originalHead 2>$null
    Write-Host "  Deleted tag $Tag, reset to $($originalHead.Substring(0,8))"
    Write-Host ""
    Write-Host "=== Push failed. Local changes reverted. ==="
    exit 1
}

$branch = git rev-parse --abbrev-ref HEAD
Write-Host ""
Write-Host "=== Success: tag $Tag pushed ==="
Write-Host "Don't forget to also push the version commit: git push origin $branch"
