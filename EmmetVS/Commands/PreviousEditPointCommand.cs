﻿using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Previous Edit Point command.
/// </summary>
[Command(PackageIds.PreviousEditPointCommand)]
internal sealed class PreviousEditPointCommand : BaseDICommand
{
    public PreviousEditPointCommand(DIToolkitPackage package) : base(package)
    {
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="e">OleMenuCmdEventArgs object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await base.ExecuteAsync(e);
    }
}
