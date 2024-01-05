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
using EmmetVS.Options;
using EmmetVS.Options.CustomOptionPages;
using System.ComponentModel.Design;
using System.ComponentModel;
using EmmetVS.Helpers;

namespace EmmetVS;

/// <summary>
/// Represents the Visual Studio package.
/// </summary>
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.EmmetVSString)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralOptionsProvider), "EmmetVS", "General", 0, 0, true)]
[ProvideProfile(typeof(OptionsProvider.GeneralOptionsProvider), "EmmetVS", "General", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.HtmlSnippetsOptionsPage), "EmmetVS\\HTML", "Snippets", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.HtmlSnippetsOptionsPage), "EmmetVS\\HTML", "Snippets", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.HtmlSupportedFileTypesOptionsPage), "EmmetVS\\HTML", "Supported File Types", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.HtmlSupportedFileTypesOptionsPage), "EmmetVS\\HTML", "Supported File Types", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.CssSnippetsOptionsPage), "EmmetVS\\CSS", "Snippets", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.CssSnippetsOptionsPage), "EmmetVS\\CSS", "Snippets", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.CssSupportedFileTypesOptionsPage), "EmmetVS\\CSS", "Supported File Types", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.CssSupportedFileTypesOptionsPage), "EmmetVS\\CSS", "Supported File Types", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.XslSnippetsOptionsPage), "EmmetVS\\XSL", "Snippets", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.XslSnippetsOptionsPage), "EmmetVS\\XSL", "Snippets", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.XslSupportedFileTypesOptionsPage), "EmmetVS\\XSL", "Supported File Types", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.XslSupportedFileTypesOptionsPage), "EmmetVS\\XSL", "Supported File Types", 0, 0, true)]
[ProvideOptionPage(typeof(CustomOptionsPageProvider.VariableOptionsPage), "EmmetVS", "Variables", 0, 0, true)]
[ProvideProfile(typeof(CustomOptionsPageProvider.VariableOptionsPage), "EmmetVS", "Variables", 0, 0, true)]
[ProvideOptionPage(typeof(OptionsProvider.ConfigurationOptionsProvider), "EmmetVS", "Configuration", 0, 0, true)]
[ProvideProfile(typeof(OptionsProvider.ConfigurationOptionsProvider), "EmmetVS", "Configuration", 0, 0, true)]
[ProvideProfile(typeof(OptionsProvider.RuntimeOptionsProvider), "EmmetVS", "RuntimeOptions", 0, 0, true)]
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
        try
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
        catch (Exception ex)
        {
            ex.Log("Error while initializing services.");
        }
    }

    /// <summary>
    /// Initializes the package asynchronously.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken object</param>
    /// <param name="progress">ServiceProgressData object</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        try
        {
            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ExtensionInitializationHelper.CheckDefaultInstallationOptionsAsync();

            var commandService = await this.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
            ExtensionInitializationHelper.CheckCommandsAvailability(commandService);

            GeneralOptions.Instance.PropertyChanged += GeneralOptions_PropertyChanged;

            GeneralOptions.Instance.SnippetsChanged += GeneralOptions_SnippetsChanged;
            CssOptions.Instance.SnippetsChanged += GeneralOptions_SnippetsChanged;
            HtmlOptions.Instance.SnippetsChanged += GeneralOptions_SnippetsChanged;
            XslOptions.Instance.SnippetsChanged += GeneralOptions_SnippetsChanged;

            SnippetsHelper.RemovePlaceholderSnippets();
            SnippetsHelper.UpdateSnippets();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while initializing package.");
        }
    }

    /// <summary>
    /// Handles the general options property changed event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">PropertyChangedEventArgs object</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Event handler requires async void.")]
    private async void GeneralOptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(GeneralOptions.Instance.Enable) || e.PropertyName == nameof(GeneralOptions.Instance.EnableAdvanced))
            {
                var commandService = await this.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
                ExtensionInitializationHelper.CheckCommandsAvailability(commandService);
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while handling general options save.");
        }
    }

    /// <summary>
    /// Handles the general options property changed event for snippets.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">PropertyChangedEventArgs object</param>
    private void GeneralOptions_SnippetsChanged(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(GeneralOptions.Instance.EnableSnippets) ||
                e.PropertyName == nameof(CssOptions.Instance.Snippets) ||
                e.PropertyName == nameof(HtmlOptions.Instance.Snippets) ||
                e.PropertyName == nameof(XslOptions.Instance.Snippets))
            {
                SnippetsHelper.UpdateSnippets();
            }
        }
        catch (Exception ex)
        {
            ex.Log("Error while handling snippets save.");
        }
    }
}
