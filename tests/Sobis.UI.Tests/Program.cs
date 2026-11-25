#pragma warning disable BL0005
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Sobis.UI;

int passed = 0;
void Check(bool ok, string message)
{
    if (!ok) throw new Exception("FAIL: " + message);
    passed++;
    Console.WriteLine("PASS " + message);
}

// =========================================================================
// 1. CssBuilder
// =========================================================================
var cssEmpty = CssBuilder.Empty().Build();
Check(cssEmpty is null, "CssBuilder.Empty().Build() returns null");

var cssBuilt = CssBuilder.Default("sbs-btn")
    .AddClass("sbs-btn-primary", true)
    .AddClass("sbs-btn-disabled", false)
    .AddClass("custom-a", () => true)
    .AddClass("custom-b", () => false)
    .AddClass(() => "from-func", true)
    .AddClass("   ")
    .AddClass(CssBuilder.Default("nested-cls"))
    .AddClassFromAttributes(new Dictionary<string, object> { ["CLASS"] = "attr-cls", ["id"] = "x" })
    .ToString();
Check(cssBuilt == "sbs-btn sbs-btn-primary custom-a from-func nested-cls attr-cls",
    $"CssBuilder composes classes cleanly (got '{cssBuilt}')");

// =========================================================================
// 2. StyleBuilder
// =========================================================================
Check(StyleBuilder.Empty().Build() is null, "StyleBuilder.Empty().Build() returns null");

var styleBuilt = StyleBuilder.Default("gap", "8px")
    .AddStyle("color", "red", true)
    .AddStyle("display", "none", false)
    .AddStyle("width", () => "100px", true)
    .AddStyle("margin:4px;")
    .AddStyle(StyleBuilder.Default("padding", "6px"))
    .AddStyleFromAttributes(new Dictionary<string, object> { ["Style"] = "border:1px solid #000" })
    .Build();
Check(styleBuilt == "gap:8px; color:red; width:100px; margin:4px; padding:6px; border:1px solid #000;",
    $"StyleBuilder composes styles cleanly (got '{styleBuilt}')");

// =========================================================================
// 3. Enums: .ToCss(), .TryParse(), .ToCssVar(), .ToCssToken()
// =========================================================================
Check(SbsVariant.Primary.ToCss() == "primary" && SbsVariant.Outlined.ToCss() == "outlined" && SbsVariant.Save.ToCss() == "save",
    "SbsVariant.ToCss() maps variants");
Check(SbsVariantExtensions.TryParse("  danger ", out var parsedVariant) && parsedVariant == SbsVariant.Danger,
    "SbsVariantExtensions.TryParse parses case-insensitively with whitespace");
Check(!SbsVariantExtensions.TryParse("nonexistent", out var fallbackVariant) && fallbackVariant == SbsVariant.Primary,
    "SbsVariantExtensions.TryParse returns false for invalid value");

Check(SbsSize.Xs.ToCss() == "xs" && SbsSize.Xl.ToCss() == "xl", "SbsSize.ToCss() maps sizes");
Check(SbsSizeExtensions.TryParse("LG", out var parsedSize) && parsedSize == SbsSize.Lg,
    "SbsSizeExtensions.TryParse parses case-insensitively");
Check(!SbsSizeExtensions.TryParse(null, out var fallbackSize) && fallbackSize == SbsSize.Md,
    "SbsSizeExtensions.TryParse handles null with Md default");

Check(SbsModalSize.Xxl.ToCss() == "xxl" && SbsModalSize.Full.ToCss() == "full", "SbsModalSize.ToCss() maps modal sizes");
Check(SbsModalSizeExtensions.TryParse("xl", out var parsedModal) && parsedModal == SbsModalSize.Xl,
    "SbsModalSizeExtensions.TryParse parses modal sizes");

Check(SbsColor.Success.ToCss() == "success" && SbsColor.Muted.ToCss() == "muted", "SbsColor.ToCss() maps colors");
Check(SbsAlertSeverity.Warning.ToCss() == "warning" && SbsAlertSeverity.Danger.ToCss() == "danger", "SbsAlertSeverity.ToCss() maps severities");
Check(SbsDirection.ColumnReverse.ToCss() == "column-reverse" && SbsOrientation.Vertical.ToCss() == "vertical" && SbsPlacement.Right.ToCss() == "right",
    "SbsDirection/Orientation/Placement.ToCss() map layout tokens");
