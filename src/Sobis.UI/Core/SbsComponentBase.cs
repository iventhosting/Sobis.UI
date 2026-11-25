using Microsoft.AspNetCore.Components;

namespace Sobis.UI;

/// <summary>
/// Base class for all Sobis.UI components providing standardized styling, class merging,
/// and unmatched HTML attribute forwarding.
/// </summary>
public abstract class SbsComponentBase : ComponentBase
{
    /// <summary>Additional CSS classes appended to the component root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Additional inline CSS styles appended to the component root element.</summary>
    [Parameter] public string? Style { get; set; }

    /// <summary>Captures any additional HTML attributes passed to the component.</summary>
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Returns <see cref="AdditionalAttributes"/> without "class" and "style" entries
    /// so they can be splatted safely onto an element whose class/style are built via
    /// <see cref="CssBuilder"/> and <see cref="StyleBuilder"/>.
    /// </summary>
    protected IReadOnlyDictionary<string, object>? UserAttributes
    {
        get
        {
            if (AdditionalAttributes is null || AdditionalAttributes.Count == 0)
                return null;

            var hasClassOrStyle = false;
            foreach (var key in AdditionalAttributes.Keys)
            {
                if (string.Equals(key, "class", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
                {
                    hasClassOrStyle = true;
                    break;
                }
            }

            if (!hasClassOrStyle)
                return AdditionalAttributes;

            var filtered = new Dictionary<string, object>(AdditionalAttributes.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in AdditionalAttributes)
            {
                if (string.Equals(kv.Key, "class", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(kv.Key, "style", StringComparison.OrdinalIgnoreCase))
                    continue;

                filtered[kv.Key] = kv.Value;
            }

            return filtered.Count > 0 ? filtered : null;
        }
    }

    protected CssBuilder CreateCssBuilder(string? baseClass = null)
        => CssBuilder.Default(baseClass);

    protected StyleBuilder CreateStyleBuilder(string? baseStyle = null)
        => string.IsNullOrWhiteSpace(baseStyle) ? StyleBuilder.Empty() : StyleBuilder.Empty().AddStyle(baseStyle);

    protected string BuildClass(string? baseClass = null)
        => CssBuilder.Default(baseClass)
            .AddClass(CssClass)
            .AddClassFromAttributes(AdditionalAttributes)
            .ToString();

    protected string BuildClass(CssBuilder builder)
        => builder
            .AddClass(CssClass)
            .AddClassFromAttributes(AdditionalAttributes)
            .ToString();

    protected string? BuildStyle(string? baseStyle = null)
        => StyleBuilder.Empty()
            .AddStyle(baseStyle)
            .AddStyle(Style)
            .AddStyleFromAttributes(AdditionalAttributes)
            .Build();

    protected string? BuildStyle(StyleBuilder builder)
        => builder
            .AddStyle(Style)
            .AddStyleFromAttributes(AdditionalAttributes)
            .Build();
}
