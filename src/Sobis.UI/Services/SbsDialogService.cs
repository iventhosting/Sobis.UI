using Microsoft.AspNetCore.Components;

namespace Sobis.UI;

/// <summary>
/// Scoped service managing active programmatic modal dialogs for SbsDialogProvider.
/// </summary>
public sealed class SbsDialogService : ISbsDialogService
{
    private static readonly IReadOnlyDictionary<string, object?> EmptyParameters =
        new Dictionary<string, object?>();

    private readonly List<SbsDialogInstance> _dialogs = [];
    private readonly object _lock = new();

    public event Action? OnDialogsChanged;

    public IReadOnlyList<SbsDialogInstance> ActiveDialogs
    {
        get
        {
            lock (_lock)
                return _dialogs.ToList();
        }
    }

    public Task<SbsDialogInstance> ShowAsync<TComponent>(
        string title,
        SbsDialogParameters? parameters = null,
        SbsDialogOptions? options = null) where TComponent : IComponent
        => ShowAsync(typeof(TComponent), title, parameters, options);

    public Task<SbsDialogInstance> ShowAsync(
        Type componentType,
        string title,
        SbsDialogParameters? parameters = null,
        SbsDialogOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        if (!typeof(IComponent).IsAssignableFrom(componentType))
            throw new ArgumentException($"Type '{componentType.FullName}' must implement IComponent.", nameof(componentType));

        var paramDict = parameters is not null
            ? new Dictionary<string, object?>(parameters.Values)
            : EmptyParameters;

        var instance = new SbsDialogInstance(
            title ?? string.Empty,
            componentType,
            paramDict,
            options ?? new SbsDialogOptions(),
            (id, result) => Close(id, result));

        lock (_lock)
            _dialogs.Add(instance);

        OnDialogsChanged?.Invoke();
        return Task.FromResult(instance);
    }

    public void Close(Guid id, SbsDialogResult? result = null)
    {
        SbsDialogInstance? removed = null;
        lock (_lock)
        {
            var index = _dialogs.FindIndex(d => d.Id == id);
            if (index >= 0)
            {
                removed = _dialogs[index];
                _dialogs.RemoveAt(index);
            }
        }

        if (removed is not null)
        {
            removed.Resolve(result ?? SbsDialogResult.Cancel());
            OnDialogsChanged?.Invoke();
        }
    }

    public void CloseAll()
    {
        List<SbsDialogInstance> snapshot;
        lock (_lock)
        {
            snapshot = _dialogs.ToList();
            _dialogs.Clear();
        }

        foreach (var dialog in snapshot)
            dialog.Resolve(SbsDialogResult.Cancel());

        if (snapshot.Count > 0)
            OnDialogsChanged?.Invoke();
    }
}
