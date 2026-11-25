using Microsoft.AspNetCore.Components;

namespace Sobis.UI;

/// <summary>
/// Result returned when an SbsDialogInstance closes.
/// </summary>
public sealed class SbsDialogResult
{
    private SbsDialogResult(bool canceled, object? data, Type? dataType)
    {
        Canceled = canceled;
        Data = data;
        DataType = dataType;
    }

    public bool Canceled { get; }
    public object? Data { get; }
    public Type? DataType { get; }

    public static SbsDialogResult Ok() => new(false, null, null);
    public static SbsDialogResult Ok<T>(T data) => new(false, data, typeof(T));
    public static SbsDialogResult Cancel() => new(true, null, null);

    public T? GetData<T>() => Data is T typed ? typed : default;
}

/// <summary>
/// Configuration options for programmatic dialogs shown through <see cref="ISbsDialogService"/>.
/// </summary>
public sealed class SbsDialogOptions
{
    public SbsModalSize Size { get; set; } = SbsModalSize.Md;
    public bool CloseOnBackdrop { get; set; } = true;
    public bool ShowCloseButton { get; set; } = true;
    public string? Icon { get; set; }
    public string? CssClass { get; set; }
    public string? Style { get; set; }
}

/// <summary>
/// Parameter bag passed to dynamically rendered dialog components.
/// </summary>
public sealed class SbsDialogParameters
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, object?> Values => _values;

    public SbsDialogParameters Add(string name, object? value)
    {
        _values[name] = value;
        return this;
    }

    public T? Get<T>(string name)
        => _values.TryGetValue(name, out var val) && val is T typed ? typed : default;
}

/// <summary>
/// Active dialog handle cascaded into dialog body components and returned by <see cref="ISbsDialogService"/>.
/// </summary>
public sealed class SbsDialogInstance
{
    private readonly Action<Guid, SbsDialogResult> _closeCallback;
    private readonly TaskCompletionSource<SbsDialogResult> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal SbsDialogInstance(
        string title,
        Type componentType,
        IReadOnlyDictionary<string, object?> parameters,
        SbsDialogOptions options,
        Action<Guid, SbsDialogResult> closeCallback)
    {
        Id = Guid.NewGuid();
        Title = title;
        ComponentType = componentType;
        Parameters = parameters;
        Options = options;
        _closeCallback = closeCallback;
    }

    public Guid Id { get; }
    public string Title { get; }
    public Type ComponentType { get; }
    public IReadOnlyDictionary<string, object?> Parameters { get; }
    public SbsDialogOptions Options { get; }
    public Task<SbsDialogResult> Result => _tcs.Task;

    public void Close() => _closeCallback(Id, SbsDialogResult.Ok());
    public void Close<T>(T data) => _closeCallback(Id, SbsDialogResult.Ok(data));
    public void Close(SbsDialogResult result) => _closeCallback(Id, result);
    public void Cancel() => _closeCallback(Id, SbsDialogResult.Cancel());

    internal void Resolve(SbsDialogResult result) => _tcs.TrySetResult(result);
}

/// <summary>
/// Programmatic modal dialog service rendered by SbsDialogProvider.
/// </summary>
public interface ISbsDialogService
{
    event Action? OnDialogsChanged;
    IReadOnlyList<SbsDialogInstance> ActiveDialogs { get; }

    Task<SbsDialogInstance> ShowAsync<TComponent>(
        string title,
        SbsDialogParameters? parameters = null,
        SbsDialogOptions? options = null) where TComponent : IComponent;

    Task<SbsDialogInstance> ShowAsync(
        Type componentType,
        string title,
        SbsDialogParameters? parameters = null,
        SbsDialogOptions? options = null);

    void Close(Guid id, SbsDialogResult? result = null);
    void CloseAll();
}
