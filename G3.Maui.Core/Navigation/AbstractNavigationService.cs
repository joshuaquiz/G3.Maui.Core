using System.Threading.Tasks;
using G3.Maui.Core.Interfaces;

namespace G3.Maui.Core.Navigation;

/// <summary>
/// Abstract base class for navigation services.
/// Extend this class to provide app-specific navigation logic while
/// satisfying the <see cref="INavigationService"/> contract.
/// </summary>
public abstract class AbstractNavigationService : INavigationService
{
    /// <inheritdoc/>
    public abstract Task GoBackAsync();

    /// <inheritdoc/>
    public abstract bool CanGoBack();

    /// <inheritdoc/>
    public virtual void Initialize()
    {
    }
}
