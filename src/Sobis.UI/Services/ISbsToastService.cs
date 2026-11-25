namespace Sobis.UI;

public enum SbsToastType
{
    Success,
    Error,
    Warning,
    Info
}

public enum SbsToastPosition
{
    TopRight,
    TopLeft,
    TopCenter,
    BottomRight,
    BottomLeft,
    BottomCenter
}

public sealed class SbsToastMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Title { get; init; }
    public string Message { get; init; } = string.Empty;
    public SbsToastType Type { get; init; }
    public SbsToastPosition Position { get; init; } = SbsToastPosition.TopRight;
    public bool ShowCloseButton { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}

/// <summary>
/// Toast notification service rendered by SbsToastContainer.
/// </summary>
public interface ISbsToastService
{
    event Action? OnToastsChanged;
    event Action<SbsToastMessage>? OnToastShown;
    event Action<string>? OnErrorShown;

    IReadOnlyList<SbsToastMessage> Toasts { get; }

    SbsToastPosition DefaultPosition { get; set; }
    bool DefaultShowCloseButton { get; set; }

    void Show(
        string message,
        SbsToastType type,
        int durationMs = 3500,
        SbsToastPosition? position = null,
        bool? showCloseButton = null,
        string? title = null);

    void Success(string message, int durationMs = 3000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null);
    void Error(string message, int durationMs = 5000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null);
    void Warning(string message, int durationMs = 4000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null);
    void Info(string message, int durationMs = 3000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null);

    void Dismiss(Guid id);
    void DismissAll();
}
