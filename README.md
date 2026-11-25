<p align="center">
  <a href="https://www.sobistech.com">
    <img src="https://raw.githubusercontent.com/iventhosting/Sobis.UI/main/assets/banner.svg" alt="Sobis.UI Banner" width="100%" />
  </a>
</p>

<p align="center">
  <a href="https://www.sobistech.com"><img src="https://img.shields.io/badge/Live%20Demo-sobistech.com-F8A045?style=for-the-badge&logo=blazor&logoColor=white" alt="Live Demo" /></a>
  <a href="https://www.nuget.org/packages/Sobis.UI"><img src="https://img.shields.io/nuget/v/Sobis.UI?style=for-the-badge&logo=nuget&color=004880" alt="NuGet" /></a>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10" />
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-34d399?style=for-the-badge" alt="MIT License" /></a>
</p>

---

**Sobis.UI** is a standalone, enterprise-grade **.NET 10 Blazor UI Component Library** crafted by **Sobis Bilgi Teknolojileri Sistemleri** (`www.sobistech.com`). Designed for data-dense business applications, it eliminates raw `<div>` clutter and Bootstrap dependencies with **48 composable, strongly-typed Razor components** powered by pure `--sbs-*` CSS design tokens.

### ✨ Highlights

| Capability | What You Get |
|---|---|
| **Zero-Div Layout System** | Build complete CRUD screens with `SbsPage`, `SbsToolbar`, `SbsStack`, `SbsGridLayout`, `SbsFormLayout`, `SbsFormGroup`, `SbsField`, and `SbsCard` — no raw `<div>` or Bootstrap utility soup. |
| **Zero Bootstrap Dependency** | 100% pure CSS with `--sbs-*` design tokens, compact enterprise **Dark** default and crisp **Light** (`[data-theme="light"]`) mode. |
| **Dual-Mode `SbsGrid`** | Supports both declarative `<SbsGridColumn ... />` markup and imperative `Columns="_columns"` lists, client/server sorting & paging, `tfoot` aggregates (`Count`, `Sum`, `Average`, `Min`, `Max`), row actions, and Excel/PDF export. |
| **Turkish-Aware Search** | Built-in `İ/ı/Ş/ş/Ğ/ğ/Ö/ö/Ü/ü/Ç/ç` case-insensitive filtering across `SbsGrid`, `SbsComboBox`, `SbsMultiSelect`, and `SbsTagBox`. |
| **Enterprise Form Context** | Cascade `Disabled`, `ReadOnly`, `LabelPosition`, `CascadedRequiredFields`, `CascadedEditableFields`, and Blazor `EditContext` seamlessly via `SbsInputBase<T>`. |
| **Pluggable Auth & i18n** | Clean host abstractions (`ISbsAuthorizationProvider`, `ISbsLocalizer`) with zero domain coupling — works out of the box in both **Blazor Server** and **Blazor WebAssembly**. |

---

## 🚀 Quick Start

### 1. Install Package

```bash
dotnet add package Sobis.UI
```

### 2. Register Services (`Program.cs`)

```csharp
using Sobis.UI;

// Optional: plug in your own claim authorization & localization providers
builder.Services.AddScoped<ISbsAuthorizationProvider, MyAuthProvider>();
builder.Services.AddScoped<ISbsLocalizer, MyLocalizer>();

builder.Services.AddSbsUI(options =>
{
    options.DefaultToastPosition = SbsToastPosition.TopRight;
    options.DefaultToastShowCloseButton = true;
    options.DefaultTheme = "dark"; // "dark" or "light"
});
```

### 3. Include Static Assets (`App.razor` or `wwwroot/index.html`)

```html
<!-- Inside <head> -->
<link rel="stylesheet" href="_content/Sobis.UI/css/sobis-ui.css" />
<link rel="stylesheet" href="YourApp.styles.css" />

<!-- Before </body> -->
<script src="_content/Sobis.UI/js/sobis-ui.js"></script>
```

### 4. Import Namespace & Add Global Hosts

```razor
@* _Imports.razor *@
@using Sobis.UI

@* MainLayout.razor *@
<SbsToastContainer />
<SbsConfirmDialog />
<SbsDialogProvider />
```

### 5. Build a Zero-Div CRUD Screen

