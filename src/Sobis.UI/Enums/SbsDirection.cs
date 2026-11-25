namespace Sobis.UI;

/// <summary>
/// Flex direction options for SbsStack and layout containers.
/// </summary>
public enum SbsDirection
{
    Row,
    Column,
    RowReverse,
    ColumnReverse
}

/// <summary>
/// Orientation for dividers, splitters, and radio groups.
/// </summary>
public enum SbsOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Placement / anchor options for tooltips, popovers, and drawers.
/// </summary>
public enum SbsPlacement
{
    Top,
    Bottom,
    Left,
    Right
}

public static class SbsDirectionExtensions
{
    public static string ToCss(this SbsDirection direction) => direction switch
    {
        SbsDirection.Row => "row",
        SbsDirection.Column => "column",
        SbsDirection.RowReverse => "row-reverse",
        SbsDirection.ColumnReverse => "column-reverse",
        _ => "row"
    };

    public static string ToCss(this SbsOrientation orientation) => orientation switch
    {
        SbsOrientation.Vertical => "vertical",
        _ => "horizontal"
    };

    public static string ToCss(this SbsPlacement placement) => placement switch
    {
        SbsPlacement.Top => "top",
        SbsPlacement.Bottom => "bottom",
        SbsPlacement.Left => "left",
        SbsPlacement.Right => "right",
        _ => "top"
    };
}
