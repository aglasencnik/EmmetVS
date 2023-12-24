using Community.VisualStudio.Toolkit;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.Globalization;
using System.Linq;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the increment/decrement helper class.
/// </summary>
public static class IncrementDecrementHelper
{
    /// <summary>
    /// Represents the number operation delegate.
    /// </summary>
    /// <param name="number">Number</param>
    public delegate void NumberOperation(ref double number);

    /// <summary>
    /// Increments the number under the caret by a given value.
    /// </summary>
    /// <param name="operation">Action object of a certain type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ProcessNumberInLineAsync(NumberOperation operation)
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

        if (caretPos == lineText.Length || (!char.IsDigit(lineText[caretPos]) && caretPos > 0 && char.IsDigit(lineText[caretPos - 1])))
            caretPos--;

        var numPosStart = -1;
        var numPosEnd = -1;

        // Search left
        for (int i = caretPos; i >= 0; i--)
        {
            if (char.IsDigit(lineText[i]) || lineText[i] == '.' || lineText[i] == ',')
                numPosStart = i;
            else
                break;
        }

        // Search right
        for (int i = caretPos; i < lineText.Length; i++)
        {
            if (char.IsDigit(lineText[i]) || lineText[i] == '.' || lineText[i] == ',')
                numPosEnd = i;
            else
                break;
        }

        var segment = lineText.Substring(numPosStart, (numPosEnd - numPosStart) + 1);
        var parsedSegment = segment.Replace(',', '.');
        double.TryParse(parsedSegment, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedNumber);

        operation(ref parsedNumber);  // Apply the operation

        char separator = segment.Contains(',') ? ',' : '.';
        var numberText = (parsedNumber % 1 == 0)
            ? ((int)parsedNumber).ToString()
            : parsedNumber.ToString($"F1", separator == ',' ? new CultureInfo("fr-FR") : CultureInfo.InvariantCulture);

        var spanToReplace = new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + numPosStart, numPosEnd - numPosStart + 1);
        using (var edit = docView.TextView.TextBuffer.CreateEdit())
        {
            edit.Replace(spanToReplace, numberText);
            edit.Apply();
        }

        var newSpan = new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + numPosStart, numberText.Length);
        docView.TextView.Selection.Select(newSpan, isReversed: false);
        docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + numPosStart + numberText.Length));
    }
}
