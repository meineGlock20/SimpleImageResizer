# .NET Version Upgrade — SimpleImageResizer

## Strategy
**Selected**: All-at-Once — all upgrade work coordinated in a single operation across concern-based tasks.
**Rationale**: 1 project, SDK-style, no inter-project dependencies, medium complexity with distinct work areas.

### Execution Constraints
- All tasks operate on the single `SimpleImageResizer.csproj` — no tier ordering needed.
- Task `02-framework-and-packages` must complete before `03` and `04` (TFM change resolves WPF binary flags).
- Fix all compilation errors in a single bounded pass per task — no iterative retry loops.
- Validate `dotnet build` succeeds before marking any task complete.
- Final validation (`05`) runs after all code changes are done.

## Preferences
- **Flow Mode**: Guided
- **Commit Strategy**: After Each Task
- **Target Framework**: net10.0-windows
- **Source Branch**: main
- **Working Branch**: upgrade-to-NET10
