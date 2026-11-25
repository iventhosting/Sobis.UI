namespace Sobis.UI;

/// <summary>
/// Scoped confirmation dialog coordinator backed by a TaskCompletionSource.
/// </summary>
public sealed class SbsConfirmService : ISbsConfirmService
{
    private readonly ISbsLocalizer _localizer;
    private TaskCompletionSource<bool>? _tcs;

    public SbsConfirmService(ISbsLocalizer localizer)
    {
        _localizer = localizer;
    }

    public event Action<SbsConfirmOptions>? OnShow;

    public Task<bool> ShowAsync(
        string message,
        string? title = null,
        string icon = "fa fa-question-circle",
        string? subMessage = null,
        string? confirmText = null,
        string? cancelText = null,
        string confirmVariant = "primary")
    {
        return ShowAsync(new SbsConfirmOptions
        {
            Message = message,
            Title = string.IsNullOrWhiteSpace(title) ? _localizer[SbsLocalizerKeys.ConfirmTitle] : title,
            Icon = icon,
            SubMessage = subMessage,
            ConfirmText = confirmText,
            CancelText = cancelText,
            ConfirmVariant = confirmVariant
        });
    }

    public Task<bool> ShowAsync(SbsConfirmOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.Title))
            options.Title = _localizer[SbsLocalizerKeys.ConfirmTitle];

        _tcs?.TrySetResult(false);
        _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        OnShow?.Invoke(options);
        return _tcs.Task;
    }

    public void Resolve(bool result)
    {
        _tcs?.TrySetResult(result);
    }
}
