using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Linq;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Merge Lines command.
/// </summary>
[Command(PackageIds.MergeLinesCommand)]
internal sealed class MergeLinesCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly ICssMatcherService _cssMatcherService;

    public MergeLinesCommand(DIToolkitPackage package,
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

            var selection = docView.TextView.Selection;
            var selectedText = selection.SelectedSpans?.FirstOrDefault().GetText();
            var hasMultipleLines = selectedText?.Contains("\n") ?? false;
            var startEditPosition = 0;

            if (hasMultipleLines)
                startEditPosition = selection.Start.Position.Position;
            else
            {
                var activeDocumentExtension = Path.GetExtension(DocumentHelper.GetActiveDocumentPath());
                if (string.IsNullOrWhiteSpace(activeDocumentExtension))
                    return;

                var position = docView.TextView.Caret.Position.BufferPosition.Position;
                var documentContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
                var syntaxType = SyntaxHelper.GetSyntaxType(activeDocumentExtension);
                if (syntaxType == FileType.None)
                    return;

                if (position == 0)
                    position = 1;
                else if (position == documentContent.Length)
                    position--;

                if (syntaxType == FileType.Markup)
                {
                    var matchResult = _htmlMatcherService.Match(documentContent, position);
                    if (matchResult is null || !matchResult.ClosingTagRange.HasValue)
                        return;

                    selectedText = documentContent.Substring(matchResult.OpeningTagRange.Item1, matchResult.ClosingTagRange.Value.Item2 - matchResult.OpeningTagRange.Item1);
                    startEditPosition = matchResult.OpeningTagRange.Item1;
                }
                else if (syntaxType == FileType.Stylesheet)
                {
                    var matchResult = _cssMatcherService.Match(documentContent, position);
                    if (matchResult is null)
                        return;

                    while (matchResult.Type != EmmetNetSharp.Enums.CssMatchType.Selector)
                    {
                        matchResult = _cssMatcherService.Match(documentContent, matchResult.Start - 1);
                        if (matchResult is null)
                            return;
                    }

                    selectedText = documentContent.Substring(matchResult.Start, matchResult.End - matchResult.Start);
                    startEditPosition = matchResult.Start;
                }
            }

            using var edit = docView.TextBuffer.CreateEdit();

            var lines = selectedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var trimmedLines = lines.Select(line => line.Trim()).ToArray();
            var mergedText = string.Join("", trimmedLines);

            if (hasMultipleLines)
                edit.Replace(selection.SelectedSpans.FirstOrDefault(), mergedText);
            else
                edit.Replace(startEditPosition, selectedText.Length, mergedText);

            edit.Apply();

            var newSpan = new SnapshotSpan(docView.TextView.TextSnapshot, startEditPosition, mergedText.Length);
            docView.TextView.Selection.Select(newSpan, isReversed: false);
            docView.TextView.Caret.MoveTo(newSpan.End);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while merging lines.");
        }
    }
}
