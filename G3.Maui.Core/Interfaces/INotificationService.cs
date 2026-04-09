using System;
using System.Threading.Tasks;

namespace G3.Maui.Core.Interfaces;

/// <summary>
/// Service for displaying user notifications (toasts, alerts, local push, etc.)
/// </summary>
public interface INotificationService
{
    /// <summary>Show a short toast message.</summary>
    Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short);

    /// <summary>Show an error message to the user.</summary>
    Task ShowErrorAsync(string message, string? title = null);

    /// <summary>Show a success message to the user.</summary>
    Task ShowSuccessAsync(string message, string? title = null);

    /// <summary>Show a warning message to the user.</summary>
    Task ShowWarningAsync(string message, string? title = null);

    /// <summary>Send a local notification with an optional click action.</summary>
    Task SendLocalNotificationAsync(
        string title,
        string message,
        int notificationId,
        NotificationClickAction? clickAction = null,
        Uri? imageUrl = null);

    /// <summary>Request notification permissions (Android 13+).</summary>
    Task<bool> RequestNotificationPermissionAsync();

    /// <summary>Check if notification permissions are granted.</summary>
    Task<bool> HasNotificationPermissionAsync();
}

/// <summary>Duration for toast messages.</summary>
public enum ToastDuration
{
    /// <summary>Short toast duration.</summary>
    Short,

    /// <summary>Long toast duration.</summary>
    Long
}

/// <summary>Action to perform when a notification is clicked.</summary>
public class NotificationClickAction
{
    /// <summary>Type of action to perform.</summary>
    public NotificationActionType ActionType { get; set; }

    /// <summary>Data associated with the action (e.g., a record ID for deep linking).</summary>
    public string? ActionData { get; set; }
}

/// <summary>Types of notification click actions.</summary>
public enum NotificationActionType
{
    /// <summary>Navigate to a record using deep linking.</summary>
    DeepLink,

    /// <summary>Show an alert dialog.</summary>
    ShowAlert
}
