using Community.VisualStudio.Toolkit;
using EmmetNetSharp.Interfaces;
using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Options;
using Microsoft.VisualBasic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the helper class for abbreviation expander.
/// </summary>
internal static class AbbreviationExpanderHelper
{
    #region Utils

    /// <summary>
    /// Gets the content to wrap with abbreviation.
    /// </summary>
    /// <param name="text">Selected text</param>
    /// <returns>Array of text lines</returns>
    private static string[] GetWrapContent(string text)
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

    #endregion

    #region Methods

    /// <summary>
    /// Expands abbreviation at caret position.
    /// </summary>
    /// <param name="docView">DocumentView Object</param>
    /// <param name="abbreviationService">AbbreviationService Object</param>
    /// <returns>Whether abbreviation was successful.</returns>
    internal static bool ExpandAbbreviation(DocumentView docView, IAbbreviationService abbreviationService)
    {
        var activeDocumentExtension = Path.GetExtension(DocumentHelper.GetActiveDocumentPath());
        if (string.IsNullOrWhiteSpace(activeDocumentExtension))
            return false;

        var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
        if (fileType == FileType.None)
            return false;

        var caretPosition = docView.TextView.Caret.Position.BufferPosition.Position;
        var currentLine = docView.TextView.TextSnapshot.GetLineFromPosition(caretPosition);
        var lineText = currentLine.GetText();
        var textAfterCaret = lineText.Substring(caretPosition - currentLine.Start.Position);
        if (!string.IsNullOrWhiteSpace(textAfterCaret))
            return false;

        var config = OptionsHelper.GetAbbreviationOptions();
        var abbreviation = abbreviationService.ExtractAbbreviation(lineText.TrimEnd(), caretPosition);
        if (abbreviation is null)
            return false;


        var expandedAbbreviation = abbreviationService.ExpandAbbreviation(abbreviation.Abbreviation, new UserConfig
        {
            Syntax = SyntaxHelper.GetFileSyntax(activeDocumentExtension, fileType),
            Type = fileType.ToString().ToLower(),
            Options = config,
            Variables = VariableOptions.Instance.Variables,
            Snippets = fileType == FileType.Markup ? HtmlOptions.Instance.Snippets : (fileType == FileType.Stylesheet ? CssOptions.Instance.Snippets : XslOptions.Instance.Snippets)
        });

        if (string.IsNullOrWhiteSpace(expandedAbbreviation))
            return false;

        using var edit = docView.TextBuffer.CreateEdit();
        edit.Replace(currentLine.Start.Position + abbreviation.Start, abbreviation.End - abbreviation.Start, expandedAbbreviation);
        edit.Apply();

        DocumentHelper.FormatDocument();
        return true;
    }

    /// <summary>
    /// Wrpas selected text with abbreviation.
    /// </summary>
    /// <param name="docView">DocumentView Object</param>
    /// <param name="htmlMatcherService">HtmlMatcherService Object</param>
    /// <param name="abbreviationService">AbbreviationService Object</param>
    /// <returns>Whether wrapping selected text with abbreviation was successful.</returns>
    internal static bool WrapWithAbbreviation(DocumentView docView, IHtmlMatcherService htmlMatcherService, IAbbreviationService abbreviationService)
    {
        var activeDocumentExtension = Path.GetExtension(DocumentHelper.GetActiveDocumentPath());
        if (string.IsNullOrWhiteSpace(activeDocumentExtension))
            return false;

        var fileType = SyntaxHelper.GetFileType(activeDocumentExtension);
        if (fileType == FileType.None || fileType == FileType.Stylesheet)
            return false;

        var selection = docView.TextView.Selection;
        var selectedText = selection.SelectedSpans?.FirstOrDefault().GetText();
        int startEditPosition;

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

            var matchResult = htmlMatcherService.Match(documentContent, position);
            if (matchResult is null || !matchResult.ClosingTagRange.HasValue)
                return false;

            selectedText = documentContent.Substring(matchResult.OpeningTagRange.Item1, matchResult.ClosingTagRange.Value.Item2 - matchResult.OpeningTagRange.Item1);
            startEditPosition = matchResult.OpeningTagRange.Item1;
        }

        if (string.IsNullOrWhiteSpace(selectedText))
            return false;

        var abbrInput = Interaction.InputBox("Enter abbreviation:", "Enter abbreviation");
        if (string.IsNullOrWhiteSpace(abbrInput))
            return false;

        var abbreviation = abbreviationService.ExtractAbbreviation(abbrInput);
        if (abbreviation is null)
            return false;

        var config = OptionsHelper.GetAbbreviationOptions();

        var expandedAbbreviation = abbreviationService.ExpandAbbreviation(abbreviation.Abbreviation, new UserConfig
        {
            Syntax = SyntaxHelper.GetFileSyntax(activeDocumentExtension, fileType),
            Type = fileType.ToString().ToLower(),
            Options = config,
            Variables = VariableOptions.Instance.Variables,
            Snippets = fileType == FileType.Markup ? HtmlOptions.Instance.Snippets : (fileType == FileType.Stylesheet ? CssOptions.Instance.Snippets : XslOptions.Instance.Snippets),
            Text = GetWrapContent(selectedText)
        });

        if (string.IsNullOrWhiteSpace(expandedAbbreviation))
            return false;

        using var edit = docView.TextBuffer.CreateEdit();
        edit.Replace(startEditPosition, selectedText.Length, expandedAbbreviation);
        edit.Apply();

        DocumentHelper.FormatDocument();
        return true;
    }

    #endregion
}
