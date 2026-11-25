# Changelog

## [1.0.0] - 2026-11-25
### Added
- Standalone `.NET 10` Razor Class Library (`Sobis.UI`) with zero Bootstrap dependency.
- Core architecture: `SbsComponentBase`, `SbsInputBase<TValue>`, `CssBuilder`, `StyleBuilder`, `SbsFormContext`, strongly-typed enums, and DI service abstractions (`ISbsAuthorizationProvider`, `ISbsLocalizer`, `ISbsToastService`, `ISbsConfirmService`, `ISbsDialogService`).
- 48 Razor components across Layout (12), Inputs (12), Data & Actions (3), Navigation (5), Overlays (6), and Display & Feedback (10).
- Dual-mode `SbsGrid` (declarative `<SbsGridColumn>` + imperative `Columns` list, Turkish-aware search, sorting, paging, footer aggregates, CSV/HTML print export).
- Compact enterprise Dark (default) and Light (`[data-theme="light"]`) CSS token system (`--sbs-*`).
- Standalone Blazor WebAssembly interactive documentation portal (`src/Sobis.UI.Docs`, hosted at https://www.sobistech.com).
- Automated verification suite (`tests/Sobis.UI.Tests`, 52 checks).
