using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using System.IO;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Expand Abbreviation command.
/// </summary>
[Command(PackageIds.ExpandAbbreviationCommand)]
internal sealed class ExpandAbbreviationCommand : BaseDICommand
{
    private readonly IAbbreviationService _abbreviationService;

    public ExpandAbbreviationCommand(DIToolkitPackage package,
        IAbbreviationService abbreviationService) : base(package)
    {
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
            if (!GeneralOptions.Instance.Enable)
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

            var caretPosition = docView.TextView.Caret.Position.BufferPosition.Position;
            var currentLine = docView.TextView.TextSnapshot.GetLineFromPosition(caretPosition);
            var lineText = currentLine.GetText();
            var config = OptionsHelper.GetAbbreviationOptions();
            var abbreviation = _abbreviationService.ExtractAbbreviation(lineText.TrimEnd(), caretPosition);
            if (abbreviation is null)
                return;

            var expandedAbbreviation = _abbreviationService.ExpandAbbreviation(abbreviation.Abbreviation, new UserConfig
            {
                Syntax = SyntaxHelper.GetFileSyntax(activeDocumentExtension, fileType),
                Type = fileType.ToString().ToLower(),
                Options = config,
                Variables = VariableOptions.Instance.Variables,
                Snippets = fileType == FileType.Markup ? HtmlOptions.Instance.Snippets : (fileType == FileType.Stylesheet ? CssOptions.Instance.Snippets : XslOptions.Instance.Snippets)
            });

            if (string.IsNullOrWhiteSpace(expandedAbbreviation))
                return;

            using var edit = docView.TextBuffer.CreateEdit();
            edit.Replace(currentLine.Start.Position + abbreviation.Start, abbreviation.End - abbreviation.Start, expandedAbbreviation);
            edit.Apply();

            DocumentHelper.FormatDocument();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while expanding abbreviation.");
        }
    }
}
