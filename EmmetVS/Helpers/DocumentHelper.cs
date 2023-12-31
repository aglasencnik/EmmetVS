﻿using EnvDTE;

namespace EmmetVS.Helpers;

/// <summary>
/// Provides helper methods for working with documents.
/// </summary>
internal static class DocumentHelper
{
    /// <summary>
    /// Gets the active document path.
    /// </summary>
    /// <returns>Active document path</returns>
    internal static string GetActiveDocumentPath()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        return dte?.ActiveDocument?.FullName;
    }

    /// <summary>
    /// Formats document.
    /// </summary>
    internal static void FormatDocument()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        var command = "Edit.FormatDocument";
        if (dte.Commands.Item(command).IsAvailable)
            dte.ExecuteCommand(command);
    }

    /// <summary>
    /// Comments selection.
    /// </summary>
    internal static void CommentSelection()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        var command = "Edit.CommentSelection";
        if (dte.Commands.Item(command).IsAvailable)
            dte.ExecuteCommand(command);
    }

    /// <summary>
    /// Uncomments selection.
    /// </summary>
    internal static void UncommentSelection()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        var command = "Edit.UncommentSelection";
        if (dte.Commands.Item(command).IsAvailable)
            dte.ExecuteCommand(command);
    }
}
