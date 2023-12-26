using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.IO;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Select Previous Item command.
/// </summary>
[Command(PackageIds.SelectPreviousItemCommand)]
internal class SelectPreviousItemCommand : BaseDICommand
{
    private readonly IActionUtilsService _actionUtilsService;

    public SelectPreviousItemCommand(DIToolkitPackage package,
        IActionUtilsService actionUtilsService) : base(package)
    {
        _actionUtilsService = actionUtilsService;
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
            if (!GeneralOptions.Instance.Enable || !GeneralOptions.Instance.EnableAdvanced)
                return;

            var docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView is null)
                return;

            var activeDocumentExtension = Path.GetExtension(DocumentHelper.GetActiveDocumentPath());
            if (string.IsNullOrWhiteSpace(activeDocumentExtension))
                return;

            var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
            if (fileType != FileType.Markup && fileType != FileType.Stylesheet)
                return;

            var selectedItem = SelectItemHelper.FindNewSelectedItem(_actionUtilsService, docView, fileType == FileType.Stylesheet, true);
            if (!selectedItem.HasValue)
                return;

            docView.TextView.Selection.Select(new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, selectedItem.Value.Item1, selectedItem.Value.Item2 - selectedItem.Value.Item1), false);
            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, selectedItem.Value.Item1));
            docView.TextView.Caret.EnsureVisible();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while selecting previous item.");
        }
    }
}
