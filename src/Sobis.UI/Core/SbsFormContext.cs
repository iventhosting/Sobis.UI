namespace Sobis.UI;

/// <summary>
/// Cascading form context shared by SbsFormLayout and child form fields / inputs.
/// </summary>
public sealed class SbsFormContext
{
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }
    public string? LabelWidth { get; set; }
    public SbsLabelPosition LabelPosition { get; set; } = SbsLabelPosition.Left;
    public bool Compact { get; set; }
    public SbsSize Size { get; set; } = SbsSize.Md;
}