Check(SbsAlign.Start.ToCss() == "flex-start" && SbsTextAlign.Justify.ToCss() == "justify" && SbsJustify.Between.ToCss() == "space-between",
    "SbsAlign/TextAlign/Justify.ToCss() map flex/text alignment");
Check(SbsSpacing.None.ToCssVar() == "0" && SbsSpacing.Md.ToCssVar() == "var(--sbs-space-md)" && SbsSpacing.Lg.ToCssToken() == "lg",
    "SbsSpacing.ToCssVar()/ToCssToken() map spacing tokens");
Check(SbsTextVariant.H2.ToCss() == "h2" && SbsTextVariant.Overline.ToCss() == "overline", "SbsTextVariant.ToCss() maps typography variants");
Check(SbsTabsVariant.Pill.ToCss() == "pill" && SbsSkeletonVariant.Circle.ToCss() == "circle", "SbsTabsVariant & SbsSkeletonVariant.ToCss() map variants");

// =========================================================================
// 4. SbsInputBase<T>: property extraction, required/editable/form cascading, EditContext
// =========================================================================
var model = new SampleModel { Ad = "", Kod = "B01" };
var input = new TestStringInput
{
    Value = model.Ad,
    ValueExpression = () => model.Ad,
    CascadedRequiredFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Ad" },
    CascadedEditableFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Kod" }
};
Check(input.ExposedActualPropertyName == "Ad", "SbsInputBase extracts property name from ValueExpression");
Check(input.ExposedIsRequired, "SbsInputBase marks field required via CascadedRequiredFields");
Check(input.ExposedIsInvalid, "SbsInputBase marks required empty field invalid");
Check(input.ExposedEffectiveDisabled, "SbsInputBase disables field excluded from CascadedEditableFields");

input.CascadedEditableFields.Add("Ad");
Check(!input.ExposedEffectiveDisabled, "SbsInputBase enables field when included in CascadedEditableFields");

input.FormContext = new SbsFormContext { Disabled = true, ReadOnly = true };
Check(input.ExposedEffectiveDisabled && input.ExposedEffectiveReadOnly,
    "SbsInputBase inherits Disabled and ReadOnly from SbsFormContext");

input.FormContext = new SbsFormContext { Disabled = false, ReadOnly = false };
input.Value = "Personel";
Check(!input.ExposedIsInvalid, "SbsInputBase clears invalid state once required field has value");

input.PropertyName = "OverrideProp";
Check(input.ExposedActualPropertyName == "OverrideProp", "SbsInputBase prefers explicit PropertyName over ValueExpression");

// UnaryExpression (boxed / converted property) extraction
Expression<Func<object?>> boxedExpr = () => model.Sayi;
Check(TestStringInput.CallExtractPropertyName(boxedExpr) == nameof(SampleModel.Sayi),
    "SbsInputBase.ExtractPropertyName unwraps UnaryExpression");

// EditContext validation integration (without triggering unmounted InvokeAsync)
var editContext = new EditContext(model);
var validationMessages = new ValidationMessageStore(editContext);
var editInput = new TestStringInput
{
    Value = "Valid",
    ValueExpression = () => model.Ad,
    EditContext = editContext
};
editInput.InvokeOnParametersSet();
Check(!editInput.ExposedIsInvalid, "SbsInputBase with clean EditContext is valid");
validationMessages.Add(FieldIdentifier.Create(() => model.Ad), "Ad geçersiz");
Check(editInput.ExposedIsInvalid && editInput.ExposedEffectiveErrorText == "Ad geçersiz",
    "SbsInputBase surfaces EditContext validation messages");
editInput.Dispose();

// =========================================================================
// 5. SbsToastService
// =========================================================================
var localizer = new DefaultSbsLocalizer();
using var toastSvc = new SbsToastService(localizer, new SbsOptions
{
    DefaultToastPosition = SbsToastPosition.BottomRight,
    DefaultToastShowCloseButton = false
});
int toastChangedCount = 0;
SbsToastMessage? lastShown = null;
string? lastError = null;
toastSvc.OnToastsChanged += () => toastChangedCount++;
toastSvc.OnToastShown += m => lastShown = m;
toastSvc.OnErrorShown += err => lastError = err;

toastSvc.Success("Kaydedildi", durationMs: 0);
Check(toastSvc.Toasts.Count == 1
      && lastShown is { Type: SbsToastType.Success, Position: SbsToastPosition.BottomRight, ShowCloseButton: false }
      && !string.IsNullOrWhiteSpace(lastShown.Title)
      && toastChangedCount == 1,
    "SbsToastService.Success records toast with SbsOptions defaults and fires events");

