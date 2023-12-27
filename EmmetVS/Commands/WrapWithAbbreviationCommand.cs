using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualBasic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Wrap With Abbreviation command.
/// </summary>
[Command(PackageIds.WrapWithAbbreviationCommand)]
internal sealed class WrapWithAbbreviationCommand : BaseDICommand
{
    private readonly IHtmlMatcherService _htmlMatcherService;
    private readonly IAbbreviationService _abbreviationService;

    public WrapWithAbbreviationCommand(DIToolkitPackage package,
        IHtmlMatcherService htmlMatcherService,
        IAbbreviationService abbreviationService) : base(package)
    {
        _htmlMatcherService = htmlMatcherService;
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
            if (!GeneralOptions.Instance.Enable || !GeneralOptions.Instance.EnableAdvanced)
                return;

            var docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView is null)
                return;

            var activeDocumentExtension = Path.GetExtension(DocumentHelper.GetActiveDocumentPath());
            if (string.IsNullOrWhiteSpace(activeDocumentExtension))
                return;

            var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
            if (fileType == FileType.None || fileType == FileType.Stylesheet)
                return;

            var selection = docView.TextView.Selection;
            var selectedText = selection.SelectedSpans?.FirstOrDefault().GetText();
            var startEditPosition = 0;

            if (!string.IsNullOrWhiteSpace(selectedText))
                startEditPosition = selection.Start.Position.Position;
            else
            {
                var position = docView.TextView.Caret.Position.BufferPosition.Position;
                var documentContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();

                if (position == 0)
                    position = 1;
                else if (position == documentContent.Length)
                    position--;

                var matchResult = _htmlMatcherService.Match(documentContent, position);
                if (matchResult is null || !matchResult.ClosingTagRange.HasValue)
                    return;

                selectedText = documentContent.Substring(matchResult.OpeningTagRange.Item1, matchResult.ClosingTagRange.Value.Item2 - matchResult.OpeningTagRange.Item1);
                startEditPosition = matchResult.OpeningTagRange.Item1;
            }

            if (string.IsNullOrWhiteSpace(selectedText))
                return;

            var abbrInput = Interaction.InputBox("Enter abbreviation:", "Enter abbreviation");
            if (string.IsNullOrWhiteSpace(abbrInput))
                return;

            var abbreviation = _abbreviationService.ExtractAbbreviation(abbrInput);
            if (abbreviation is null)
                return;

            var config = OptionsHelper.GetAbbreviationOptions();

            var expandedAbbreviation = _abbreviationService.ExpandAbbreviation(abbreviation.Abbreviation, new UserConfig
            {
                Syntax = SyntaxHelper.GetFileSyntax(activeDocumentExtension, fileType),
                Type = fileType.ToString().ToLower(),
                Options = config,
                Variables = VariableOptions.Instance.Variables,
                Snippets = fileType == FileType.Markup ? HtmlOptions.Instance.Snippets : (fileType == FileType.Stylesheet ? CssOptions.Instance.Snippets : XslOptions.Instance.Snippets),
                Text = GetWrapContent(selectedText)
            });

            if (string.IsNullOrWhiteSpace(expandedAbbreviation))
                return;

            using var edit = docView.TextBuffer.CreateEdit();
            edit.Replace(startEditPosition, selectedText.Length, expandedAbbreviation);
            edit.Apply();

            DocumentHelper.FormatDocument();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while wrapping with abbreviation.");
        }
    }

    /// <summary>
    /// Gets the content to wrap with abbreviation.
    /// </summary>
    /// <param name="text">Selected text</param>
    /// <returns>Array of text lines</returns>
    private string[] GetWrapContent(string text)
    {
        var lines = Regex.Replace(text, @"\r\n?|\n", "\n").Split('\n');
        var firstLine = lines.FirstOrDefault();
        var match = Regex.Match(firstLine, @"^\s+");
        var indent = match.Success ? match.Value : string.Empty;

        return lines.Select((line, index) =>
        {
            if (index != 0 && line.StartsWith(indent))
                return line.Substring(indent.Length);
            return line;
        }).ToArray();
    }
}
