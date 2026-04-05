# 04-api-modernization: Fix source-incompatible APIs and behavioral changes

Address the remaining 61 source-incompatible API usages and 38 behavioral change warnings:

- **`System.Media.SystemSounds` / `SystemSound.Play`** (32 usages) — these types require the `System.Windows.Extensions` NuGet package on .NET Core / .NET 5+. Add the package reference and verify the calls compile and behave correctly.
- **`System.Uri` behavioral changes** (23 usages of constructor + 6 of string overload) — `System.Uri` parsing was tightened in modern .NET. Review URI construction sites for stricter validation behaviour and add handling where needed.
- Any remaining source-incompatible APIs surfaced during the build after task `02`.

**Done when**: Solution builds with 0 errors and 0 warnings related to the above APIs; `System.Windows.Extensions` added if `SystemSounds` is retained; URI-related code reviewed and hardened.
