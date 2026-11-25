using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sobis.UI;

/// <summary>
/// Dependency injection registration helpers for Sobis.UI.
/// </summary>
public static class SbsServiceCollectionExtensions
{
    /// <summary>
    /// Registers core Sobis.UI services (toast, confirm, dialog, localizer, authorization provider).
    /// Uses TryAdd* so host applications can override any service before or after calling AddSbsUI().
    /// </summary>
    public static IServiceCollection AddSbsUI(
        this IServiceCollection services,
        Action<SbsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new SbsOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.TryAddScoped<ISbsAuthorizationProvider, AllowAllSbsAuthorizationProvider>();
        services.TryAddScoped<ISbsLocalizer, DefaultSbsLocalizer>();
        services.TryAddScoped<ISbsToastService, SbsToastService>();
        services.TryAddScoped<ISbsConfirmService, SbsConfirmService>();
        services.TryAddScoped<ISbsDialogService, SbsDialogService>();

        return services;
    }
}
