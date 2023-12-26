using Community.VisualStudio.Toolkit;
using EmmetNetSharp.Interfaces;
using Microsoft.VisualStudio.Text;
using System.Linq;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the Select Item helper.
/// </summary>
internal static class SelectItemHelper
{
    /// <summary>
    /// Finds the new selected item.
    /// </summary>
    /// <param name="actionUtilsService">Action utils service object</param>
    /// <param name="docView">Document View object</param>
    /// <param name="isCss">Whether document is css</param>
    /// <param name="isPrev">Whether it is select previous</param>
    /// <returns>Range, if it exists</returns>
    internal static (int, int)? FindNewSelectedItem(IActionUtilsService actionUtilsService, DocumentView docView, bool isCss, bool isPrev)
    {
        var selection = docView.TextView.Selection;
        var code = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
        var caretPosition = docView.TextView.Caret.Position.BufferPosition.Position;
        var model = isCss ? actionUtilsService.SelectItemCss(code, caretPosition, isPrev) : actionUtilsService.SelectItemHtml(code, caretPosition, isPrev);
        if (model is null || !model.Ranges.Any())
            return null;

        var range = FindRange(selection.SelectedSpans.FirstOrDefault(), model.Ranges, isPrev);
        if (!range.HasValue)
        {
            var nextPos = isPrev ? model.Start : model.End;
            model = isCss ? actionUtilsService.SelectItemCss(code, nextPos, isPrev) : actionUtilsService.SelectItemHtml(code, nextPos, isPrev);
            if (model is not null && model.Ranges.Any())
                range = FindRange(selection.SelectedSpans.FirstOrDefault(), model.Ranges, isPrev);
        }

        if (range.HasValue)
            return range.Value;

        return null;
    }

    /// <summary>
    /// Finds the range.
    /// </summary>
    /// <param name="selection">Selection span</param>
    /// <param name="ranges">Possible ranges</param>
    /// <param name="reverse">If it is reverse</param>
    /// <returns>Range</returns>
    private static (int, int)? FindRange(Span selection, (int, int)[] ranges, bool reverse = false)
    {
        if (reverse)
            ranges = ranges.Reverse().ToArray();

        var getNext = false;
        (int, int)? candidate = null;

        foreach (var range in ranges)
        {
            if (getNext)
                return range;

            if (range.Item1 == selection.Start && range.Item2 == selection.End)
                getNext = true;
            else if (!candidate.HasValue && (Contains(range, selection) || (reverse && range.Item1 <= selection.Start) || (!reverse && range.Item1 >= selection.Start)))
                candidate = range;
        }

        if (!getNext)
            return candidate;

        return null;
    }

    /// <summary>
    /// Checks whether range contains selection.
    /// </summary>
    /// <param name="range">Range</param>
    /// <param name="selection">Selection span</param>
    /// <returns>Whether range contains selection</returns>
    private static bool Contains((int, int) range, Span selection)
    {
        return selection.Start >= range.Item1 && selection.Start <= range.Item2 
            && selection.End >= range.Item1 && selection.End <= range.Item2;
    }
}
