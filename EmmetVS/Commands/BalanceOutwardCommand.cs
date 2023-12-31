﻿using Community.VisualStudio.Toolkit;
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
/// Represents the Balance (outward) command.
/// </summary>
[Command(PackageIds.BalanceOutwardCommand)]
internal sealed class BalanceOutwardCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly ICssMatcherService _cssMatcherService;

    public BalanceOutwardCommand(DIToolkitPackage package,
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
            var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
            if (fileType == FileType.None)
                return;

            if (position == 0)
                position = 1;
            else if (position == documentContent.Length)
                position--;

            if (fileType == FileType.Markup)
            {
                var balanceResult = _htmlMatcherService.BalanceOutward(documentContent, position);
                if (balanceResult is null || !balanceResult.Any())
                    return;

                var tag = balanceResult.First();
                if (!tag.ClosingTagRange.HasValue) // Select whole tag
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
                else if ((position >= tag.OpeningTagRange.Item1 && position <= tag.OpeningTagRange.Item2) 
                    || (position >= tag.ClosingTagRange.Value.Item1 && position <= tag.ClosingTagRange.Value.Item2)) // Select whole tag
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
                else // Select body
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
            }
            else
            {
                var balanceResult = _cssMatcherService.BalanceOutward(documentContent, position);
                if (balanceResult is null || balanceResult.Length < 2)
                    return;

                var element = balanceResult.Skip(1).First();
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
            await ex.LogAsync("Error while balancing outward.");
        }
    }
}