toastSvc.Error("Hata oluştu", durationMs: 0, title: "Özel Başlık");
Check(toastSvc.Toasts.Count == 2 && lastError == "Hata oluştu" && toastSvc.Toasts[1].Title == "Özel Başlık",
    "SbsToastService.Error fires OnErrorShown and preserves custom title");

toastSvc.Warning("Uyarı", durationMs: 0);
toastSvc.Info("Bilgi", durationMs: 0);
Check(toastSvc.Toasts.Count == 4, "SbsToastService.Warning and Info append toasts");

var firstId = toastSvc.Toasts[0].Id;
toastSvc.Dismiss(firstId);
Check(toastSvc.Toasts.Count == 3 && toastSvc.Toasts.All(t => t.Id != firstId),
    "SbsToastService.Dismiss removes specific toast");

toastSvc.DismissAll();
Check(toastSvc.Toasts.Count == 0, "SbsToastService.DismissAll clears all toasts");

// =========================================================================
// 6. SbsConfirmService
// =========================================================================
var confirmSvc = new SbsConfirmService(localizer);
SbsConfirmOptions? receivedConfirm = null;
confirmSvc.OnShow += opt => receivedConfirm = opt;

var confirmTask1 = confirmSvc.ShowAsync("Silinsin mi?", confirmVariant: "danger");
Check(receivedConfirm is { Message: "Silinsin mi?", ConfirmVariant: "danger" } && !string.IsNullOrWhiteSpace(receivedConfirm.Title),
    "SbsConfirmService.ShowAsync raises OnShow with localized default title");
confirmSvc.Resolve(true);
Check(await confirmTask1 == true, "SbsConfirmService.Resolve(true) completes task with true");

var confirmTask2 = confirmSvc.ShowAsync("İlk soru");
var confirmTask3 = confirmSvc.ShowAsync("İkinci soru");
Check(await confirmTask2 == false, "SbsConfirmService cancels superseded confirmation with false");
confirmSvc.Resolve(false);
Check(await confirmTask3 == false, "SbsConfirmService.Resolve(false) completes task with false");

// =========================================================================
// 7. SbsDialogService
// =========================================================================
var dialogSvc = new SbsDialogService();
int dialogChanges = 0;
dialogSvc.OnDialogsChanged += () => dialogChanges++;

var dlgParams = new SbsDialogParameters().Add("Note", "Merhaba");
var dlgOpts = new SbsDialogOptions { Size = SbsModalSize.Lg, Icon = "fa fa-info" };
var dlg1 = await dialogSvc.ShowAsync<DummyDialogContent>("Detay", dlgParams, dlgOpts);
Check(dialogSvc.ActiveDialogs.Count == 1 && dlg1.Title == "Detay" && dlg1.Parameters["Note"]?.ToString() == "Merhaba" && dialogChanges == 1,
    "SbsDialogService.ShowAsync adds active dialog with parameters and options");

dlg1.Close("Sonuç-42");
var dlg1Result = await dlg1.Result;
Check(!dlg1Result.Canceled && dlg1Result.GetData<string>() == "Sonuç-42" && dialogSvc.ActiveDialogs.Count == 0,
    "SbsDialogInstance.Close<T> resolves typed SbsDialogResult and removes dialog");

var dlg2 = await dialogSvc.ShowAsync<DummyDialogContent>("İkinci");
var dlg3 = await dialogSvc.ShowAsync<DummyDialogContent>("Üçüncü");
dialogSvc.CloseAll();
var r2 = await dlg2.Result;
var r3 = await dlg3.Result;
Check(r2.Canceled && r3.Canceled && dialogSvc.ActiveDialogs.Count == 0,
    "SbsDialogService.CloseAll cancels all active dialogs");

var threwOnNonComponent = false;
try { await dialogSvc.ShowAsync(typeof(string), "Invalid"); }
catch (ArgumentException) { threwOnNonComponent = true; }
Check(threwOnNonComponent, "SbsDialogService rejects non-IComponent types");

// =========================================================================
// 8. SbsTextHelper Turkish Search Normalization (via reflection on internal class)
// =========================================================================
var helperType = typeof(SbsComponentBase).Assembly.GetType("Sobis.UI.SbsTextHelper")
    ?? throw new Exception("SbsTextHelper type not found in Sobis.UI");
