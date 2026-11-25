using System.Globalization;

namespace Sobis.UI;

/// <summary>
/// Built-in localizer with Turkish and English defaults plus optional <see cref="SbsOptions.LocalizationOverrides"/>.
/// </summary>
public sealed class DefaultSbsLocalizer : ISbsLocalizer
{
    private static readonly Dictionary<string, string> TrDefaults = new(StringComparer.OrdinalIgnoreCase)
    {
        [SbsLocalizerKeys.Ok] = "Tamam",
        [SbsLocalizerKeys.Cancel] = "İptal",
        [SbsLocalizerKeys.Close] = "Kapat",
        [SbsLocalizerKeys.Clear] = "Temizle",
        [SbsLocalizerKeys.Search] = "Ara",
        [SbsLocalizerKeys.SearchPlaceholder] = "Ara...",
        [SbsLocalizerKeys.SelectAll] = "Tümünü Seç",
        [SbsLocalizerKeys.ClearSelection] = "Seçimi Temizle",
        [SbsLocalizerKeys.SelectedCount] = "{0} seçili",
        [SbsLocalizerKeys.NoOptions] = "Seçenek yok",
        [SbsLocalizerKeys.MoreResults] = "+{0} sonuç daha...",
        [SbsLocalizerKeys.Loading] = "Yükleniyor...",
        [SbsLocalizerKeys.ConfirmTitle] = "Onay",

        [SbsLocalizerKeys.Today] = "Bugün",
        [SbsLocalizerKeys.DatePlaceholder] = "gg.aa.yyyy",
        [SbsLocalizerKeys.DayMon] = "Pzt",
        [SbsLocalizerKeys.DayTue] = "Sal",
        [SbsLocalizerKeys.DayWed] = "Çar",
        [SbsLocalizerKeys.DayThu] = "Per",
        [SbsLocalizerKeys.DayFri] = "Cum",
        [SbsLocalizerKeys.DaySat] = "Cmt",
        [SbsLocalizerKeys.DaySun] = "Paz",

        [SbsLocalizerKeys.Page] = "Sayfa",
        [SbsLocalizerKeys.First] = "İlk",
        [SbsLocalizerKeys.Previous] = "Önceki",
        [SbsLocalizerKeys.Next] = "Sonraki",
        [SbsLocalizerKeys.Last] = "Son",
        [SbsLocalizerKeys.Status] = "Durum",
        [SbsLocalizerKeys.Edit] = "Düzenle",
        [SbsLocalizerKeys.Delete] = "Sil",
        [SbsLocalizerKeys.RecordCount] = "{0} kayıt",
        [SbsLocalizerKeys.EmptyGrid] = "Kayıt bulunamadı",
        [SbsLocalizerKeys.ExportExcel] = "Excel'e Aktar",
        [SbsLocalizerKeys.ExportPdf] = "PDF'e Aktar",
        [SbsLocalizerKeys.ReportDefaultTitle] = "Rapor",

        [SbsLocalizerKeys.ToastSuccess] = "Başarılı",
        [SbsLocalizerKeys.ToastError] = "Hata",
        [SbsLocalizerKeys.ToastWarning] = "Uyarı",
        [SbsLocalizerKeys.ToastInfo] = "Bilgi",
        [SbsLocalizerKeys.ToastNotification] = "Bildirim",

        [SbsLocalizerKeys.DropFilesHere] = "Dosyaları buraya sürükleyin veya seçin",
        [SbsLocalizerKeys.SelectFiles] = "Dosya Seç",
        [SbsLocalizerKeys.RemoveFile] = "Kaldır"
    };

