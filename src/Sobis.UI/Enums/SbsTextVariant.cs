namespace Sobis.UI;

/// <summary>
/// Typography variants for SbsText.
/// </summary>
public enum SbsTextVariant
{
    H1,
    H2,
    H3,
    H4,
    H5,
    H6,
    Subtitle,
    Body,
    Caption,
    Muted,
    Overline
}

public static class SbsTextVariantExtensions
{
    public static string ToCss(this SbsTextVariant variant) => variant switch
    {
        SbsTextVariant.H1 => "h1",
        SbsTextVariant.H2 => "h2",
        SbsTextVariant.H3 => "h3",
        SbsTextVariant.H4 => "h4",
        SbsTextVariant.H5 => "h5",
        SbsTextVariant.H6 => "h6",
        SbsTextVariant.Subtitle => "subtitle",
        SbsTextVariant.Body => "body",
        SbsTextVariant.Caption => "caption",
        SbsTextVariant.Muted => "muted",
        SbsTextVariant.Overline => "overline",
        _ => "body"
    };
}
