# 02-framework-and-packages: Update target framework and NuGet packages

Update `SimpleImageResizer.csproj` to target `net10.0-windows` (from `net8.0-windows7.0`). This single change resolves the 840 binary-incompatible WPF API flags — WPF is fully supported on .NET 10 Windows targets. Also update `Microsoft.Data.Sqlite` from `7.0.2` to `10.0.5` (the only package requiring an upgrade).

**Done when**: Project targets `net10.0-windows`; `Microsoft.Data.Sqlite` references version `10.0.5`; `dotnet restore` succeeds with no errors.
