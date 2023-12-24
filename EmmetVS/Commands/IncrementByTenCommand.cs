using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetVS.Helpers;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Increment by ten command.
/// </summary>
[Command(PackageIds.IncrementByTenCommand)]
internal sealed class IncrementByTenCommand(DIToolkitPackage package) : BaseDICommand(package)
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="e">OleMenuCmdEventArgs object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        try
        {
            await IncrementDecrementHelper.ProcessNumberInLineAsync((ref double parsedNumber) => parsedNumber += 10);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while incrementing by ten.");
        }
    }
}
