namespace Sobis.UI;

/// <summary>
/// Cross-axis alignment options (align-items) for flex/grid containers.
/// </summary>
public enum SbsAlign
{
    Start,
    Center,
    End,
    Stretch,
    Baseline
}

/// <summary>
/// Horizontal text alignment options for SbsText and table cells.
/// </summary>
public enum SbsTextAlign
{
    Left,
    Center,
    Right,
    Justify
}

public static class SbsAlignExtensions
{
    public static string ToCss(this SbsAlign align) => align switch
    {
        SbsAlign.Start => "flex-start",
        SbsAlign.Center => "center",
        SbsAlign.End => "flex-end",
        SbsAlign.Stretch => "stretch",
        SbsAlign.Baseline => "baseline",
        _ => "stretch"
    };

    public static string ToCss(this SbsTextAlign align) => align switch
    {
        SbsTextAlign.Left => "left",
        SbsTextAlign.Center => "center",
        SbsTextAlign.Right => "right",
        SbsTextAlign.Justify => "justify",
        _ => "left"
    };
}
