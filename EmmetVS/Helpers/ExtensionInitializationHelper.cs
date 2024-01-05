using EmmetVS.DefaultSnippets;
using EmmetVS.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EmmetVS.Helpers;

internal static class ExtensionInitializationHelper
{
    /// <summary>
    /// Checks the default installation options.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal static async Task CheckDefaultInstallationOptionsAsync()
    {
        if (RuntimeOptions.Instance.DefaultValuesSet)
            return;

        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var htmlSnippets = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.HtmlSnippetsLocation)) ?? string.Empty);
        var cssSnippets = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.CssSnippetsLocation)) ?? string.Empty);
        var xslSnippets = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.XslSnippetsLocation)) ?? string.Empty);

        var htmlSupportedFileTypes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.HtmlSupportedFileTypesLocation)) ?? string.Empty);
        var cssSupportedFileTypes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.CssSupportedFileTypesLocation)) ?? string.Empty);
        var xslSupportedFileTypes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.XslSupportedFileTypesLocation)) ?? string.Empty);

        var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(assemblyPath, SnippetDefaults.VariablesLocation)) ?? string.Empty);

        if (htmlSnippets is not null && htmlSnippets.Any())
        {
            HtmlOptions.Instance.Snippets = htmlSnippets;
            await HtmlOptions.Instance.SaveAsync();
        }

        if (htmlSupportedFileTypes is not null && htmlSupportedFileTypes.Any())
        {
            HtmlOptions.Instance.SupportedFileTypes = htmlSupportedFileTypes;
            await HtmlOptions.Instance.SaveAsync();
        }

        if (cssSnippets is not null && cssSnippets.Any())
        {
            CssOptions.Instance.Snippets = cssSnippets;
            await CssOptions.Instance.SaveAsync();
        }

        if (cssSupportedFileTypes is not null && cssSupportedFileTypes.Any())
        {
            CssOptions.Instance.SupportedFileTypes = cssSupportedFileTypes;
            await CssOptions.Instance.SaveAsync();
        }

        if (xslSnippets is not null && xslSnippets.Any())
        {
            XslOptions.Instance.Snippets = xslSnippets;
            await XslOptions.Instance.SaveAsync();
        }

        if (xslSupportedFileTypes is not null && xslSupportedFileTypes.Any())
        {
            XslOptions.Instance.SupportedFileTypes = xslSupportedFileTypes;
            await XslOptions.Instance.SaveAsync();
        }

        if (variables is not null && variables.Any())
        {
            VariableOptions.Instance.Variables = variables;
            await VariableOptions.Instance.SaveAsync();
        }

        RuntimeOptions.Instance.DefaultValuesSet = true;
        await RuntimeOptions.Instance.SaveAsync();
    }

    /// <summary>
    /// Checks the availability of commands.
    /// </summary>
    /// <param name="commandService">MenuCommandService object.</param>
    internal static void CheckCommandsAvailability(IMenuCommandService commandService)
    {
        if (commandService is null)
            return;

        // Get command ids
        var expandAbbreviationCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.ExpandAbbreviationCommand);
        var wrapWithAbbreviationCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.WrapWithAbbreviationCommand);
        var balanceOutwardCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.BalanceOutwardCommand);
        var balanceInwardCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.BalanceInwardCommand);
        var goToMatchingPairCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.GoToMatchingPairCommand);
        var splitJoinTagCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.SplitJoinTagCommand);
        var removeTagCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.RemoveTagCommand);
        var mergeLinesCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.MergeLinesCommand);
        var nextEditPointCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.NextEditPointCommand);
        var previousEditPointCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.PreviousEditPointCommand);
        var selectNextItemCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.SelectNextItemCommand);
        var selectPreviousItemCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.SelectPreviousItemCommand);
        var toggleCommentCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.ToggleCommentCommand);
        var reflectCSSValueCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.ReflectCSSValueCommand);
        var updateImageSizeCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.UpdateImageSizeCommand);
        var evaluateMathExpressionCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.EvaluateMathExpressionCommand);
        var incrementByOneCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.IncrementByOneCommand);
        var decrementByOneCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.DecrementByOneCommand);
        var incrementByPointOneCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.IncrementByPointOneCommand);
        var decrementByPointOneCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.DecrementByPointOneCommand);
        var incrementByTenCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.IncrementByTenCommand);
        var decrementByTenCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.DecrementByTenCommand);
        var encodeDecodeImageToDataURLCommandId = new CommandID(PackageGuids.EmmetVS, PackageIds.EncodeDecodeImageToDataURLCommand);

        // Get commands
        var expandAbbreviationCommand = commandService.FindCommand(expandAbbreviationCommandId);
        var wrapWithAbbreviationCommand = commandService.FindCommand(wrapWithAbbreviationCommandId);
        var balanceOutwardCommand = commandService.FindCommand(balanceOutwardCommandId);
        var balanceInwardCommand = commandService.FindCommand(balanceInwardCommandId);
        var goToMatchingPairCommand = commandService.FindCommand(goToMatchingPairCommandId);
        var splitJoinTagCommand = commandService.FindCommand(splitJoinTagCommandId);
        var removeTagCommand = commandService.FindCommand(removeTagCommandId);
        var mergeLinesCommand = commandService.FindCommand(mergeLinesCommandId);
        var nextEditPointCommand = commandService.FindCommand(nextEditPointCommandId);
        var previousEditPointCommand = commandService.FindCommand(previousEditPointCommandId);
        var selectNextItemCommand = commandService.FindCommand(selectNextItemCommandId);
        var selectPreviousItemCommand = commandService.FindCommand(selectPreviousItemCommandId);
        var toggleCommentCommand = commandService.FindCommand(toggleCommentCommandId);
        var reflectCSSValueCommand = commandService.FindCommand(reflectCSSValueCommandId);
        var updateImageSizeCommand = commandService.FindCommand(updateImageSizeCommandId);
        var evaluateMathExpressionCommand = commandService.FindCommand(evaluateMathExpressionCommandId);
        var incrementByOneCommand = commandService.FindCommand(incrementByOneCommandId);
        var decrementByOneCommand = commandService.FindCommand(decrementByOneCommandId);
        var incrementByPointOneCommand = commandService.FindCommand(incrementByPointOneCommandId);
        var decrementByPointOneCommand = commandService.FindCommand(decrementByPointOneCommandId);
        var incrementByTenCommand = commandService.FindCommand(incrementByTenCommandId);
        var decrementByTenCommand = commandService.FindCommand(decrementByTenCommandId);
        var encodeDecodeImageToDataURLCommand = commandService.FindCommand(encodeDecodeImageToDataURLCommandId);

        var enableCommands = GeneralOptions.Instance.Enable;
        var enableAdvanced = enableCommands && GeneralOptions.Instance.EnableAdvanced;

        expandAbbreviationCommand.Enabled = enableCommands;
        wrapWithAbbreviationCommand.Enabled = enableCommands;

        balanceOutwardCommand.Enabled = enableAdvanced;
        balanceInwardCommand.Enabled = enableAdvanced;
        goToMatchingPairCommand.Enabled = enableAdvanced;
        splitJoinTagCommand.Enabled = enableAdvanced;
        removeTagCommand.Enabled = enableAdvanced;
        mergeLinesCommand.Enabled = enableAdvanced;
        nextEditPointCommand.Enabled = enableAdvanced;
        previousEditPointCommand.Enabled = enableAdvanced;
        selectNextItemCommand.Enabled = enableAdvanced;
        selectPreviousItemCommand.Enabled = enableAdvanced;
        toggleCommentCommand.Enabled = enableAdvanced;
        reflectCSSValueCommand.Enabled = enableAdvanced;
        updateImageSizeCommand.Enabled = enableAdvanced;
        evaluateMathExpressionCommand.Enabled = enableAdvanced;
        incrementByOneCommand.Enabled = enableAdvanced;
        decrementByOneCommand.Enabled = enableAdvanced;
        incrementByPointOneCommand.Enabled = enableAdvanced;
        decrementByPointOneCommand.Enabled = enableAdvanced;
        incrementByTenCommand.Enabled = enableAdvanced;
        decrementByTenCommand.Enabled = enableAdvanced;
        encodeDecodeImageToDataURLCommand.Enabled = enableAdvanced;
    }
}