var normalizeTr = helperType.GetMethod("NormalizeTr", BindingFlags.Public | BindingFlags.Static)!;
var containsTr = helperType.GetMethod("ContainsTr", BindingFlags.Public | BindingFlags.Static)!;
var equalsTr = helperType.GetMethod("EqualsTr", BindingFlags.Public | BindingFlags.Static)!;

Check((string)normalizeTr.Invoke(null, ["SİBEL ŞAHİN ÇAĞLA ÜNAL ÖZKAN IĞDIR"])! == "sibel sahin cagla unal ozkan igdir",
    "SbsTextHelper.NormalizeTr folds Turkish diacritics and dotted/dotless I");
Check((bool)containsTr.Invoke(null, ["İstanbul Şube Müdürlüğü", "sube mudurlugu"])!,
    "SbsTextHelper.ContainsTr matches Turkish text with ASCII query");
Check((bool)containsTr.Invoke(null, ["Istanbul Sube Mudurlugu", "İSTANBUL"])!,
    "SbsTextHelper.ContainsTr matches ASCII text with Turkish uppercase query");
Check((bool)equalsTr.Invoke(null, ["Çalışma", "calisma"])! && !(bool)equalsTr.Invoke(null, ["Çalışma", "baska"])!,
    "SbsTextHelper.EqualsTr performs Turkish-insensitive equality");

// =========================================================================
// 9. HtmlRenderer Component Rendering Checks (SbsGrid List & Declarative + SbsPage/SbsFormGroup)
// =========================================================================
using var provider = new ServiceCollection()
    .AddSbsUI()
    .AddSingleton<IJSRuntime, NoOpJsRuntime>()
    .BuildServiceProvider();

await using var renderer = new HtmlRenderer(provider, NullLoggerFactory.Instance);
async Task<string> RenderAsync<TComponent>(Dictionary<string, object?>? args = null) where TComponent : IComponent
{
    var html = await renderer.Dispatcher.InvokeAsync(async () =>
    {
        var output = await renderer.RenderComponentAsync<TComponent>(
            args is null ? ParameterView.Empty : ParameterView.FromDictionary(args));
        return System.Net.WebUtility.HtmlDecode(output.ToHtmlString());
    });
    return Regex.Replace(html, @"\s+", " ");
}

var rows = new List<GridRow>
{
    new("İstanbul Şube", 100m),
    new("Çankaya Birim", 250m),
    new("İzmir Merkez", 150m)
};

// 9a. SbsGrid imperative List<SbsGridColumn<T>> + FooterAggregate
var listColumns = new List<SbsGridColumn<GridRow>>
{
    new() { Field = nameof(GridRow.Ad), Caption = "Birim Adı", FooterAggregate = SbsGridAggregate.Count },
    new() { Field = nameof(GridRow.Tutar), Caption = "Tutar", FooterAggregate = SbsGridAggregate.Sum, FooterFormat = "N0" }
};
var listGridHtml = await RenderAsync<SbsGrid<GridRow>>(new()
{
    ["Data"] = rows,
    ["Columns"] = listColumns,
    ["Title"] = "Birim Listesi"
});
Check(listGridHtml.Contains("Birim Adı")
      && listGridHtml.Contains("İstanbul Şube")
      && listGridHtml.Contains("Çankaya Birim")
      && listGridHtml.Contains("<tfoot")
      && listGridHtml.Contains("500"),
    "SbsGrid renders imperative Columns list and footer Sum/Count aggregates");

// 9b. SbsGrid declarative <SbsGridColumn> markup inside ChildContent
RenderFragment declarativeCols = builder =>
{
    builder.OpenComponent<SbsGridColumn<GridRow>>(0);
    builder.AddAttribute(1, nameof(SbsGridColumn<GridRow>.Field), nameof(GridRow.Ad));
    builder.AddAttribute(2, nameof(SbsGridColumn<GridRow>.Caption), "Deklaratif Ad");
    builder.CloseComponent();
    builder.OpenComponent<SbsGridColumn<GridRow>>(3);
    builder.AddAttribute(4, nameof(SbsGridColumn<GridRow>.Field), nameof(GridRow.Tutar));
    builder.AddAttribute(5, nameof(SbsGridColumn<GridRow>.Caption), "Deklaratif Tutar");
    builder.CloseComponent();
};
var declGridHtml = await RenderAsync<SbsGrid<GridRow>>(new()
{
    ["Data"] = rows,
    ["ChildContent"] = declarativeCols
});
Check(declGridHtml.Contains("Deklaratif Ad")
      && declGridHtml.Contains("Deklaratif Tutar")
      && declGridHtml.Contains("İzmir Merkez"),
    "SbsGrid renders declarative SbsGridColumn children");

