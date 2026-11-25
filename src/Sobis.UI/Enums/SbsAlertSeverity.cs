namespace Sobis.UI;

/// <summary>
/// Severity levels for SbsAlert inline notification banners.
/// </summary>
public enum SbsAlertSeverity
{
    Info,
    Success,
    Warning,
    Danger
}

public static class SbsAlertSeverityExtensions
{
    public static string ToCss(this SbsAlertSeverity severity) => severity switch
    {
        SbsAlertSeverity.Success => "success",
        SbsAlertSeverity.Warning => "warning",
        SbsAlertSeverity.Danger => "danger",
        _ => "info"
    };
}
