namespace Sobis.UI;

public sealed class SbsConfirmOptions
{
    public string Message { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = "fa fa-question-circle";
    public string? SubMessage { get; set; }
    public string? ConfirmText { get; set; }
    public string? CancelText { get; set; }
    public string ConfirmVariant { get; set; } = "primary";
}

/// <summary>
/// Confirmation dialog service rendered by SbsConfirmDialog.
/// </summary>
public interface ISbsConfirmService
{
    event Action<SbsConfirmOptions>? OnShow;

    Task<bool> ShowAsync(
        string message,
        string? title = null,
        string icon = "fa fa-question-circle",
        string? subMessage = null,
        string? confirmText = null,
        string? cancelText = null,
        string confirmVariant = "primary");

    Task<bool> ShowAsync(SbsConfirmOptions options);

    void Resolve(bool result);
}
