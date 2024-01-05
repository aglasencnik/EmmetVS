using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Helpers;
using EmmetVS.Options;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Wrap With Abbreviation command.
/// </summary>
[Command(PackageIds.WrapWithAbbreviationCommand)]
internal sealed class WrapWithAbbreviationCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly IAbbreviationService _abbreviationService;

    public WrapWithAbbreviationCommand(DIToolkitPackage package,
        IHtmlMatcherService htmlMatcherService,
        IAbbreviationService abbreviationService) : base(package)
    {
        _htmlMatcherService = htmlMatcherService;
        _abbreviationService = abbreviationService;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="e">OleMenuCmdEventArgs object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        try
        {
            if (!GeneralOptions.Instance.Enable)
                return;

            var docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView is null)
                return;

            AbbreviationExpanderHelper.WrapWithAbbreviation(docView, _htmlMatcherService, _abbreviationService);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while wrapping with abbreviation.");
        }
    }
}
