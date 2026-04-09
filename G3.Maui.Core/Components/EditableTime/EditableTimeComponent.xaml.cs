using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace G3.Maui.Core.Components.EditableTime;

/// <summary>
/// An editable time picker component that displays a human-readable time in view mode
/// and switches to two pickers (hours and minutes) in edit mode.
/// Bind <see cref="TimeValue"/> two-way to a nullable <see cref="TimeSpan"/> in your view model.
/// Toggle <see cref="IsEditMode"/> to switch between display and edit states.
/// </summary>
public partial class EditableTimeComponent : ContentView
{
    #region Bindable Properties

    /// <summary>Bindable property for <see cref="TimeValue"/>.</summary>
    public static readonly BindableProperty TimeValueProperty =
        BindableProperty.Create(nameof(TimeValue), typeof(TimeSpan?), typeof(EditableTimeComponent), null,
            BindingMode.TwoWay, propertyChanged: OnTimeValueChanged);

    /// <summary>Bindable property for <see cref="LabelText"/>.</summary>
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(EditableTimeComponent), string.Empty);

    /// <summary>Bindable property for <see cref="ShowLabel"/>.</summary>
    public static readonly BindableProperty ShowLabelProperty =
        BindableProperty.Create(nameof(ShowLabel), typeof(bool), typeof(EditableTimeComponent), false);

    /// <summary>Bindable property for <see cref="ShowInlineLabel"/>.</summary>
    public static readonly BindableProperty ShowInlineLabelProperty =
        BindableProperty.Create(nameof(ShowInlineLabel), typeof(bool), typeof(EditableTimeComponent), true);

    /// <summary>Bindable property for <see cref="IsEditMode"/>.</summary>
    public static readonly BindableProperty IsEditModeProperty =
        BindableProperty.Create(nameof(IsEditMode), typeof(bool), typeof(EditableTimeComponent), false,
            propertyChanged: OnIsEditModeChanged);

    /// <summary>Bindable property for <see cref="IsViewMode"/>.</summary>
    public static readonly BindableProperty IsViewModeProperty =
        BindableProperty.Create(nameof(IsViewMode), typeof(bool), typeof(EditableTimeComponent), true);

    /// <summary>Bindable property for <see cref="FontSize"/>.</summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(EditableTimeComponent), 18.0);

    /// <summary>Bindable property for <see cref="TextColor"/>.</summary>
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(EditableTimeComponent));

    /// <summary>Bindable property for <see cref="LabelFontSize"/>.</summary>
    public static readonly BindableProperty LabelFontSizeProperty =
        BindableProperty.Create(nameof(LabelFontSize), typeof(double), typeof(EditableTimeComponent), 14.0);

    /// <summary>Bindable property for <see cref="LabelFontAttributes"/>.</summary>
    public static readonly BindableProperty LabelFontAttributesProperty =
        BindableProperty.Create(nameof(LabelFontAttributes), typeof(FontAttributes), typeof(EditableTimeComponent), FontAttributes.Bold);

    /// <summary>Bindable property for <see cref="LabelTextColor"/>.</summary>
    public static readonly BindableProperty LabelTextColorProperty =
        BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(EditableTimeComponent));

    /// <summary>Bindable property for <see cref="HoursPickerAutomationId"/>.</summary>
    public static readonly BindableProperty HoursPickerAutomationIdProperty =
        BindableProperty.Create(nameof(HoursPickerAutomationId), typeof(string), typeof(EditableTimeComponent), null);

    /// <summary>Bindable property for <see cref="MinutesPickerAutomationId"/>.</summary>
    public static readonly BindableProperty MinutesPickerAutomationIdProperty =
        BindableProperty.Create(nameof(MinutesPickerAutomationId), typeof(string), typeof(EditableTimeComponent), null);

    /// <summary>Bindable property for <see cref="SelectedHourIndex"/>.</summary>
    public static readonly BindableProperty SelectedHourIndexProperty =
        BindableProperty.Create(nameof(SelectedHourIndex), typeof(int), typeof(EditableTimeComponent), 0,
            BindingMode.TwoWay, propertyChanged: OnPickerChanged);

    /// <summary>Bindable property for <see cref="SelectedMinuteIndex"/>.</summary>
    public static readonly BindableProperty SelectedMinuteIndexProperty =
        BindableProperty.Create(nameof(SelectedMinuteIndex), typeof(int), typeof(EditableTimeComponent), 0,
            BindingMode.TwoWay, propertyChanged: OnPickerChanged);

    #endregion

    #region Properties

    /// <summary>The selected time. Bind this two-way to a nullable TimeSpan in your view model.</summary>
    public TimeSpan? TimeValue
    {
        get => (TimeSpan?)GetValue(TimeValueProperty);
        set => SetValue(TimeValueProperty, value);
    }

    /// <summary>Label text shown above the component when <see cref="ShowLabel"/> is true.</summary>
    public string? LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    /// <summary>When true, shows the label above the component. Defaults to false.</summary>
    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>When true, shows an inline label alongside the time display. Defaults to true.</summary>
    public bool ShowInlineLabel
    {
        get => (bool)GetValue(ShowInlineLabelProperty);
        set => SetValue(ShowInlineLabelProperty, value);
    }

    /// <summary>When true, the pickers are visible for editing. When false, the formatted time label is shown.</summary>
    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    /// <summary>Inverse of <see cref="IsEditMode"/>. Updated automatically.</summary>
    public bool IsViewMode
    {
        get => (bool)GetValue(IsViewModeProperty);
        set => SetValue(IsViewModeProperty, value);
    }

    /// <summary>Font size for the time display label. Defaults to 18.</summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>Text color for the time display label.</summary>
    public Color? TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>Font size for the field label. Defaults to 14.</summary>
    public double LabelFontSize
    {
        get => (double)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    /// <summary>Font attributes for the field label. Defaults to Bold.</summary>
    public FontAttributes LabelFontAttributes
    {
        get => (FontAttributes)GetValue(LabelFontAttributesProperty);
        set => SetValue(LabelFontAttributesProperty, value);
    }

    /// <summary>Text color for the field label.</summary>
    public Color? LabelTextColor
    {
        get => (Color)GetValue(LabelTextColorProperty);
        set => SetValue(LabelTextColorProperty, value);
    }

    /// <summary>AutomationId for the hours picker (for UI testing).</summary>
    public string? HoursPickerAutomationId
    {
        get => (string?)GetValue(HoursPickerAutomationIdProperty);
        set => SetValue(HoursPickerAutomationIdProperty, value);
    }

    /// <summary>AutomationId for the minutes picker (for UI testing).</summary>
    public string? MinutesPickerAutomationId
    {
        get => (string?)GetValue(MinutesPickerAutomationIdProperty);
        set => SetValue(MinutesPickerAutomationIdProperty, value);
    }

    /// <summary>Zero-based index into the hours list (0-24).</summary>
    public int SelectedHourIndex
    {
        get => (int)GetValue(SelectedHourIndexProperty);
        set => SetValue(SelectedHourIndexProperty, value);
    }

    /// <summary>Zero-based index into the minutes list (each step represents 5 minutes).</summary>
    public int SelectedMinuteIndex
    {
        get => (int)GetValue(SelectedMinuteIndexProperty);
        set => SetValue(SelectedMinuteIndexProperty, value);
    }

    #endregion

    /// <summary>Initializes a new instance of <see cref="EditableTimeComponent"/>.</summary>
    public EditableTimeComponent()
    {
        InitializeComponent();
    }

    private static void OnTimeValueChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTimeComponent)?.UpdatePickersFromTimeValue();

    private static void OnIsEditModeChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTimeComponent)?.UpdateUI();

    private static void OnPickerChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTimeComponent { IsEditMode: true } c)
        {
            c.UpdateTimeValueFromPickers();
        }
    }

    private void UpdateUI()
    {
        IsViewMode = !IsEditMode;
        ViewModeDisplay.IsVisible = !IsEditMode;
        EditModeDisplay.IsVisible = IsEditMode;
    }

    private void UpdatePickersFromTimeValue()
    {
        if (TimeValue.HasValue)
        {
            SelectedHourIndex = Math.Min((int)TimeValue.Value.TotalHours, 24);
            SelectedMinuteIndex = (int)(Math.Round(TimeValue.Value.Minutes / 5.0) * 5) / 5;
        }
        else
        {
            SelectedHourIndex = 0;
            SelectedMinuteIndex = 0;
        }
    }

    private void UpdateTimeValueFromPickers()
    {
        TimeValue = new TimeSpan(SelectedHourIndex, SelectedMinuteIndex * 5, 0);
    }
}
