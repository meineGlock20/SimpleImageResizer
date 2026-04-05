# 03-legacy-config-migration: Migrate legacy configuration system

The app uses `System.Configuration.ApplicationSettingsBase` (26 flagged usages across `Save`, `Item` indexer) and `app.config`-based settings — a legacy XML configuration system not natively supported in modern .NET. Migrate to `Microsoft.Extensions.Configuration` using `appsettings.json`, or use the `System.Configuration.ConfigurationManager` NuGet package as a compatibility bridge if full migration is disproportionate to the app's size.

Affected: 28 issues across the configuration subsystem.

**Done when**: All `ApplicationSettingsBase` usages compile cleanly; settings are read from a supported configuration source; `app.config` is no longer required for runtime configuration.
