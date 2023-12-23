global using Community.VisualStudio.Toolkit.DependencyInjection.Microsoft;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using Community.VisualStudio.Toolkit;
using EmmetNetSharp.Interfaces;
using EmmetNetSharp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace EmmetVS;

/// <summary>
/// Represents the Visual Studio package.
/// </summary>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.EmmetVSString)]
[ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
public sealed class EmmetVSPackage : MicrosoftDIToolkitPackage<EmmetVSPackage>
{
    /// <summary>
    /// Initializes services and registers them to the service collection.
    /// This method should be overridden to add custom services to the application.
    /// </summary>
    /// <param name="services">The service collection to which services are registered.</param>
    protected override void InitializeServices(IServiceCollection services)
    {
        base.InitializeServices(services);

        services.AddSingleton<IAbbreviationService, AbbreviationService>();
        services.AddSingleton<IActionUtilsService, ActionUtilsService>();
        services.AddSingleton<ICssMatcherService, CssMatcherService>();
        services.AddSingleton<IHtmlMatcherService, HtmlMatcherService>();
        services.AddSingleton<IMathExpressionService, MathExpressionService>();
        services.AddSingleton<IScannerService, ScannerService>();

        services.RegisterCommands(ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Initializes the package asynchronously.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken object</param>
    /// <param name="progress">ServiceProgressData object</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);
    }
}
