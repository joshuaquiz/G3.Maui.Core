using System.Windows.Input;

namespace G3.Maui.Core.Models;

/// <summary>
/// Represents a command icon in the custom navigation bar.
/// </summary>
public class NavBarCommand
{
    /// <summary>The icon source (e.g., "gear", "edit").</summary>
    public string IconSource { get; set; } = string.Empty;

    /// <summary>The display name shown in the overflow action sheet.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>The command to execute when the icon is tapped.</summary>
    public ICommand? Command { get; set; }

    /// <summary>Optional command parameter.</summary>
    public object? CommandParameter { get; set; }
}
