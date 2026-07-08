@echo off
setlocal enabledelayedexpansion

REM release.bat - Create a release tag with automatic version update in Huoreimu.json
REM Usage: release.bat <version> [--force]
REM   version: e.g. 1.0.0 or 1.0.0b (without leading 'v')
REM   --force: skip dirty workspace check
REM
REM Example:
REM   release.bat 1.0.0
REM   release.bat 1.0.0b --force

set VERSION=%~1
if "%VERSION%"=="" (
  echo Usage: %~nx0 ^<version^> [--force]
  echo   version: e.g. 1.0.0 or 1.0.0b
  exit /b 1
)

REM Strip leading 'v'
if /I "%VERSION:~0,1%"=="v" set VERSION=%VERSION:~1%
set TAG=v%VERSION%

echo === Preparing release %TAG% ===

REM Validate format
echo %VERSION% | findstr /R "^[0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*b*$" >nul 2>nul
if errorlevel 1 (
  echo Error: Invalid version format: %VERSION%
  echo Expected: X.Y.Z or X.Y.Zb (e.g. 1.0.0, 1.0.0b)
  exit /b 1
)

REM Check workspace
git diff --quiet HEAD 2>nul
if errorlevel 1 (
  if /I "%~2"=="--force" (
    echo Warning: Workspace is dirty. Proceeding due to --force.
  ) else (
    set /p "CONFIRM=Warning: Workspace has uncommitted changes. Continue? [y/N] "
    if /I not "!CONFIRM!"=="y" (
      echo Aborted.
      exit /b 1
    )
  )
)

REM Save original HEAD
for /f %%i in ('git rev-parse HEAD') do set ORIGINAL_HEAD=%%i
echo   Original HEAD: %ORIGINAL_HEAD:~0,8%

REM --- Update Huoreimu.json via PowerShell (no jq dependency) ---
> "%TEMP%\huoreimu_release.ps1" (
echo param([string]$ver^)
echo $c = Get-Content 'Huoreimu.json' -Raw
echo $c = $c -replace '"version":\s*"[^"]*"', ('"version": "' + $ver + '"')
echo Set-Content 'Huoreimu.json' -Value $c -NoNewline -Encoding utf8
)
powershell -NoProfile -ExecutionPolicy Bypass -File "%TEMP%\huoreimu_release.ps1" -ver "%VERSION%"
if errorlevel 1 (
  echo Error: Failed to update Huoreimu.json.
  del "%TEMP%\huoreimu_release.ps1" 2>nul
  exit /b 1
)
del "%TEMP%\huoreimu_release.ps1" 2>nul

REM Check if actually changed
git diff --quiet HEAD 2>nul
if not errorlevel 1 (
  echo No changes (version already %VERSION%). Nothing to commit.
  exit /b 0
)

echo   Updated Huoreimu.json version to %VERSION%

REM Commit
git add Huoreimu.json
if errorlevel 1 (
  echo Error: git add failed.
  exit /b 1
)

git commit -m "chore: update version to %VERSION%"
if errorlevel 1 (
  echo Error: git commit failed.
  exit /b 1
)
for /f %%i in ('git rev-parse HEAD') do set COMMIT_SHA=%%i
echo   Committed: %COMMIT_SHA:~0,8% - chore: update version to %VERSION%

REM Tag
git tag "%TAG%"
if errorlevel 1 (
  echo Error: git tag failed.
  exit /b 1
)
echo   Created tag: %TAG%

REM Push
echo   Pushing tag %TAG%...
git push origin "%TAG%"
if errorlevel 1 (
  echo   Push failed. Rolling back...
  git tag -d "%TAG%" 2>nul
  git reset --hard "%ORIGINAL_HEAD%" 2>nul
  echo   Deleted tag %TAG%, reset to %ORIGINAL_HEAD:~0,8%
  echo.
  echo === Push failed. Local changes reverted. ===
  exit /b 1
)

for /f %%i in ('git rev-parse --abbrev-ref HEAD') do set BRANCH=%%i
echo.
echo === Success: tag %TAG% pushed ===
echo Don't forget to also push the version commit: git push origin %BRANCH%
