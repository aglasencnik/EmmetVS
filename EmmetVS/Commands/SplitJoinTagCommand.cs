using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Split/Join Tag command.
/// </summary>
[Command(PackageIds.SplitJoinTagCommand)]
internal sealed class SplitJoinTagCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;

    public SplitJoinTagCommand(DIToolkitPackage package,
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
                var selfClosingTag = documentContent.Substring(tag.OpeningTagRange.Item1, tag.OpeningTagRange.Item2 - tag.OpeningTagRange.Item1);
                var openingTag = Regex.Replace(selfClosingTag, @"<([^/]+?)\s*/?\s*>", "<$1>");
                var closingTag = $"</{tag.Name}>";
                var newTag = $"{openingTag}{closingTag}";
                var numCharsRemoved = selfClosingTag.Length - openingTag.Length;

                edit.Replace(new Span(tag.OpeningTagRange.Item1, selfClosingTag.Length), newTag);
                edit.Apply();

                docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextSnapshot, tag.OpeningTagRange.Item2 - numCharsRemoved));
            }
            else
            {
                var openingTagContent = documentContent
                    .Substring(tag.OpeningTagRange.Item1, tag.OpeningTagRange.Item2 - tag.OpeningTagRange.Item1)
                    .TrimEnd('>', ' ');
                var selfClosingTag = $"{openingTagContent} />";

                edit.Replace(new Span(tag.OpeningTagRange.Item1, tag.ClosingTagRange.Value.Item2 - tag.OpeningTagRange.Item1), selfClosingTag);
                edit.Apply();

                docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextSnapshot, tag.OpeningTagRange.Item1 + selfClosingTag.Length));
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while splitting or joining tag.");
        }
    }
}
