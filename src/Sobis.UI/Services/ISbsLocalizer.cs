namespace Sobis.UI;

/// <summary>
/// Lightweight localization abstraction for built-in Sobis.UI component strings.
/// Host applications can register a custom implementation to bridge IStringLocalizer or DB localization.
/// </summary>
public interface ISbsLocalizer
{
    string this[string key] { get; }
    string this[string key, params object[] arguments] { get; }
    string Get(string key, string? fallback = null);
    string Format(string key, params object[] arguments);
}

/// <summary>
/// Built-in localization keys used across Sobis.UI components.
/// </summary>
public static class SbsLocalizerKeys
{
    public const string Ok = "Sbs.Ok";
    public const string Cancel = "Sbs.Cancel";
    public const string Close = "Sbs.Close";
    public const string Clear = "Sbs.Clear";
    public const string Search = "Sbs.Search";
    public const string SearchPlaceholder = "Sbs.SearchPlaceholder";
    public const string SelectAll = "Sbs.SelectAll";
    public const string ClearSelection = "Sbs.ClearSelection";
    public const string SelectedCount = "Sbs.SelectedCount";
    public const string NoOptions = "Sbs.NoOptions";
    public const string MoreResults = "Sbs.MoreResults";
    public const string Loading = "Sbs.Loading";
    public const string ConfirmTitle = "Sbs.ConfirmTitle";

    // DateBox
    public const string Today = "Sbs.Date.Today";
    public const string DatePlaceholder = "Sbs.Date.Placeholder";
    public const string DayMon = "Sbs.Date.Mon";
    public const string DayTue = "Sbs.Date.Tue";
    public const string DayWed = "Sbs.Date.Wed";
    public const string DayThu = "Sbs.Date.Thu";
    public const string DayFri = "Sbs.Date.Fri";
    public const string DaySat = "Sbs.Date.Sat";
    public const string DaySun = "Sbs.Date.Sun";

    // Grid
    public const string Page = "Sbs.Grid.Page";
    public const string First = "Sbs.Grid.First";
    public const string Previous = "Sbs.Grid.Previous";
    public const string Next = "Sbs.Grid.Next";
    public const string Last = "Sbs.Grid.Last";
    public const string Status = "Sbs.Grid.Status";
    public const string Edit = "Sbs.Grid.Edit";
    public const string Delete = "Sbs.Grid.Delete";
    public const string RecordCount = "Sbs.Grid.RecordCount";
    public const string EmptyGrid = "Sbs.Grid.Empty";
    public const string ExportExcel = "Sbs.Grid.ExportExcel";
    public const string ExportPdf = "Sbs.Grid.ExportPdf";
    public const string ReportDefaultTitle = "Sbs.Grid.Report";

    // Toast
    public const string ToastSuccess = "Sbs.Toast.Success";
    public const string ToastError = "Sbs.Toast.Error";
    public const string ToastWarning = "Sbs.Toast.Warning";
    public const string ToastInfo = "Sbs.Toast.Info";
    public const string ToastNotification = "Sbs.Toast.Notification";

    // FileUpload
    public const string DropFilesHere = "Sbs.Upload.DropHere";
    public const string SelectFiles = "Sbs.Upload.SelectFiles";
    public const string RemoveFile = "Sbs.Upload.RemoveFile";
}
