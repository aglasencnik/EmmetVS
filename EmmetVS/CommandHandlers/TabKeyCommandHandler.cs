using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
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
                // TODO
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
