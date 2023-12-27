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
/// Represents the Toggle Comment command.
/// </summary>
[Command(PackageIds.ToggleCommentCommand)]
internal sealed class ToggleCommentCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly ICssMatcherService _cssMatcherService;

    public ToggleCommentCommand(DIToolkitPackage package,
        IHtmlMatcherService htmlMatcherService,
        ICssMatcherService cssMatcherService) : base(package)
    {
        _htmlMatcherService = htmlMatcherService;
        _cssMatcherService = cssMatcherService;
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
            if (fileType == FileType.None)
                return;

            var position = docView.TextView.Caret.Position.BufferPosition.Position;
            var documentContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();

            if (position == 0)
                position = 1;
            else if (position == documentContent.Length)
                position--;

            var isComment = false;
            (int, int)? selectionRange = null;

            if (fileType != FileType.Stylesheet) // HTML or XML
            {
                var matchResult = _htmlMatcherService.Match(documentContent, position);
                if (matchResult is null)
                {
                    var startSectionLength = Math.Min(position + 4, documentContent.Length - 1);
                    var startSection = documentContent.Substring(0, startSectionLength);
                    var startCommentIndex = startSection.LastIndexOf("<!--", StringComparison.Ordinal);
                    if (startCommentIndex == -1)
                        return;

                    var endSectionStartIndex = Math.Max(0, position - 3);
                    var endSection = documentContent.Substring(endSectionStartIndex);
                    var endCommentIndex = endSection.IndexOf("-->", StringComparison.Ordinal);
                    if (endCommentIndex == -1)
                        return;

                    selectionRange = (startCommentIndex, endSectionStartIndex + endCommentIndex + 3);
                    isComment = false;
                }
                else
                {
                    if (IsInsideComment(documentContent, position, "<!--", "-->"))
                    {
                        var startSectionLength = Math.Min(position + 4, documentContent.Length - 1);
                        var startSection = documentContent.Substring(0, startSectionLength);
                        var startCommentIndex = startSection.LastIndexOf("<!--", StringComparison.Ordinal);
                        if (startCommentIndex == -1)
                            return;

                        var endSectionStartIndex = Math.Max(0, position - 3);
                        var endSection = documentContent.Substring(endSectionStartIndex);
                        var endCommentIndex = endSection.IndexOf("-->", StringComparison.Ordinal);
                        if (endCommentIndex == -1)
                            return;

                        selectionRange = (startCommentIndex, endSectionStartIndex + endCommentIndex + 3);
                        isComment = false;
                    }
                    else
                    {
                        if (matchResult.ClosingTagRange.HasValue)
                            selectionRange = (matchResult.OpeningTagRange.Item1, matchResult.ClosingTagRange.Value.Item2);
                        else
                            selectionRange = (matchResult.OpeningTagRange.Item1, matchResult.OpeningTagRange.Item2);

                        isComment = true;
                    }
                }
            }
            else // CSS
            {
                var matchResult = _cssMatcherService.Match(documentContent, position);
                if (matchResult is null)
                {
                    var startSectionLength = Math.Min(position + 2, documentContent.Length - 1);
                    var startSection = documentContent.Substring(0, startSectionLength);
                    var startCommentIndex = startSection.LastIndexOf("/*", StringComparison.Ordinal);
                    if (startCommentIndex == -1)
                        return;

                    var endSectionStartIndex = Math.Max(0, position - 2);
                    var endSection = documentContent.Substring(endSectionStartIndex);
                    var endCommentIndex = endSection.IndexOf("*/", StringComparison.Ordinal);
                    if (endCommentIndex == -1)
                        return;

                    selectionRange = (startCommentIndex, endSectionStartIndex + endCommentIndex + 2);
                    isComment = false;
                }
                else
                {
                    if (IsInsideComment(documentContent, position, "/*", "*/"))
                    {
                        var startSectionLength = Math.Min(position + 2, documentContent.Length - 1);
                        var startSection = documentContent.Substring(0, startSectionLength);
                        var startCommentIndex = startSection.LastIndexOf("/*", StringComparison.Ordinal);
                        if (startCommentIndex == -1)
                            return;

                        var endSectionStartIndex = Math.Max(0, position - 2);
                        var endSection = documentContent.Substring(endSectionStartIndex);
                        var endCommentIndex = endSection.IndexOf("*/", StringComparison.Ordinal);
                        if (endCommentIndex == -1)
                            return;

                        selectionRange = (startCommentIndex, endSectionStartIndex + endCommentIndex + 2);
                        isComment = false;
                    }
                    else
                    {
                        selectionRange = (matchResult.Start, matchResult.End);
                        isComment = true;
                    }
                }
            }

            if (selectionRange.HasValue)
            {
                var span = new SnapshotSpan(
                    docView.TextView.TextSnapshot, 
                    new Span(selectionRange.Value.Item1, selectionRange.Value.Item2 - selectionRange.Value.Item1)
                );

                docView.TextView.Selection.Select(span, false);

                if (isComment)
                    DocumentHelper.CommentSelection();
                else
                    DocumentHelper.UncommentSelection();

                docView.TextView.Selection.Clear();
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while toggling comment.");
        }
    }

    /// <summary>
    /// Checks whether the specified position is inside a comment.
    /// </summary>
    /// <param name="content">File content</param>
    /// <param name="position">Caret position</param>
    /// <param name="startComment">Start comment</param>
    /// <param name="endComment">End comment</param>
    /// <returns>Whether caret is inside comment</returns>
    private bool IsInsideComment(string content, int position, string startComment, string endComment)
    {
        int startCommentIndex = content.LastIndexOf(startComment, position, StringComparison.Ordinal);
        if (startCommentIndex == -1)
            return false;

        int endCommentIndex = content.IndexOf(endComment, startCommentIndex, StringComparison.Ordinal);
        if (endCommentIndex == -1)
            return false;

        return position > startCommentIndex && position < endCommentIndex + endComment.Length;
    }
}
