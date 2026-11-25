using System.Collections.Concurrent;

namespace Sobis.UI;

/// <summary>
/// Thread-safe scoped toast notification service with per-message auto-dismiss timers.
/// </summary>
public sealed class SbsToastService : ISbsToastService, IDisposable
{
    private readonly List<SbsToastMessage> _toasts = [];
    private readonly ConcurrentDictionary<Guid, Timer> _timers = new();
    private readonly object _lock = new();
    private readonly ISbsLocalizer _localizer;
    private bool _disposed;

    public SbsToastService(ISbsLocalizer localizer, SbsOptions? options = null)
    {
        _localizer = localizer;
        if (options is not null)
        {
            DefaultPosition = options.DefaultToastPosition;
            DefaultShowCloseButton = options.DefaultToastShowCloseButton;
        }
    }

    public event Action? OnToastsChanged;
    public event Action<SbsToastMessage>? OnToastShown;
    public event Action<string>? OnErrorShown;

    public IReadOnlyList<SbsToastMessage> Toasts
    {
        get
        {
            lock (_lock)
                return _toasts.ToList();
        }
    }

    public SbsToastPosition DefaultPosition { get; set; } = SbsToastPosition.TopRight;
    public bool DefaultShowCloseButton { get; set; } = true;

    public void Show(
        string message,
        SbsToastType type,
        int durationMs = 3500,
        SbsToastPosition? position = null,
        bool? showCloseButton = null,
        string? title = null)
    {
        if (_disposed || string.IsNullOrWhiteSpace(message))
            return;

        if (string.IsNullOrWhiteSpace(title))
        {
            title = type switch
            {
                SbsToastType.Success => _localizer[SbsLocalizerKeys.ToastSuccess],
                SbsToastType.Error => _localizer[SbsLocalizerKeys.ToastError],
                SbsToastType.Warning => _localizer[SbsLocalizerKeys.ToastWarning],
                SbsToastType.Info => _localizer[SbsLocalizerKeys.ToastInfo],
                _ => _localizer[SbsLocalizerKeys.ToastNotification]
            };
        }

        var msg = new SbsToastMessage
        {
            Title = title,
            Message = message,
            Type = type,
            Position = position ?? DefaultPosition,
            ShowCloseButton = showCloseButton ?? DefaultShowCloseButton
        };

        lock (_lock)
            _toasts.Add(msg);

        Notify();

        try { OnToastShown?.Invoke(msg); } catch { }

        if (type == SbsToastType.Error)
        {
            try { OnErrorShown?.Invoke(message); } catch { }
        }

        if (durationMs > 0)
        {
            var timer = new Timer(_ => Dismiss(msg.Id), null, durationMs, Timeout.Infinite);
            _timers[msg.Id] = timer;
        }
    }

    public void Success(string message, int durationMs = 3000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null)
        => Show(message, SbsToastType.Success, durationMs, position, showCloseButton, title);

    public void Error(string message, int durationMs = 5000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null)
        => Show(message, SbsToastType.Error, durationMs, position, showCloseButton, title);

    public void Warning(string message, int durationMs = 4000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null)
        => Show(message, SbsToastType.Warning, durationMs, position, showCloseButton, title);

    public void Info(string message, int durationMs = 3000, SbsToastPosition? position = null, bool? showCloseButton = null, string? title = null)
        => Show(message, SbsToastType.Info, durationMs, position, showCloseButton, title);

    public void Dismiss(Guid id)
    {
        lock (_lock)
        {
            var item = _toasts.FirstOrDefault(t => t.Id == id);
            if (item is not null)
                _toasts.Remove(item);
        }

        if (_timers.TryRemove(id, out var timer))
            timer.Dispose();

        Notify();
    }

    public void DismissAll()
    {
        lock (_lock)
            _toasts.Clear();

        foreach (var kv in _timers)
        {
            if (_timers.TryRemove(kv.Key, out var timer))
                timer.Dispose();
        }

        Notify();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        foreach (var kv in _timers)
        {
            if (_timers.TryRemove(kv.Key, out var timer))
                timer.Dispose();
        }

        lock (_lock)
            _toasts.Clear();
    }

    private void Notify() => OnToastsChanged?.Invoke();
}
