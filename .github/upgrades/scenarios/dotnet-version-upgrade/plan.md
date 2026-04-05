# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade SimpleImageResizer WPF application from `net8.0-windows7.0` to `net10.0-windows` and modernize the codebase.
**Scope**: 1 project, ~4,400 LOC, 38 code files — medium complexity, distinct modernization work areas.

### Selected Strategy
**All-At-Once** — Single project upgraded in one coordinated operation across focused concern areas.
**Rationale**: 1 project, SDK-style, clear dependency structure, medium complexity with distinct work areas warranting per-concern task tracking.

---

## Tasks

### 01-prerequisites: Validate .NET 10 SDK and environment

Verify the .NET 10 SDK is installed and that `global.json` (if present) is compatible with the target framework. This unblocks all subsequent tasks and avoids mid-upgrade surprises.

**Done when**: .NET 10 SDK is confirmed installed; any `global.json` references are compatible with `net10.0-windows`.

---

### 02-framework-and-packages: Update target framework and NuGet packages

Update `SimpleImageResizer.csproj` to target `net10.0-windows` (from `net8.0-windows7.0`). This single change resolves the 840 binary-incompatible WPF API flags — WPF is fully supported on .NET 10 Windows targets. Also update `Microsoft.Data.Sqlite` from `7.0.2` to `10.0.5` (the only package requiring an upgrade).

**Done when**: Project targets `net10.0-windows`; `Microsoft.Data.Sqlite` references version `10.0.5`; `dotnet restore` succeeds with no errors.

---

### 03-legacy-config-migration: Migrate legacy configuration system

The app uses `System.Configuration.ApplicationSettingsBase` (26 flagged usages across `Save`, `Item` indexer) and `app.config`-based settings — a legacy XML configuration system not natively supported in modern .NET. Migrate to `Microsoft.Extensions.Configuration` using `appsettings.json`, or use the `System.Configuration.ConfigurationManager` NuGet package as a compatibility bridge if full migration is disproportionate to the app's size.

Affected: 28 issues across the configuration subsystem.

**Done when**: All `ApplicationSettingsBase` usages compile cleanly; settings are read from a supported configuration source; `app.config` is no longer required for runtime configuration.

---

### 04-api-modernization: Fix source-incompatible APIs and behavioral changes

Address the remaining 61 source-incompatible API usages and 38 behavioral change warnings:

- **`System.Media.SystemSounds` / `SystemSound.Play`** (32 usages) — these types require the `System.Windows.Extensions` NuGet package on .NET Core / .NET 5+. Add the package reference and verify the calls compile and behave correctly.
- **`System.Uri` behavioral changes** (23 usages of constructor + 6 of string overload) — `System.Uri` parsing was tightened in modern .NET. Review URI construction sites for stricter validation behaviour and add handling where needed.
- Any remaining source-incompatible APIs surfaced during the build after task `02`.

**Done when**: Solution builds with 0 errors and 0 warnings related to the above APIs; `System.Windows.Extensions` added if `SystemSounds` is retained; URI-related code reviewed and hardened.

---

### 05-validation: Build and test validation

Perform a clean solution build and run all available tests to confirm the upgrade did not introduce regressions. Verify the WPF application launches and the core image-resizing workflow functions as expected.

**Done when**: Solution builds with 0 errors; all tests pass; application launches successfully on .NET 10.
