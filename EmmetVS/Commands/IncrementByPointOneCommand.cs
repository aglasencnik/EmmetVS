using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Increment by point one command.
/// </summary>
[Command(PackageIds.IncrementByPointOneCommand)]
internal sealed class IncrementByPointOneCommand : BaseDICommand
{
    public IncrementByPointOneCommand(DIToolkitPackage package) : base(package)
    {
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="e">OleMenuCmdEventArgs object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        return base.ExecuteAsync(e);
    }
}
