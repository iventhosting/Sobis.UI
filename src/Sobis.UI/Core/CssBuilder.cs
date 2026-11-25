using System.Text;

namespace Sobis.UI;

/// <summary>
/// Lightweight fluent builder for constructing space-delimited CSS class strings.
/// </summary>
public struct CssBuilder
{
    private StringBuilder? _buffer;

    private CssBuilder(string? value)
    {
        _buffer = null;
        if (!string.IsNullOrWhiteSpace(value))
        {
            _buffer = new StringBuilder(value.Trim());
        }
    }

    public static CssBuilder Empty() => new(null);

    public static CssBuilder Default(string? value = null) => new(value);

    public CssBuilder AddClass(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return this;

        var trimmed = value.Trim();
        if (_buffer is null)
        {
            _buffer = new StringBuilder(trimmed);
        }
        else if (_buffer.Length == 0)
        {
            _buffer.Append(trimmed);
        }
        else
        {
            _buffer.Append(' ').Append(trimmed);
        }

        return this;
    }

    public CssBuilder AddClass(string? value, bool when)
        => when ? AddClass(value) : this;

    public CssBuilder AddClass(string? value, Func<bool>? when)
        => when is not null && when() ? AddClass(value) : this;

    public CssBuilder AddClass(Func<string?> value, bool when = true)
        => when ? AddClass(value()) : this;

    public CssBuilder AddClass(CssBuilder builder, bool when = true)
        => when ? AddClass(builder.Build()) : this;

    public CssBuilder AddClassFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null)
            return this;

        if (additionalAttributes.TryGetValue("class", out var value) && value is not null)
            return AddClass(value.ToString());

        foreach (var kv in additionalAttributes)
        {
            if (string.Equals(kv.Key, "class", StringComparison.OrdinalIgnoreCase) && kv.Value is not null)
                return AddClass(kv.Value.ToString());
        }

        return this;
    }

    public readonly string? Build()
    {
        if (_buffer is null || _buffer.Length == 0)
            return null;

        return _buffer.ToString();
    }

    public readonly override string ToString() => Build() ?? string.Empty;
}
