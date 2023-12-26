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
/// Represents the Remove Tag command.
/// </summary>
[Command(PackageIds.RemoveTagCommand)]
internal sealed class RemoveTagCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;

    public RemoveTagCommand(DIToolkitPackage package,
        IHtmlMatcherService htmlMatcherService) : base(package)
    {
        _htmlMatcherService = htmlMatcherService;
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

            var position = docView.TextView.Caret.Position.BufferPosition.Position;
            var documentContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
            var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
            if (fileType != FileType.Markup)
                return;

            if (position == 0)
                position = 1;
            else if (position == documentContent.Length)
                position--;

            var tag = _htmlMatcherService.Match(documentContent, position);
            if (tag is null)
                return;

            using var edit = docView.TextBuffer.CreateEdit();

            if (!tag.ClosingTagRange.HasValue)
            {
                var lineStart = documentContent.LastIndexOf('\n', Math.Max(0, tag.OpeningTagRange.Item1 - 1));
                var lineEnd = documentContent.IndexOf('\n', tag.OpeningTagRange.Item2);
                if (lineEnd == -1)
                    lineEnd = documentContent.Length;

                var lineContentBeforeTag = documentContent.Substring(lineStart + 1, tag.OpeningTagRange.Item1 - lineStart - 1);
                var lineContentAfterTag = documentContent.Substring(tag.OpeningTagRange.Item2, lineEnd - tag.OpeningTagRange.Item2);

                if (string.IsNullOrWhiteSpace(lineContentBeforeTag) && string.IsNullOrWhiteSpace(lineContentAfterTag))
                    edit.Delete(new Span(lineStart == -1 ? 0 : lineStart, lineEnd - (lineStart == -1 ? 0 : lineStart)));
                else
                    edit.Delete(new Span(tag.OpeningTagRange.Item1, tag.OpeningTagRange.Item2 - tag.OpeningTagRange.Item1));

                edit.Apply();
            }
            else
            {
                var innerContent = documentContent.Substring(tag.OpeningTagRange.Item2, tag.ClosingTagRange.Value.Item1 - tag.OpeningTagRange.Item2).Trim([' ', '\n', '\r']);
                edit.Delete(new Span(tag.OpeningTagRange.Item1, tag.ClosingTagRange.Value.Item2 - tag.OpeningTagRange.Item1));
                edit.Insert(tag.OpeningTagRange.Item1, innerContent);
                edit.Apply();

                DocumentHelper.FormatDocument();
            }

            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextSnapshot, tag.OpeningTagRange.Item1));
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while removing tag.");
        }
    }
}
