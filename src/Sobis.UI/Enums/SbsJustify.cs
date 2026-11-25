namespace Sobis.UI;

/// <summary>
/// Main-axis alignment options (justify-content) for flex/grid containers.
/// </summary>
public enum SbsJustify
{
    Start,
    Center,
    End,
    Between,
    Around,
    Evenly
}

public static class SbsJustifyExtensions
{
    public static string ToCss(this SbsJustify justify) => justify switch
    {
        SbsJustify.Start => "flex-start",
        SbsJustify.Center => "center",
        SbsJustify.End => "flex-end",
        SbsJustify.Between => "space-between",
        SbsJustify.Around => "space-around",
        SbsJustify.Evenly => "space-evenly",
        _ => "flex-start"
    };
}
