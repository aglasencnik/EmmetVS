using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Helpers;
using EmmetVS.Options;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Expand Abbreviation command.
/// </summary>
[Command(PackageIds.ExpandAbbreviationCommand)]
internal sealed class ExpandAbbreviationCommand : BaseDICommand
{
    private readonly IAbbreviationService _abbreviationService;

    public ExpandAbbreviationCommand(DIToolkitPackage package,
        IAbbreviationService abbreviationService) : base(package)
    {
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

            AbbreviationExpanderHelper.ExpandAbbreviation(docView, _abbreviationService);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while expanding abbreviation.");
        }
    }
}
