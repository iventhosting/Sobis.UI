using Microsoft.AspNetCore.Components;

namespace Sobis.UI;

/// <summary>
/// Server-side sort request payload raised by <see cref="SbsGrid{TItem}"/>.
/// </summary>
public sealed record SbsGridSort(string Field, bool Descending);

/// <summary>
/// Column definition for <see cref="SbsGrid{TItem}"/>.
/// Supports both imperative list usage (<c>new SbsGridColumn&lt;T&gt; { ... }</c>)
/// and declarative Razor markup (<c>&lt;SbsGridColumn TItem="T" Field="..." Caption="..." /&gt;</c>).
/// </summary>
public class SbsGridColumn<TItem> : ComponentBase, IDisposable
{
    [CascadingParameter] internal SbsGrid<TItem>? ParentGrid { get; set; }

    [Parameter] public string Caption { get; set; } = string.Empty;
    [Parameter] public string Field { get; set; } = string.Empty;
    [Parameter] public string Width { get; set; } = string.Empty;
    [Parameter] public bool AllowSort { get; set; } = true;
    [Parameter] public bool Printable { get; set; } = true;
    [Parameter] public bool StopRowClick { get; set; }
    [Parameter] public Func<TItem, object?>? ValueGetter { get; set; }
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }
    [Parameter] public RenderFragment<TItem>? Template { get; set; }
    [Parameter] public RenderFragment<TItem>? ChildContent { get; set; }
    [Parameter] public string Align { get; set; } = "left";
    [Parameter] public string FooterText { get; set; } = string.Empty;
    [Parameter] public SbsGridAggregate FooterAggregate { get; set; } = SbsGridAggregate.None;
    [Parameter] public string FooterFormat { get; set; } = string.Empty;
    [Parameter] public Func<IReadOnlyList<TItem>, object?>? FooterValueGetter { get; set; }

    private bool _hasSnapshot;
    private (string Caption, string Field, string Width, bool AllowSort, bool Printable, bool StopRowClick, string Align, string FooterText, SbsGridAggregate FooterAggregate, string FooterFormat) _lastSnapshot;

    internal RenderFragment<TItem>? EffectiveTemplate => Template ?? ChildContent;

    protected override void OnInitialized()
    {
        ParentGrid?.RegisterColumn(this);
    }

    protected override void OnParametersSet()
    {
        var current = (Caption, Field, Width, AllowSort, Printable, StopRowClick, Align, FooterText, FooterAggregate, FooterFormat);
        if (!_hasSnapshot)
        {
            _lastSnapshot = current;
            _hasSnapshot = true;
            return;
        }

        if (!EqualityComparer<(string, string, string, bool, bool, bool, string, string, SbsGridAggregate, string)>.Default.Equals(_lastSnapshot, current))
        {
            _lastSnapshot = current;
            ParentGrid?.NotifyColumnChanged();
        }
    }

    public void Dispose()
    {
        ParentGrid?.UnregisterColumn(this);
    }
}
