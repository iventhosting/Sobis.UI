using System.Text;

namespace Sobis.UI;

/// <summary>
/// Lightweight fluent builder for constructing inline CSS style strings.
/// </summary>
public struct StyleBuilder
{
    private StringBuilder? _buffer;

    private StyleBuilder(string? prop, string? value)
    {
        _buffer = null;
        if (!string.IsNullOrWhiteSpace(prop) && !string.IsNullOrWhiteSpace(value))
        {
            _buffer = new StringBuilder();
            AppendDeclaration(_buffer, prop.Trim(), value.Trim());
        }
    }

    public static StyleBuilder Empty() => new(null, null);

    public static StyleBuilder Default(string? prop, string? value) => new(prop, value);

    public StyleBuilder AddStyle(string? rawStyle)
    {
        if (string.IsNullOrWhiteSpace(rawStyle))
            return this;

        var trimmed = rawStyle.Trim().TrimEnd(';');
        if (trimmed.Length == 0)
            return this;

        _buffer ??= new StringBuilder();
        if (_buffer.Length > 0)
            _buffer.Append(' ');

        _buffer.Append(trimmed).Append(';');
        return this;
    }

    public StyleBuilder AddStyle(string? rawStyle, bool when)
        => when ? AddStyle(rawStyle) : this;

    public StyleBuilder AddStyle(string prop, string? value, bool when = true)
    {
        if (!when || string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value))
            return this;

        _buffer ??= new StringBuilder();
        if (_buffer.Length > 0)
            _buffer.Append(' ');

        AppendDeclaration(_buffer, prop.Trim(), value.Trim());
        return this;
    }

    public StyleBuilder AddStyle(string prop, Func<string?> value, bool when = true)
        => when ? AddStyle(prop, value(), true) : this;

    public StyleBuilder AddStyle(StyleBuilder builder, bool when = true)
        => when ? AddStyle(builder.Build()) : this;

    public StyleBuilder AddStyleFromAttributes(IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes is null)
            return this;

        if (additionalAttributes.TryGetValue("style", out var value) && value is not null)
            return AddStyle(value.ToString());

        foreach (var kv in additionalAttributes)
        {
            if (string.Equals(kv.Key, "style", StringComparison.OrdinalIgnoreCase) && kv.Value is not null)
                return AddStyle(kv.Value.ToString());
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

    private static void AppendDeclaration(StringBuilder sb, string prop, string value)
    {
        sb.Append(prop).Append(':').Append(value.TrimEnd(';')).Append(';');
    }
}
