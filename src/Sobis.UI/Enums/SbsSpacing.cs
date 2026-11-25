namespace Sobis.UI;

/// <summary>
/// Spacing scale tokens mapped to --sbs-space-* CSS custom properties.
/// </summary>
public enum SbsSpacing
{
    None = 0,
    Xs = 1,
    Sm = 2,
    Md = 3,
    Lg = 4,
    Xl = 5
}

public static class SbsSpacingExtensions
{
    public static string ToCssVar(this SbsSpacing spacing) => spacing switch
    {
        SbsSpacing.None => "0",
        SbsSpacing.Xs => "var(--sbs-space-xs)",
        SbsSpacing.Sm => "var(--sbs-space-sm)",
        SbsSpacing.Md => "var(--sbs-space-md)",
        SbsSpacing.Lg => "var(--sbs-space-lg)",
        SbsSpacing.Xl => "var(--sbs-space-xl)",
        _ => "0"
    };

    public static string ToCssToken(this SbsSpacing spacing) => spacing switch
    {
        SbsSpacing.None => "none",
        SbsSpacing.Xs => "xs",
        SbsSpacing.Sm => "sm",
        SbsSpacing.Md => "md",
        SbsSpacing.Lg => "lg",
        SbsSpacing.Xl => "xl",
        _ => "none"
    };
}
