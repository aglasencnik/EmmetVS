using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Reflect CSS Value command.
/// </summary>
[Command(PackageIds.ReflectCSSValueCommand)]
internal sealed class ReflectCSSValueCommand(DIToolkitPackage package) : BaseDICommand(package)
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
            if (!GeneralOptions.Instance.Enable || !GeneralOptions.Instance.EnableAdvanced)
                return;

            var docView = await VS.Documents.GetActiveDocumentViewAsync();
            if (docView?.TextView is null)
                return;

            var position = docView.TextView.Caret.Position.BufferPosition;
            var currentLine = position.GetContainingLine();
            var lineText = currentLine.GetText();
            var caretPos = position.Position - currentLine.Start.Position;
            if (caretPos == lineText.Length)
                caretPos--;

            var entireContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
            var cursorPosition = position.Position;

            var openBracePosition = entireContent.LastIndexOf('{', cursorPosition);
            if (openBracePosition == -1)
                return;

            var closeBracePosition = entireContent.IndexOf('}', cursorPosition);
            if (closeBracePosition == -1)
                return;

            var selectorStartPosition = entireContent.LastIndexOf('\n', openBracePosition);
            if (selectorStartPosition == -1) // If there's no newline, use the start of the document
                selectorStartPosition = 0;
            else
                selectorStartPosition++; // Skip the newline character

            var cssBlock = entireContent.Substring(selectorStartPosition, (closeBracePosition - selectorStartPosition) + 1);

            var match = Regex.Match(lineText, @"^\s*(?<property>[\w-]+)\s*:\s*(?<value>[^;]+);");
            if (!match.Success)
                return;

            var propertyUnderCursor = match.Groups["property"].Value ?? string.Empty;
            var valueUnderCursor = match.Groups["value"].Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(propertyUnderCursor) || string.IsNullOrWhiteSpace(valueUnderCursor))
                return;

            var properties = Regex.Matches(cssBlock, @"\s*(?<property>[\w-]+)\s*:\s*(?<value>[^;]+);")
                .Cast<Match>()
                .ToDictionary(m => m.Groups["property"].Value, m => m.Groups["value"].Value);

            var vendorPrefixes = new[] { "-webkit-", "-moz-", "-ms-", "-o-" };
            var isVendorProperty = vendorPrefixes.Any(propertyUnderCursor.StartsWith);

            var baseProperty = (!isVendorProperty) ? propertyUnderCursor : propertyUnderCursor.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            properties[baseProperty] = valueUnderCursor;  // Set the base property

            foreach (var prefix in vendorPrefixes)
            {
                var vendorProperty = prefix + baseProperty;
                if (!properties.ContainsKey(vendorProperty) || properties[vendorProperty] != valueUnderCursor)
                {
                    properties[vendorProperty] = valueUnderCursor;  // Add or update the vendor-specific property
                }
            }

            var selector = cssBlock.Substring(0, openBracePosition - selectorStartPosition).Trim();
            var indentation = StylesheetHelper.GetIndentation(cssBlock);
            var generatedCss = StylesheetHelper.GenerateVendorProperties(selector, properties, baseProperty, indentation);

            using var edit = docView.TextView.TextBuffer.CreateEdit();
            edit.Replace(new Span(selectorStartPosition, (closeBracePosition - selectorStartPosition) + 1), generatedCss);
            edit.Apply();

            var point = new SnapshotPoint(docView.TextView.TextSnapshot, currentLine.Start + caretPos);
            docView.TextView.Caret.MoveTo(point);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while reflecting CSS values.");
        }
    }
}