// 9c. SbsGrid Turkish search filter end-to-end
var gridInst = new SbsGrid<GridRow> { Data = rows, Columns = listColumns };
typeof(SbsGrid<GridRow>).GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(gridInst, []);
typeof(SbsGrid<GridRow>).GetMethod("OnSearchInput", BindingFlags.Instance | BindingFlags.NonPublic)!
    .Invoke(gridInst, [new ChangeEventArgs { Value = "cankaya" }]);
var visibleRows = (List<GridRow>)typeof(SbsGrid<GridRow>)
    .GetField("_visibleRows", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(gridInst)!;
Check(visibleRows.Count == 1 && visibleRows[0].Ad == "Çankaya Birim",
    "SbsGrid search filters rows using Turkish normalization ('cankaya' -> 'Çankaya Birim')");

// 9d. SbsPage -> SbsFormGroup(Disabled=true) -> SbsTextBox cascading required & disabled end-to-end
var pageHtml = await RenderAsync<CascadingIntegrationHost>();
Check(pageHtml.Contains("sbs-input-invalid") && pageHtml.Contains("disabled") && pageHtml.Contains("sbs-form-group-disabled"),
    "SbsPage + SbsFormGroup cascade CascadedRequiredFields and Disabled to SbsTextBox");

// 9e. DI AddSbsUI registers all core services cleanly
Check(provider.GetRequiredService<ISbsAuthorizationProvider>() is AllowAllSbsAuthorizationProvider
      && provider.GetRequiredService<ISbsLocalizer>() is DefaultSbsLocalizer
      && provider.GetRequiredService<ISbsToastService>() is SbsToastService
      && provider.GetRequiredService<ISbsConfirmService>() is SbsConfirmService
      && provider.GetRequiredService<ISbsDialogService>() is SbsDialogService,
    "AddSbsUI registers all 5 core Sobis.UI services");

Console.WriteLine($"\nALL {passed} SOBIS.UI CHECKS PASSED!");

// =========================================================================
// Test Helpers
// =========================================================================
sealed class SampleModel
{
    public string? Ad { get; set; }
    public string? Kod { get; set; }
    public int Sayi { get; set; }
}

sealed record GridRow(string Ad, decimal Tutar);

sealed class TestStringInput : SbsInputBase<string?>
{
    public string? ExposedActualPropertyName => ActualPropertyName;
    public bool ExposedIsRequired => IsRequired;
    public bool ExposedEffectiveDisabled => EffectiveDisabled;
    public bool ExposedEffectiveReadOnly => EffectiveReadOnly;
    public bool ExposedIsInvalid => IsInvalid;
    public string? ExposedEffectiveErrorText => EffectiveErrorText;
    public void InvokeOnParametersSet() => OnParametersSet();
    public static string? CallExtractPropertyName(LambdaExpression? expr) => ExtractPropertyName(expr);
}

sealed class DummyDialogContent : ComponentBase
{
    [Parameter] public string? Note { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, Note);
}

sealed class CascadingIntegrationHost : ComponentBase
{
    private readonly SampleModel _m = new();
    private readonly HashSet<string> _req = new(StringComparer.OrdinalIgnoreCase) { nameof(SampleModel.Ad) };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SbsPage>(0);
        builder.AddAttribute(1, nameof(SbsPage.RequiredFields), _req);
        builder.AddAttribute(2, nameof(SbsPage.ChildContent), (RenderFragment)(b2 =>
        {
            b2.OpenComponent<SbsFormGroup>(0);
            b2.AddAttribute(1, nameof(SbsFormGroup.Caption), "Test Form");
            b2.AddAttribute(2, nameof(SbsFormGroup.Disabled), true);
            b2.AddAttribute(3, nameof(SbsFormGroup.ChildContent), (RenderFragment)(b3 =>
            {
                b3.OpenComponent<SbsTextBox>(0);
                b3.AddAttribute(1, nameof(SbsTextBox.Value), _m.Ad);
                b3.AddAttribute(2, nameof(SbsTextBox.ValueExpression), (Expression<Func<string?>>)(() => _m.Ad));
                b3.CloseComponent();
            }));
            b2.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

sealed class NoOpJsRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        => ValueTask.FromResult(default(TValue)!);

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => ValueTask.FromResult(default(TValue)!);
}
