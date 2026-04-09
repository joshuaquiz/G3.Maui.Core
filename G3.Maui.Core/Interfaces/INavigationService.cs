using System.Threading.Tasks;

namespace G3.Maui.Core.Interfaces;

/// <summary>
/// Abstraction for navigation. Implement this interface (or extend
/// <see cref="G3.Maui.Core.Navigation.AbstractNavigationService"/>) to provide
/// app-specific navigation logic.
/// </summary>
public interface INavigationService
{
    /// <summary>Navigate back in the navigation stack.</summary>
    System.Threading.Tasks.Task GoBackAsync();

    /// <summary>Returns true if there is a page to go back to.</summary>
    bool CanGoBack();

    /// <summary>
    /// Initialize the navigation service (register routes, etc.).
    /// Called once during app startup.
    /// </summary>
    void Initialize();
}
