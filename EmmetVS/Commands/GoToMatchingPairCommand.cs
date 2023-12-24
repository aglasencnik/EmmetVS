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
/// Represents the Go to Matching Pair command.
/// </summary>
[Command(PackageIds.GoToMatchingPairCommand)]
internal sealed class GoToMatchingPairCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly ICssMatcherService _cssMatcherService;

    public GoToMatchingPairCommand(DIToolkitPackage package,
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
                if (matchResult is null)
                    return;
                
                if (!matchResult.ClosingTagRange.HasValue)
                {
                    var point = new SnapshotPoint(docView.TextView.TextSnapshot, matchResult.OpeningTagRange.Item1);
                    docView.TextView.Caret.MoveTo(point);
                }
                else if (position >= matchResult.OpeningTagRange.Item1 && position <= matchResult.OpeningTagRange.Item2 - 1)
                {
                    var point = new SnapshotPoint(docView.TextView.TextSnapshot, matchResult.ClosingTagRange.Value.Item2);
                    docView.TextView.Caret.MoveTo(point);
                }
                else if (position <= matchResult.ClosingTagRange.Value.Item2)
                {
                    var point = new SnapshotPoint(docView.TextView.TextSnapshot, matchResult.OpeningTagRange.Item1);
                    docView.TextView.Caret.MoveTo(point);
                }
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

                if (position >= matchResult.Start && position <= matchResult.BodyStart)
                {
                    var point = new SnapshotPoint(docView.TextView.TextSnapshot, matchResult.End);
                    docView.TextView.Caret.MoveTo(point);
                }
                else if (position >= matchResult.BodyStart && position <= matchResult.End)
                {
                    var point = new SnapshotPoint(docView.TextView.TextSnapshot, matchResult.Start);
                    docView.TextView.Caret.MoveTo(point);
                }
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while matching pair.");
        }
    }
}