    private static readonly Dictionary<string, string> EnDefaults = new(StringComparer.OrdinalIgnoreCase)
    {
        [SbsLocalizerKeys.Ok] = "OK",
        [SbsLocalizerKeys.Cancel] = "Cancel",
        [SbsLocalizerKeys.Close] = "Close",
        [SbsLocalizerKeys.Clear] = "Clear",
        [SbsLocalizerKeys.Search] = "Search",
        [SbsLocalizerKeys.SearchPlaceholder] = "Search...",
        [SbsLocalizerKeys.SelectAll] = "Select All",
        [SbsLocalizerKeys.ClearSelection] = "Clear Selection",
        [SbsLocalizerKeys.SelectedCount] = "{0} selected",
        [SbsLocalizerKeys.NoOptions] = "No options",
        [SbsLocalizerKeys.MoreResults] = "+{0} more results...",
        [SbsLocalizerKeys.Loading] = "Loading...",
        [SbsLocalizerKeys.ConfirmTitle] = "Confirm",

        [SbsLocalizerKeys.Today] = "Today",
        [SbsLocalizerKeys.DatePlaceholder] = "dd.MM.yyyy",
        [SbsLocalizerKeys.DayMon] = "Mon",
        [SbsLocalizerKeys.DayTue] = "Tue",
        [SbsLocalizerKeys.DayWed] = "Wed",
        [SbsLocalizerKeys.DayThu] = "Thu",
        [SbsLocalizerKeys.DayFri] = "Fri",
        [SbsLocalizerKeys.DaySat] = "Sat",
        [SbsLocalizerKeys.DaySun] = "Sun",

        [SbsLocalizerKeys.Page] = "Page",
        [SbsLocalizerKeys.First] = "First",
        [SbsLocalizerKeys.Previous] = "Previous",
        [SbsLocalizerKeys.Next] = "Next",
        [SbsLocalizerKeys.Last] = "Last",
        [SbsLocalizerKeys.Status] = "Status",
        [SbsLocalizerKeys.Edit] = "Edit",
        [SbsLocalizerKeys.Delete] = "Delete",
        [SbsLocalizerKeys.RecordCount] = "{0} records",
        [SbsLocalizerKeys.EmptyGrid] = "No records found",
        [SbsLocalizerKeys.ExportExcel] = "Export to Excel",
        [SbsLocalizerKeys.ExportPdf] = "Export to PDF",
        [SbsLocalizerKeys.ReportDefaultTitle] = "Report",

        [SbsLocalizerKeys.ToastSuccess] = "Success",
        [SbsLocalizerKeys.ToastError] = "Error",
        [SbsLocalizerKeys.ToastWarning] = "Warning",
        [SbsLocalizerKeys.ToastInfo] = "Info",
        [SbsLocalizerKeys.ToastNotification] = "Notification",

        [SbsLocalizerKeys.DropFilesHere] = "Drop files here or click to browse",
        [SbsLocalizerKeys.SelectFiles] = "Select Files",
        [SbsLocalizerKeys.RemoveFile] = "Remove"
    };

    private readonly IReadOnlyDictionary<string, string>? _overrides;

    public DefaultSbsLocalizer(SbsOptions? options = null)
    {
        _overrides = options?.LocalizationOverrides;
    }

    public string this[string key] => Get(key);

    public string this[string key, params object[] arguments] => Format(key, arguments);

    public string Get(string key, string? fallback = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return fallback ?? string.Empty;

        if (_overrides is not null && _overrides.TryGetValue(key, out var overridden))
            return overridden;

        var isTurkish = string.Equals(
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
            "tr",
            StringComparison.OrdinalIgnoreCase);

        var primary = isTurkish ? TrDefaults : EnDefaults;
        if (primary.TryGetValue(key, out var val))
            return val;

        var secondary = isTurkish ? EnDefaults : TrDefaults;
        if (secondary.TryGetValue(key, out var secondaryVal))
            return secondaryVal;

        return fallback ?? key;
    }

    public string Format(string key, params object[] arguments)
    {
        var template = Get(key);
        if (arguments is null || arguments.Length == 0)
            return template;

        try
        {
            return string.Format(CultureInfo.CurrentUICulture, template, arguments);
        }
        catch
        {
            return template;
        }
    }
}
