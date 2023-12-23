﻿using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Go to Matching Pair command.
/// </summary>
[Command(PackageIds.GoToMatchingPairCommand)]
internal sealed class GoToMatchingPairCommand : BaseDICommand
{
    public GoToMatchingPairCommand(DIToolkitPackage package) : base(package)
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