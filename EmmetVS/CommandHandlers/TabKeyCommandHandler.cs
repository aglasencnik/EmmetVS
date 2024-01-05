using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Linq;
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
                if (!GeneralOptions.Instance.Enable)
                    return false;

                var docView = VS.Documents.GetActiveDocumentViewAsync().GetAwaiter().GetResult();
                if (docView?.TextView is null)
                    return false;

                var selection = docView.TextView.Selection;
                var selectedText = selection.SelectedSpans?.FirstOrDefault().GetText();

                if ((!string.IsNullOrWhiteSpace(selectedText) && !GeneralOptions.Instance.EnableWrapWithTabKey) ||
                    (string.IsNullOrWhiteSpace(selectedText) && !GeneralOptions.Instance.EnableExpandWithTabKey))
                    return false;

                var serviceProvider = VS.GetServiceAsync<SToolkitServiceProvider<EmmetVSPackage>, IToolkitServiceProvider<EmmetVSPackage>>().GetAwaiter().GetResult();
                var abbreviationService = serviceProvider.GetRequiredService<IAbbreviationService>();
                if (abbreviationService is null)
                    return false;

                if (string.IsNullOrWhiteSpace(selectedText))
                    return AbbreviationExpanderHelper.ExpandAbbreviation(docView, abbreviationService);
                else
                {
                    var htmlMatcherService = serviceProvider.GetRequiredService<IHtmlMatcherService>();
                    if (htmlMatcherService is null)
                        return false;

                    return AbbreviationExpanderHelper.WrapWithAbbreviation(docView, htmlMatcherService, abbreviationService);
                }
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
