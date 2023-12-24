using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Decrement by one command.
/// </summary>
[Command(PackageIds.DecrementByOneCommand)]
internal sealed class DecrementByOneCommand : BaseDICommand
{
    public DecrementByOneCommand(DIToolkitPackage package) : base(package)
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
