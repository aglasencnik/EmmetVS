using Community.VisualStudio.Toolkit;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the edit point helper.
/// </summary>
internal static class EditPointHelper
{
    /// <summary>
    /// Finds the new edit point.
    /// </summary>
    /// <param name="docView">Document view object</param>
    /// <param name="inc">Whether to search forwards or backwards</param>
    /// <returns>Position of the new edit point</returns>
    internal static int? FindNewEditPoint(DocumentView docView, int inc)
    {
        var doc = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
        var docLength = doc.Length;
        var curPos = docView.TextView.Caret.Position.BufferPosition.Position + inc;

        while (curPos < docLength && curPos >= 0)
        {
            curPos += inc;
            var curChar = (docLength - 1 >= curPos && curPos >= 0) ? doc[curPos] : default;
            var next = (docLength - 1 >= curPos + 1 && curPos + 1 >= 0) ? doc[curPos + 1] : default;
            var prev = (curPos - 1 >= 0 && curPos - 1 >= 0) ? doc[curPos - 1] : default;

            if ((curChar == '\'' || curChar == '"') && next == curChar && prev == '=')
                return curPos + 1; // Empty attribute value

            if (curChar == '<' && prev == '>')
                return curPos; // Between tags

            if (curChar == '\n' || curChar == '\r')
            {
                var lineRange = docView.TextView.TextSnapshot.GetLineFromPosition(curPos).Extent;
                var line = docView.TextView.TextSnapshot.GetText(lineRange);
                if (string.IsNullOrWhiteSpace(line))
                    return lineRange.End.Position - (((line.Length - 1 >= 0 && line[line.Length - 1] == '\n') || (line.Length - 1 >= 0 && line[line.Length - 1] == '\r')) ? 1 : 0); // Empty line
            }
        }

        return null;
    }
}
