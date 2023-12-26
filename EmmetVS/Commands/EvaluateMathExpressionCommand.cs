using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetNetSharp.Interfaces;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.Globalization;
using System.Linq;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Evaluate Math Expression command.
/// </summary>
[Command(PackageIds.EvaluateMathExpressionCommand)]
internal sealed class EvaluateMathExpressionCommand : BaseDICommand
{
    private readonly IMathExpressionService _mathExpressionService;

    public EvaluateMathExpressionCommand(DIToolkitPackage package,
        IMathExpressionService mathExpressionService) : base(package)
    {
        _mathExpressionService = mathExpressionService;
    }

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

            if (caretPos == lineText.Length || (!char.IsDigit(lineText[caretPos]) && caretPos > 0 && char.IsDigit(lineText[caretPos - 1])))
                caretPos--;

            var numPosStart = -1;
            var numPosEnd = -1;

            // Search left
            for (int i = caretPos; i >= 0; i--)
            {
                if (char.IsDigit(lineText[i]) || lineText[i] == '.' || lineText[i] == ',' || lineText[i] == '+' || lineText[i] == '-' || lineText[i] == '/' || lineText[i] == '*' || lineText[i] == '\\' || lineText[i] == '(' || lineText[i] == ')')
                    numPosStart = i;
                else
                    break;
            }

            // Search right
            for (int i = caretPos; i < lineText.Length; i++)
            {
                if (char.IsDigit(lineText[i]) || lineText[i] == '.' || lineText[i] == ',' || lineText[i] == '+' || lineText[i] == '-' || lineText[i] == '/' || lineText[i] == '*' || lineText[i] == '\\' || lineText[i] == '(' || lineText[i] == ')')
                    numPosEnd = i;
                else
                    break;
            }

            var segment = lineText.Substring(numPosStart, (numPosEnd - numPosStart) + 1);
            segment = MathExpressionEvaluationHelper.CustomTrim(segment, ['+', '/', '*', '\\'], ['+', '-', '/', '*', '\\']);
            var parsedSegment = segment.Replace(',', '.');

            var expression = _mathExpressionService.Extract(parsedSegment);
            if (expression is null)
                return;

            var result = _mathExpressionService.Evaluate(parsedSegment.Substring(expression.Value.Item1, expression.Value.Item2 - expression.Value.Item1));
            if (result is null)
                return;

            var separator = segment.Contains(',') ? ',' : '.';
            var numberText = (result % 1 == 0)
                ? ((int)result).ToString()
                : result.Value.ToString($"F2", separator == ',' ? new CultureInfo("fr-FR") : CultureInfo.InvariantCulture);

            var spanToReplace = new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + numPosStart, numPosEnd - numPosStart + 1);
            using var edit = docView.TextView.TextBuffer.CreateEdit();
            edit.Replace(spanToReplace, numberText);
            edit.Apply();
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while evaluating math expression.");
        }
    }
}
