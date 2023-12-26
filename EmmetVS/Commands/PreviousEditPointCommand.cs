using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.IO;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Previous Edit Point command.
/// </summary>
[Command(PackageIds.PreviousEditPointCommand)]
internal sealed class PreviousEditPointCommand(DIToolkitPackage package) : BaseDICommand(package)
{
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

            var syntaxType = SyntaxHelper.GetSyntaxType(activeDocumentExtension);
            if (syntaxType != FileType.Markup)
                return;

            var pos = EditPointHelper.FindNewEditPoint(docView, -1);
            if (pos is null)
                return;

            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, pos.Value));
            docView.TextView.Caret.EnsureVisible();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while going to previous edit point.");
        }
    }
}
