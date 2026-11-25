namespace Sobis.UI;

/// <summary>
/// Semantic color palette tokens for Sobis.UI components.
/// </summary>
public enum SbsColor
{
    Default,
    Primary,
    Secondary,
    Success,
    Warning,
    Danger,
    Info,
    Muted
}

public static class SbsColorExtensions
{
    public static string ToCss(this SbsColor color) => color switch
    {
        SbsColor.Primary => "primary",
        SbsColor.Secondary => "secondary",
        SbsColor.Success => "success",
        SbsColor.Warning => "warning",
        SbsColor.Danger => "danger",
        SbsColor.Info => "info",
        SbsColor.Muted => "muted",
        _ => "default"
    };
}
