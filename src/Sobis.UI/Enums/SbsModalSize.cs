namespace Sobis.UI;

/// <summary>
/// Width presets for SbsModal and SbsDialogOptions.
/// </summary>
public enum SbsModalSize
{
    Xs,
    Sm,
    Md,
    Lg,
    Xl,
    Xxl,
    Full
}

public static class SbsModalSizeExtensions
{
    public static string ToCss(this SbsModalSize size) => size switch
    {
        SbsModalSize.Xs => "xs",
        SbsModalSize.Sm => "sm",
        SbsModalSize.Md => "md",
        SbsModalSize.Lg => "lg",
        SbsModalSize.Xl => "xl",
        SbsModalSize.Xxl => "xxl",
        SbsModalSize.Full => "full",
        _ => "md"
    };

    public static bool TryParse(string? value, out SbsModalSize size)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            size = SbsModalSize.Md;
            return false;
        }

        return Enum.TryParse(value.Trim(), ignoreCase: true, out size);
    }
}
