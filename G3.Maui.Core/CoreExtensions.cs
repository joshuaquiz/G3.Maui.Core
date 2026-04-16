using System;
using System.Net.Http;
using G3.Maui.Core.Components.CachedImage;
using G3.Maui.Core.Interfaces;
using G3.Maui.Core.Models;
using G3.Maui.Core.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Networking;

namespace G3.Maui.Core;

/// <summary>
/// Some core extensions for the project.
/// </summary>
public static class CoreExtensions
{
    private const string AndroidDeviceRootUrl = "http://10.0.2.2:7201";
    private const string NonAndroidDeviceRootUrl = "http://localhost:7201";

    /// <summary>
    /// Sets up the <see cref="HttpClient"/> for development.
    /// </summary>
    /// <remarks>
    /// <see cref="AddCoreDeviceServices"/> should be called before this method to set up the required services.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
    /// <param name="httpClientFactory">The function that sets up the custom <see cref="HttpClient"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDevelopmentHttpClient<TDelegatingHandler, THttpClient>(
        this IServiceCollection services,
        Func<IServiceProvider, BaseDelegatingHandler, Uri, THttpClient> httpClientFactory)
        where TDelegatingHandler : BaseDelegatingHandler
        where THttpClient : BaseHttpClient
    {
        services
            .AddMemoryCache()
            .AddSingleton<TDelegatingHandler>()
            .AddSingleton(
                serviceProvider =>
                    httpClientFactory(
                        serviceProvider,
                        serviceProvider.GetRequiredService<TDelegatingHandler>(),
                        new Uri(
                            serviceProvider.GetRequiredService<IDeviceInfo>().Platform == DevicePlatform.Android
                                ? AndroidDeviceRootUrl
                                : NonAndroidDeviceRootUrl,
                            UriKind.Absolute)));
        return services;
    }

    /// <summary>
    /// Registers G3.Maui.Core UI components with the MAUI application builder.
    /// This merges the default <see cref="G3CoreResources"/> token dictionary into the app
    /// resources. Call this in your MauiProgram.cs.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <param name="imageCacheService">
    /// Optional: provide your <see cref="IImageCacheService"/> implementation here so that
    /// <see cref="CachedImage"/> is initialised automatically.
    /// </param>
    /// <returns>The same <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UseG3MauiCoreUI(
        this MauiAppBuilder builder,
        IImageCacheService? imageCacheService = null)
    {
        // Merge default resource tokens so components render without client configuration
        var app = Application.Current;
        if (app != null)
        {
            app.Resources.MergedDictionaries.Add(new G3CoreResources());
        }
        else
        {
            // Fallback: merge on first window created
            builder.Services.AddSingleton<IMauiInitializeScopedService, G3ResourcesInitializer>();
        }

        // Wire up the image cache service if provided
        if (imageCacheService != null)
        {
            CachedImage.Initialize(imageCacheService);
        }

        return builder;
    }

    /// <summary>
    /// Sets up some core device services, with optional overrides.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
    /// <param name="deviceInfo">An <see cref="IDeviceInfo"/> used to override <see cref="DeviceInfo.Current"/>.</param>
    /// <param name="connectivity">An <see cref="IConnectivity"/> used to override <see cref="Connectivity.Current"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCoreDeviceServices(
        this IServiceCollection services,
        IDeviceInfo? deviceInfo = null,
        IConnectivity? connectivity = null)
    {
        services
            .AddSingleton(deviceInfo ?? DeviceInfo.Current)
            .AddSingleton(connectivity ?? Connectivity.Current);
        return services;
    }
}

/// <summary>
/// Fallback initializer that merges <see cref="G3CoreResources"/> when the application
/// is not yet running at builder configuration time.
/// </summary>
internal sealed class G3ResourcesInitializer : IMauiInitializeScopedService
{
    public void Initialize(IServiceProvider services)
    {
        if (Application.Current != null)
        {
            Application.Current.Resources.MergedDictionaries.Add(new G3CoreResources());
        }
    }
}