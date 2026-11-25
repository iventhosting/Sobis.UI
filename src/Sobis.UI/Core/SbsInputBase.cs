using System.Collections;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Sobis.UI;

/// <summary>
/// Base class for all Sobis.UI form input components.
/// Centralizes value binding, cascading required/editable field rules, form-level context,
/// and optional Blazor EditForm validation integration.
/// </summary>
public abstract class SbsInputBase<TValue> : SbsComponentBase, IDisposable
{
    private EditContext? _subscribedEditContext;
    private bool _disposed;

    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }
    [Parameter] public string? PropertyName { get; set; }

    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? HelpText { get; set; }
    [Parameter] public string? ErrorText { get; set; }

    [CascadingParameter(Name = "CascadedRequiredFields")]
    public HashSet<string>? CascadedRequiredFields { get; set; }

    [CascadingParameter(Name = "CascadedEditableFields")]
    public HashSet<string>? CascadedEditableFields { get; set; }

    [CascadingParameter]
    public SbsFormContext? FormContext { get; set; }

    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    protected FieldIdentifier? FieldIdentifier { get; private set; }

    /// <summary>
    /// Resolves the logical field name from <see cref="PropertyName"/> or <see cref="ValueExpression"/>.
    /// </summary>
    protected virtual string? ActualPropertyName
        => !string.IsNullOrWhiteSpace(PropertyName)
            ? PropertyName
            : ExtractPropertyName(ValueExpression);

    /// <summary>
    /// True when <see cref="Required"/> is set or <see cref="ActualPropertyName"/> is in <see cref="CascadedRequiredFields"/>.
    /// </summary>
    protected virtual bool IsRequired
        => Required || (CascadedRequiredFields is not null && ActualPropertyName is not null && CascadedRequiredFields.Contains(ActualPropertyName));

    /// <summary>
    /// True when disabled directly, by <see cref="FormContext"/>, or excluded from <see cref="CascadedEditableFields"/>.
    /// </summary>
    protected virtual bool EffectiveDisabled
        => Disabled
           || (FormContext?.Disabled ?? false)
           || (CascadedEditableFields is { Count: > 0 } && (ActualPropertyName is null || !CascadedEditableFields.Contains(ActualPropertyName)));

    /// <summary>
    /// True when read-only directly or by <see cref="FormContext"/>.
    /// </summary>
    protected virtual bool EffectiveReadOnly
        => ReadOnly || (FormContext?.ReadOnly ?? false);

    /// <summary>
    /// True when the associated Blazor <see cref="EditContext"/> has validation errors for this field.
    /// </summary>
    protected bool HasEditContextErrors
        => EditContext is not null && FieldIdentifier is { } fi && EditContext.GetValidationMessages(fi).Any();

    /// <summary>
    /// Returns explicit <see cref="ErrorText"/> or the first validation error from <see cref="EditContext"/>.
    /// </summary>
    protected string? EffectiveErrorText
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ErrorText))
                return ErrorText;

            if (EditContext is not null && FieldIdentifier is { } fi)
            {
                foreach (var message in EditContext.GetValidationMessages(fi))
                {
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// True when explicit error text exists, EditContext has errors, or the field is required and empty.
    /// </summary>
    protected virtual bool IsInvalid
        => !string.IsNullOrWhiteSpace(ErrorText)
           || HasEditContextErrors
           || (IsRequired && !HasValue());

    /// <summary>
    /// Determines whether the input currently holds a non-empty value.
    /// Override in subclasses that maintain a buffered value or custom emptiness semantics.
    /// </summary>
    protected virtual bool HasValue()
    {
        if (Value is null)
            return false;

        if (Value is string s)
            return !string.IsNullOrWhiteSpace(s);

        if (Value is ICollection collection)
            return collection.Count > 0;

        return true;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!ReferenceEquals(_subscribedEditContext, EditContext))
        {
            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged -= OnEditContextValidationStateChanged;

            _subscribedEditContext = EditContext;

            if (_subscribedEditContext is not null)
                _subscribedEditContext.OnValidationStateChanged += OnEditContextValidationStateChanged;
        }

        if (EditContext is not null && ValueExpression is not null)
        {
            try
            {
                FieldIdentifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValueExpression);
            }
            catch
            {
                FieldIdentifier = null;
            }
        }
        else
        {
            FieldIdentifier = null;
        }
    }

    /// <summary>
    /// Invokes <see cref="ValueChanged"/> and notifies <see cref="EditContext"/> when present.
    /// </summary>
    protected async Task NotifyValueChangedAsync(TValue? value)
    {
        Value = value;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);

        NotifyFieldChanged();
    }

    /// <summary>
    /// Notifies <see cref="EditContext"/> that this field's value changed.
    /// </summary>
    protected void NotifyFieldChanged()
    {
        if (EditContext is not null && FieldIdentifier is { } fi)
            EditContext.NotifyFieldChanged(fi);
    }

    protected static string? ExtractPropertyName(LambdaExpression? expression)
    {
        if (expression is null)
            return null;

        return expression.Body switch
        {
            MemberExpression me => me.Member.Name,
            UnaryExpression { Operand: MemberExpression ue } => ue.Member.Name,
            _ => null
        };
    }

    private void OnEditContextValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing && _subscribedEditContext is not null)
        {
            _subscribedEditContext.OnValidationStateChanged -= OnEditContextValidationStateChanged;
            _subscribedEditContext = null;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
