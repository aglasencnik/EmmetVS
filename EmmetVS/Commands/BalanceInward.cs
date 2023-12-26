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
/// Represents the Balance (inward) command.
/// </summary>
[Command(PackageIds.BalanceInwardCommand)]
internal sealed class BalanceInward : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly ICssMatcherService _cssMatcherService;

    public BalanceInward(DIToolkitPackage package,
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
                var balanceResult = _htmlMatcherService.BalanceInward(documentContent, position);
                if (balanceResult is null || !balanceResult.Any())
                    return;

                var tag = balanceResult.First();
                if (!tag.ClosingTagRange.HasValue) // Select the whole tag
                {
                    docView.TextView.Selection.Select(
                        new SnapshotSpan(
                            docView.TextView.TextBuffer.CurrentSnapshot,
                            new Span(tag.OpeningTagRange.Item1, tag.OpeningTagRange.Item2 - tag.OpeningTagRange.Item1)
                        ),
                        false
                    );
                    docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, tag.OpeningTagRange.Item1));
                }
                else if ((position >= tag.OpeningTagRange.Item1 && position <= tag.OpeningTagRange.Item2 - 1)
                    || (position >= tag.ClosingTagRange.Value.Item1 && position <= tag.ClosingTagRange.Value.Item2 - 1)) // Select tag contents
                {
                    docView.TextView.Selection.Select(
                        new SnapshotSpan(
                            docView.TextView.TextBuffer.CurrentSnapshot,
                            new Span(tag.OpeningTagRange.Item2, tag.ClosingTagRange.Value.Item1 - tag.OpeningTagRange.Item2)
                        ),
                        false
                    );
                    docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, tag.OpeningTagRange.Item2));
                }
                else // Select the whole tag
                {
                    var innerTag = balanceResult.Skip(1).FirstOrDefault();
                    if (innerTag is null)
                    {
                        docView.TextView.Selection.Select(
                            new SnapshotSpan(
                                docView.TextView.TextBuffer.CurrentSnapshot,
                                new Span(tag.OpeningTagRange.Item1, tag.ClosingTagRange.Value.Item2 - tag.OpeningTagRange.Item1)
                            ),
                            false
                        );
                        docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, tag.OpeningTagRange.Item1));
                    }
                    else
                    {
                        if (innerTag.ClosingTagRange.HasValue)
                        {
                            docView.TextView.Selection.Select(
                                new SnapshotSpan(
                                    docView.TextView.TextBuffer.CurrentSnapshot,
                                    new Span(innerTag.OpeningTagRange.Item1, innerTag.ClosingTagRange.Value.Item2 - innerTag.OpeningTagRange.Item1)
                                ),
                                false
                            );
                            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, innerTag.OpeningTagRange.Item1));
                        }
                        else
                        {
                            docView.TextView.Selection.Select(
                                new SnapshotSpan(
                                    docView.TextView.TextBuffer.CurrentSnapshot,
                                    new Span(innerTag.OpeningTagRange.Item1, innerTag.OpeningTagRange.Item2 - innerTag.OpeningTagRange.Item1)
                                ),
                                false
                            );
                            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, innerTag.OpeningTagRange.Item1));
                        }
                    } 
                }
            }
            else
            {
                var balanceResult = _cssMatcherService.BalanceInward(documentContent, position);
                if (balanceResult is null || !balanceResult.Any())
                    return;

                var element = balanceResult.FirstOrDefault(e => e.Item1 > position);
                if (element == default)
                    return;

                docView.TextView.Selection.Select(
                    new SnapshotSpan(
                        docView.TextView.TextBuffer.CurrentSnapshot,
                        new Span(element.Item1, element.Item2 - element.Item1)
                    ),
                    false
                );
                docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, element.Item1));
            }
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while balancing inward.");
        }
    }
}
