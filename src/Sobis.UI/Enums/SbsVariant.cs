namespace Sobis.UI;

/// <summary>
/// Visual style variants for buttons, badges, chips, and surfaces.
/// </summary>
public enum SbsVariant
{
    Primary,
    Secondary,
    Ghost,
    Save,
    Success,
    Warning,
    Danger,
    Info,
    Link,
    Filled,
    Soft,
    Outlined,
    Text
}

/// <summary>
/// Tab bar presentation styles for SbsTabs.
/// </summary>
public enum SbsTabsVariant
{
    Line,
    Pill,
    Card
}

/// <summary>
/// Skeleton loading placeholder shapes for SbsSkeleton.
/// </summary>
public enum SbsSkeletonVariant
{
    Text,
    Rect,
    Circle
}

public static class SbsVariantExtensions
{
    public static string ToCss(this SbsVariant variant) => variant switch
    {
        SbsVariant.Primary => "primary",
        SbsVariant.Secondary => "secondary",
        SbsVariant.Ghost => "ghost",
        SbsVariant.Save => "save",
        SbsVariant.Success => "success",
        SbsVariant.Warning => "warning",
        SbsVariant.Danger => "danger",
        SbsVariant.Info => "info",
        SbsVariant.Link => "link",
        SbsVariant.Filled => "filled",
        SbsVariant.Soft => "soft",
        SbsVariant.Outlined => "outlined",
        SbsVariant.Text => "text",
        _ => "primary"
    };

    public static bool TryParse(string? value, out SbsVariant variant)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            variant = SbsVariant.Primary;
            return false;
        }

        return Enum.TryParse(value.Trim(), ignoreCase: true, out variant);
    }

    public static string ToCss(this SbsTabsVariant variant) => variant switch
    {
        SbsTabsVariant.Line => "line",
        SbsTabsVariant.Pill => "pill",
        SbsTabsVariant.Card => "card",
        _ => "line"
    };

    public static string ToCss(this SbsSkeletonVariant variant) => variant switch
    {
        SbsSkeletonVariant.Text => "text",
        SbsSkeletonVariant.Rect => "rect",
        SbsSkeletonVariant.Circle => "circle",
        _ => "text"
    };
}
