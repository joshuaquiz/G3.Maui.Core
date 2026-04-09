using System;
using System.Windows.Input;
using G3.Maui.Core.Enums;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace G3.Maui.Core.Components.EditableText;

/// <summary>
/// A text field that supports both view and edit modes, with optional validation,
/// character count, multiline (editor), and keyboard type configuration.
/// Bind <see cref="Text"/> two-way, toggle <see cref="IsEditMode"/> to switch modes.
/// Subscribe to <see cref="TextChanged"/> to react to input, <see cref="EntryFocused"/> /
/// <see cref="EntryUnfocused"/> to respond to keyboard show/hide, and set
/// <see cref="ReturnType"/> / <see cref="ReturnCommand"/> to control the keyboard action button.
/// Call <see cref="ClearValidation"/> to reset error state programmatically.
/// </summary>
public partial class EditableTextComponent : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(EditableTextComponent), string.Empty,
            BindingMode.TwoWay, propertyChanged: OnTextChanged);

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(EditableTextComponent), string.Empty);

    public static readonly BindableProperty ShowLabelProperty =
        BindableProperty.Create(nameof(ShowLabel), typeof(bool), typeof(EditableTextComponent), true);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EditableTextComponent), string.Empty);

    public static readonly BindableProperty IsEditModeProperty =
        BindableProperty.Create(nameof(IsEditMode), typeof(bool), typeof(EditableTextComponent), false,
            propertyChanged: OnIsEditModeChanged);

    public static readonly BindableProperty IsViewModeProperty =
        BindableProperty.Create(nameof(IsViewMode), typeof(bool), typeof(EditableTextComponent), true);

    public static readonly BindableProperty IsMultilineProperty =
        BindableProperty.Create(nameof(IsMultiline), typeof(bool), typeof(EditableTextComponent), false,
            propertyChanged: OnIsMultilineChanged);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(EditableTextComponent), 14.0);

    public static readonly BindableProperty FontAttributesProperty =
        BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(EditableTextComponent), FontAttributes.None);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(EditableTextComponent));

    public static readonly BindableProperty LabelFontSizeProperty =
        BindableProperty.Create(nameof(LabelFontSize), typeof(double), typeof(EditableTextComponent), 14.0);

    public static readonly BindableProperty LabelFontAttributesProperty =
        BindableProperty.Create(nameof(LabelFontAttributes), typeof(FontAttributes), typeof(EditableTextComponent), FontAttributes.Bold);

    public static readonly BindableProperty LabelTextColorProperty =
        BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(EditableTextComponent));

    public static readonly BindableProperty HorizontalTextAlignmentProperty =
        BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(Microsoft.Maui.TextAlignment), typeof(EditableTextComponent), Microsoft.Maui.TextAlignment.Start);

    public static readonly BindableProperty VerticalTextAlignmentProperty =
        BindableProperty.Create(nameof(VerticalTextAlignment), typeof(Microsoft.Maui.TextAlignment), typeof(EditableTextComponent), Microsoft.Maui.TextAlignment.Start);

    public static readonly BindableProperty LineBreakModeProperty =
        BindableProperty.Create(nameof(LineBreakMode), typeof(Microsoft.Maui.LineBreakMode), typeof(EditableTextComponent), Microsoft.Maui.LineBreakMode.WordWrap);

    public static readonly BindableProperty MaxLinesProperty =
        BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(EditableTextComponent), int.MaxValue);

    public static new readonly BindableProperty PaddingProperty =
        BindableProperty.Create(nameof(Padding), typeof(Microsoft.Maui.Thickness), typeof(EditableTextComponent), new Microsoft.Maui.Thickness(0));

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(EditableTextComponent), int.MaxValue,
            propertyChanged: OnMaxLengthChanged);

    public static readonly BindableProperty MinLengthProperty =
        BindableProperty.Create(nameof(MinLength), typeof(int), typeof(EditableTextComponent), 0);

    public static readonly BindableProperty IsRequiredProperty =
        BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(EditableTextComponent), false);

    public static readonly BindableProperty ShowCharacterCountProperty =
        BindableProperty.Create(nameof(ShowCharacterCount), typeof(bool), typeof(EditableTextComponent), false,
            propertyChanged: OnShowCharacterCountChanged);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Microsoft.Maui.Keyboard), typeof(EditableTextComponent), Microsoft.Maui.Keyboard.Default);

    public static readonly BindableProperty KeyboardTypeProperty =
        BindableProperty.Create(nameof(KeyboardType), typeof(KeyboardType), typeof(EditableTextComponent), KeyboardType.Default,
            propertyChanged: OnKeyboardTypeChanged);

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(EditableTextComponent), false);

    public static readonly BindableProperty EditorMinHeightProperty =
        BindableProperty.Create(nameof(EditorMinHeight), typeof(double), typeof(EditableTextComponent), 100.0);

    public static readonly BindableProperty EntryAutomationIdProperty =
        BindableProperty.Create(nameof(EntryAutomationId), typeof(string), typeof(EditableTextComponent));

    public static readonly BindableProperty DisplayLabelAutomationIdProperty =
        BindableProperty.Create(nameof(DisplayLabelAutomationId), typeof(string), typeof(EditableTextComponent));

    public static readonly BindableProperty ReturnTypeProperty =
        BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(EditableTextComponent), ReturnType.Default,
            propertyChanged: OnReturnTypeChanged);

    public static readonly BindableProperty ReturnCommandProperty =
        BindableProperty.Create(nameof(ReturnCommand), typeof(ICommand), typeof(EditableTextComponent), null,
            propertyChanged: OnReturnCommandChanged);

    public static readonly BindableProperty ReturnCommandParameterProperty =
        BindableProperty.Create(nameof(ReturnCommandParameter), typeof(object), typeof(EditableTextComponent), null,
            propertyChanged: OnReturnCommandParameterChanged);

    #endregion

    #region Properties

    public string? Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    public bool IsViewMode
    {
        get => (bool)GetValue(IsViewModeProperty);
        private set => SetValue(IsViewModeProperty, value);
    }

    public bool IsMultiline
    {
        get => (bool)GetValue(IsMultilineProperty);
        set => SetValue(IsMultilineProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public double LabelFontSize
    {
        get => (double)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    public FontAttributes LabelFontAttributes
    {
        get => (FontAttributes)GetValue(LabelFontAttributesProperty);
        set => SetValue(LabelFontAttributesProperty, value);
    }

    public Color LabelTextColor
    {
        get => (Color)GetValue(LabelTextColorProperty);
        set => SetValue(LabelTextColorProperty, value);
    }

    public Microsoft.Maui.TextAlignment HorizontalTextAlignment
    {
        get => (Microsoft.Maui.TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public Microsoft.Maui.TextAlignment VerticalTextAlignment
    {
        get => (Microsoft.Maui.TextAlignment)GetValue(VerticalTextAlignmentProperty);
        set => SetValue(VerticalTextAlignmentProperty, value);
    }

    public Microsoft.Maui.LineBreakMode LineBreakMode
    {
        get => (Microsoft.Maui.LineBreakMode)GetValue(LineBreakModeProperty);
        set => SetValue(LineBreakModeProperty, value);
    }

    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    public new Microsoft.Maui.Thickness Padding
    {
        get => (Microsoft.Maui.Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public int MinLength
    {
        get => (int)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public bool ShowCharacterCount
    {
        get => (bool)GetValue(ShowCharacterCountProperty);
        set => SetValue(ShowCharacterCountProperty, value);
    }

    public Microsoft.Maui.Keyboard Keyboard
    {
        get => (Microsoft.Maui.Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public KeyboardType KeyboardType
    {
        get => (KeyboardType)GetValue(KeyboardTypeProperty);
        set => SetValue(KeyboardTypeProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public double EditorMinHeight
    {
        get => (double)GetValue(EditorMinHeightProperty);
        set => SetValue(EditorMinHeightProperty, value);
    }

    public string EntryAutomationId
    {
        get => (string)GetValue(EntryAutomationIdProperty);
        set => SetValue(EntryAutomationIdProperty, value);
    }

    public string DisplayLabelAutomationId
    {
        get => (string)GetValue(DisplayLabelAutomationIdProperty);
        set => SetValue(DisplayLabelAutomationIdProperty, value);
    }

    /// <summary>
    /// The keyboard "Return" button label/behavior. Defaults to <see cref="ReturnType.Default"/>.
    /// Only applies to single-line mode; has no effect when <see cref="IsMultiline"/> is true.
    /// </summary>
    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    /// <summary>
    /// Command executed when the user taps the keyboard Return button in single-line mode.
    /// Only applies when <see cref="IsMultiline"/> is false.
    /// </summary>
    public ICommand? ReturnCommand
    {
        get => (ICommand?)GetValue(ReturnCommandProperty);
        set => SetValue(ReturnCommandProperty, value);
    }

    /// <summary>Optional parameter passed to <see cref="ReturnCommand"/> on execution.</summary>
    public object? ReturnCommandParameter
    {
        get => GetValue(ReturnCommandParameterProperty);
        set => SetValue(ReturnCommandParameterProperty, value);
    }

    public bool IsValid => string.IsNullOrEmpty(ValidationError);
    public string? ValidationError { get; private set; }

    #endregion

    #region Events

    /// <summary>
    /// Raised whenever the text value changes. The string argument is the new value.
    /// Useful when you need to react to input without two-way binding.
    /// </summary>
    public event EventHandler<string>? TextChanged;

    /// <summary>
    /// Raised when the underlying entry or editor gains focus (keyboard appears).
    /// Use this to scroll the active field into view.
    /// </summary>
    public event EventHandler<FocusEventArgs>? EntryFocused;

    /// <summary>
    /// Raised when the underlying entry or editor loses focus (keyboard dismisses).
    /// </summary>
    public event EventHandler<FocusEventArgs>? EntryUnfocused;

    #endregion

    public EditableTextComponent()
    {
        InitializeComponent();
        TextEntry.TextChanged += (_, e) => Text = e.NewTextValue;
        TextEditor.TextChanged += (_, e) => Text = e.NewTextValue;
        TextEntry.Focused += (_, e) => EntryFocused?.Invoke(this, e);
        TextEntry.Unfocused += (_, e) => EntryUnfocused?.Invoke(this, e);
        TextEditor.Focused += (_, e) => EntryFocused?.Invoke(this, e);
        TextEditor.Unfocused += (_, e) => EntryUnfocused?.Invoke(this, e);
        UpdateUI();
    }

    private static void OnIsEditModeChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTextComponent)?.UpdateUI();

    private static void OnIsMultilineChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTextComponent)?.UpdateUI();

    private static void OnTextChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTextComponent c)
        {
            c.Validate();
            c.UpdateCharacterCount();
            c.TextChanged?.Invoke(c, n as string ?? string.Empty);
        }
    }

    private static void OnMaxLengthChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTextComponent)?.UpdateCharacterCount();

    private static void OnShowCharacterCountChanged(
        BindableObject b,
        object o,
        object n)
        => (b as EditableTextComponent)?.UpdateCharacterCount();

    private static void OnKeyboardTypeChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTextComponent c && n is KeyboardType kt)
        {
            c.Keyboard = kt switch
            {
                KeyboardType.Url => Microsoft.Maui.Keyboard.Url,
                KeyboardType.CapitalizeSentence => Microsoft.Maui.Keyboard.Create(Microsoft.Maui.KeyboardFlags.CapitalizeSentence | Microsoft.Maui.KeyboardFlags.Spellcheck | Microsoft.Maui.KeyboardFlags.Suggestions),
                KeyboardType.CapitalizeWord => Microsoft.Maui.Keyboard.Create(Microsoft.Maui.KeyboardFlags.CapitalizeWord | Microsoft.Maui.KeyboardFlags.Spellcheck | Microsoft.Maui.KeyboardFlags.Suggestions),
                _ => Microsoft.Maui.Keyboard.Default
            };
        }
    }

    private static void OnReturnTypeChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTextComponent c && n is ReturnType rt)
        {
            c.TextEntry.ReturnType = rt;
        }
    }

    private static void OnReturnCommandChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTextComponent c)
        {
            c.TextEntry.ReturnCommand = n as ICommand;
        }
    }

    private static void OnReturnCommandParameterChanged(
        BindableObject b,
        object o,
        object n)
    {
        if (b is EditableTextComponent c)
        {
            c.TextEntry.ReturnCommandParameter = n;
        }
    }

    private void UpdateUI()
    {
        if (IsEditMode)
        {
            IsViewMode = false;
            DisplayLabel.IsVisible = false;
            if (IsMultiline)
            {
                TextEntry.IsVisible = false;
                TextEditor.IsVisible = true;
            }
            else
            {
                TextEntry.IsVisible = true;
                TextEditor.IsVisible = false;
            }
        }
        else
        {
            IsViewMode = true;
            DisplayLabel.IsVisible = true;
            TextEntry.IsVisible = false;
            TextEditor.IsVisible = false;
        }
        UpdateCharacterCount();
    }

    private void Validate()
    {
        ValidationError = null;
        if (IsRequired && string.IsNullOrWhiteSpace(Text))
        {
            ValidationError = $"{LabelText} is required";
        }
        else if (MinLength > 0 && (Text?.Length ?? 0) < MinLength)
        {
            ValidationError = $"{LabelText} must be at least {MinLength} characters";
        }
        else if (MaxLength < int.MaxValue && Text != null && Text.Length > MaxLength)
        {
            ValidationError = $"{LabelText} must be at most {MaxLength} characters";
        }

        ValidationErrorLabel.Text = ValidationError ?? string.Empty;
        ValidationErrorLabel.IsVisible = !string.IsNullOrEmpty(ValidationError) && IsEditMode;
    }

    private void UpdateCharacterCount()
    {
        if (ShowCharacterCount && IsEditMode && MaxLength < int.MaxValue)
        {
            CharacterCountLabel.Text = $"{Text?.Length ?? 0}/{MaxLength}";
            CharacterCountLabel.IsVisible = true;
        }
        else
        {
            CharacterCountLabel.IsVisible = false;
        }
    }

    public void ClearValidation()
    {
        ValidationError = null;
        ValidationErrorLabel.IsVisible = false;
    }
}
