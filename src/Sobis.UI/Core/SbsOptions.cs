namespace Sobis.UI;

/// <summary>
/// Global configuration options for Sobis.UI registered via AddSbsUI().
/// </summary>
public sealed class SbsOptions
{
    public SbsToastPosition DefaultToastPosition { get; set; } = SbsToastPosition.TopRight;
    public bool DefaultToastShowCloseButton { get; set; } = true;
    public int DefaultToastDurationMs { get; set; } = 3500;
    public string DefaultTheme { get; set; } = "dark";
    public string DefaultDateFormat { get; set; } = "dd.MM.yyyy";
    public string DefaultIconPrefix { get; set; } = "fa";
    public Dictionary<string, string>? LocalizationOverrides { get; set; }
}
