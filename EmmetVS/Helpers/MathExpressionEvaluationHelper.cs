using System.Linq;

namespace EmmetVS.Helpers;

/// <summary>
/// Helper class for evaluating math expressions.
/// </summary>
internal static class MathExpressionEvaluationHelper
{
    /// <summary>
    /// Trims the input string by the given trim characters and returns the trimmed string.
    /// </summary>
    /// <param name="input">Input string</param>
    /// <param name="trimCharsStart">Which characters to trim at the start</param>
    /// <param name="trimCharsStart">Which characters to trim at the end</param>
    /// <returns>A trimmed input string</returns>
    internal static string CustomTrim(string input, char[] trimCharsStart, char[] trimCharsEnd)
    {
        int startIndex = 0;
        int endIndex = input.Length - 1;

        // Trim start
        while (startIndex <= endIndex && trimCharsStart.Contains(input[startIndex]))
        {
            startIndex++;
        }

        // Trim end
        while (endIndex >= startIndex && trimCharsEnd.Contains(input[endIndex]))
        {
            endIndex--;
        }

        // Return substring
        return startIndex <= endIndex ? input.Substring(startIndex, endIndex - startIndex + 1) : string.Empty;
    }
}
