namespace Sobis.UI;

/// <summary>
/// Label placement modes for SbsFormLayout and SbsField.
/// </summary>
public enum SbsLabelPosition
{
    Left,
    Top,
    Hidden
}

public static class SbsLabelPositionExtensions
{
    public static string ToCss(this SbsLabelPosition position) => position switch
    {
        SbsLabelPosition.Top => "top",
        SbsLabelPosition.Hidden => "hidden",
        _ => "left"
    };
}
