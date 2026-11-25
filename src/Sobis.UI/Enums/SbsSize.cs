namespace Sobis.UI;

/// <summary>
/// Standardized sizing scale for Sobis.UI components.
/// </summary>
public enum SbsSize
{
    Xs,
    Sm,
    Md,
    Lg,
    Xl
}

public static class SbsSizeExtensions
{
    public static string ToCss(this SbsSize size) => size switch
    {
        SbsSize.Xs => "xs",
        SbsSize.Sm => "sm",
        SbsSize.Md => "md",
        SbsSize.Lg => "lg",
        SbsSize.Xl => "xl",
        _ => "md"
    };

    public static bool TryParse(string? value, out SbsSize size)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            size = SbsSize.Md;
            return false;
        }

        return Enum.TryParse(value.Trim(), ignoreCase: true, out size);
    }
}
