using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;

namespace EmmetVS.CommandHandlers;

/// <summary>
/// Represents the command handler for the Tab key command.
/// </summary>
[Export(typeof(ICommandHandler))]
[Name(nameof(TabKeyCommandHandler))]
[ContentType(ContentTypes.Text)]
[TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
internal sealed class TabKeyCommandHandler : ICommandHandler<TabKeyCommandArgs>
{
    /// <summary>
    /// Gets the display name of the command handler.
    /// </summary>
    public string DisplayName => Vsix.Name;

    /// <summary>
    /// Executes command handler.
    /// </summary>
    /// <param name="args">TabKeyCommandArgs object</param>
    /// <param name="executionContext">CommandExecutionContext object</param>
    /// <returns>Whether key was overriden successfully.</returns>
    public bool ExecuteCommand(TabKeyCommandArgs args, CommandExecutionContext executionContext)
    {
        try
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                if (!GeneralOptions.Instance.Enable || !GeneralOptions.Instance.EnableTabKey)
                    return false;

                var docView = VS.Documents.GetActiveDocumentViewAsync().GetAwaiter().GetResult();
                if (docView?.TextView is null)
                    return false;

                var serviceProvider = VS.GetServiceAsync<SToolkitServiceProvider<EmmetVSPackage>, IToolkitServiceProvider<EmmetVSPackage>>().GetAwaiter().GetResult();
                var abbreviationService = serviceProvider.GetRequiredService<IAbbreviationService>();
                if (abbreviationService is null)
                    return false;

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

            return false;
        }
        catch (Exception ex)
        {
            ex.Log("Error while executing tab key command.");
            return false;
        }
    }

    /// <summary>
    /// Determines whether command handler is enabled.
    /// </summary>
    /// <param name="args">TabKeyCommandArgs object</param>
    /// <returns>CommandState object</returns>
    public CommandState GetCommandState(TabKeyCommandArgs args) => CommandState.Available;
}