```razor
<SbsPage Busy="LocalBusy" RequiredFields="RequiredFields">
    <SbsToolbar>
        <SbsButton Text="Search" Icon="fa fa-search" Variant="primary" OnClick="OnSearchAsync" />
        <SbsToolbarDivider />
        <SbsButton Text="New" Icon="fa fa-file" Variant="ghost" ClaimCode="Unit.Create" OnClick="OnNewAsync" />
        <SbsButton Text="Save" Icon="fa fa-save" Variant="save" ClaimCode="Unit.Create,Unit.Update" OnClick="OnSaveAsync" />
    </SbsToolbar>

    <SbsGridLayout Columns="1fr 2fr" Gap="SbsSpacing.Sm">
        <SbsFormGroup Caption="Search Criteria" Icon="fa fa-search">
            <SbsField Label="Name">
                <SbsTextBox @bind-Value="Search.Filter" Immediate="true" />
            </SbsField>
        </SbsFormGroup>

        <SbsFormGroup Caption="Unit Details" Icon="fa fa-list" Disabled="@(!FormEnabled)">
            <SbsField Label="Unit Name" Required="true">
                <SbsTextBox @bind-Value="Model.Name" />
            </SbsField>
        </SbsFormGroup>
    </SbsGridLayout>

    <SbsGrid TItem="UnitDto" Data="@Rows" FillHeight="true" PageSize="50"
             ShowEditAction="true" ShowDeleteAction="true"
             OnEditClick="OnEditAsync" OnDeleteClick="OnDeleteAsync">
        <SbsGridColumn TItem="UnitDto" Field="@nameof(UnitDto.Code)" Caption="Code" Width="110px" />
        <SbsGridColumn TItem="UnitDto" Field="@nameof(UnitDto.Name)" Caption="Unit Name" />
    </SbsGrid>
</SbsPage>
```

---

## 📦 Component Catalog (48 Components)

| Category | Components |
|---|---|
| **Layout (12)** | `SbsPage`, `SbsToolbar`, `SbsToolbarDivider`, `SbsSpacer`, `SbsStack`, `SbsGridLayout`, `SbsGridItem`, `SbsFormLayout`, `SbsFormGroup`, `SbsField`, `SbsCard`, `SbsDivider` |
| **Inputs (12)** | `SbsTextBox`, `SbsTextArea`, `SbsNumberBox`, `SbsPhoneBox`, `SbsDateBox`, `SbsComboBox`, `SbsMultiSelect`, `SbsTagBox`, `SbsCheckBox`, `SbsSwitch`, `SbsRadioGroup`, `SbsFileUpload` |
| **Data & Actions (3)** | `SbsGrid`, `SbsGridColumn`, `SbsButton` |
| **Navigation (5)** | `SbsTabs`, `SbsTabPanel`, `SbsAccordion`, `SbsExpansionPanel`, `SbsDrawer` |
| **Overlays (6)** | `SbsModal`, `SbsConfirmDialog`, `SbsDialogProvider`, `SbsContextMenu`, `SbsContextMenuItem`, `SbsContextMenuDivider` |
| **Display & Feedback (10)** | `SbsText`, `SbsAlert`, `SbsBadge`, `SbsChip`, `SbsIcon`, `SbsTooltip`, `SbsEmptyState`, `SbsProgress`, `SbsSkeleton`, `SbsLoading`, `SbsToastContainer` |

---

## 🎨 Theming (Dark & Light)

Switch themes at runtime with zero page reload:

```js
window.sobisUI.theme.set('light'); // or 'dark'
window.sobisUI.theme.toggle();
```

Customize brand colors via CSS variables:

```css
:root {
    --sbs-accent: #F8A045;
    --sbs-accent-2: #e8922d;
}
```

---

## 🌐 Live Demo & Documentation

Explore all components interactively with live source code & API reference at **[https://www.sobistech.com](https://www.sobistech.com)** (built with `src/Sobis.UI.Docs` Blazor WebAssembly).

## 🧪 Building & Testing Locally

```bash
dotnet build Sobis.UI.slnx -c Release
dotnet run --project tests/Sobis.UI.Tests/Sobis.UI.Tests.csproj -c Release
dotnet pack src/Sobis.UI/Sobis.UI.csproj -c Release -o artifacts/nuget
```

## 📄 License

Licensed under the [MIT License](LICENSE). Built by **Sobis Bilgi Teknolojileri Sistemleri**.
