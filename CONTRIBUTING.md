# Contributing to Sobis.UI

Contributions, bug reports, and feature requests are welcome!

1. Fork the repository and create a feature branch from `main`.
2. Keep `src/Sobis.UI` 100% free of Bootstrap utility classes and external UI framework dependencies.
3. Style components using `--sbs-*` design tokens and `.razor.css` isolation, verifying both Dark and Light (`[data-theme="light"]`) themes.
4. Run `dotnet build Sobis.UI.slnx -c Release` and `dotnet run --project tests/Sobis.UI.Tests/Sobis.UI.Tests.csproj -c Release` before opening a Pull Request.
