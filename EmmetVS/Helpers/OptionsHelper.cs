using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Options;
using System.Linq;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the options helper class.
/// </summary>
internal static class OptionsHelper
{
    /// <summary>
    /// Gets the abbreviation options from the options page.
    /// </summary>
    /// <returns>AbbreviationOptions object.</returns>
    internal static AbbreviationOptions GetAbbreviationOptions()
    {
        var options = ConfigurationOptions.Instance;

        return new AbbreviationOptions
        {
            InlineElements = options.InlineElements.Split(',').Select(x => x.Trim()).ToArray(),
            OutputIndent = options.OutputIndent,
            OutputBaseIndent = options.OutputBaseIndent,
            OutputNewLine = options.OutputNewLine,
            OutputTagCase = options.OutputTagCase == StringCase.AsIs ? "" :
                            options.OutputTagCase == StringCase.Lower ? "lower" :
                            "upper",
            OutputAttributeCase = options.OutputAttributeCase == StringCase.AsIs ? "" :
                                  options.OutputAttributeCase == StringCase.Lower ? "lower" :
                                  "upper",
            OutputAttributeQuotes = options.OutputAttributeQuotes == AttributeQuotes.Single ? "single" : "double",
            OutputFormat = options.OutputFormat,
            OutputFormatLeafNode = options.OutputFormatLeafNode,
            OutputFormatSkip = options.OutputFormatSkip.Split(',').Select(x => x.Trim()).ToArray(),
            OutputFormatForce = options.OutputFormatForce.Split(',').Select(x => x.Trim()).ToArray(),
            OutputInlineBreak = options.OutputInlineBreak,
            OutputCompactBoolean = options.OutputCompactBoolean,
            OutputBooleanAttributes = options.OutputBooleanAttributes.Split(',').Select(x => x.Trim()).ToArray(),
            OutputReverseAttributes = options.OutputReverseAttributes,
            OutputSelfClosingStyle = options.OutputSelfClosingStyle == SelfClosingStyle.Xhtml ? "xhtml" :
                                     options.OutputSelfClosingStyle == SelfClosingStyle.Xml ? "xml" : 
                                     "html",
            MarkupHref = options.MarkupHref,
            MarkupAttrributes = options.MarkupAttrributes
                .Split(',')
                .Select(x => x.Trim().Split(':'))
                .Where(x => x.Length == 2)
                .ToDictionary(
                    x => x[0].Trim(), 
                    x => x[1].Trim()
                ),
            MarkupValuePrefix = options.MarkupValuePrefix
                .Split(',')
                .Select(x => x.Trim().Split(':'))
                .Where(x => x.Length == 2)
                .ToDictionary(
                    x => x[0].Trim(),
                    x => x[1].Trim()
                ),
            CommentEnabled = options.CommentEnabled,
            CommentTrigger = options.CommentTrigger.Split(',').Select(x => x.Trim()).ToArray(),
            CommentBefore = options.CommentBefore,
            CommentAfter = options.CommentAfter,
            BemEnabled = options.BemEnabled,
            BemElement = options.BemElement,
            BemModifier = options.BemModifier,
            JsxEnabled = options.JsxEnabled,
            StylesheetKeywords = options.StylesheetKeywords.Split(',').Select(x => x.Trim()).ToArray(),
            StylesheetUnitless = options.StylesheetUnitless.Split(',').Select(x => x.Trim()).ToArray(),
            StylesheetShortHex = options.StylesheetShortHex,
            StylesheetBetween = options.StylesheetBetween,
            StylesheetAfter = options.StylesheetAfter,
            StylesheetIntUnit = options.StylesheetIntUnit,
            StylesheetFloatUnit = options.StylesheetFloatUnit,
            StylesheetUnitAliases = options.StylesheetUnitAliases
                .Split(',')
                .Select(x => x.Trim().Split(':'))
                .Where(x => x.Length == 2)
                .ToDictionary(
                    x => x[0].Trim(),
                    x => x[1].Trim()
                ),
            StylesheetJson = options.StylesheetJson,
            StylesheetJsonDoubleQuotes = options.StylesheetJsonDoubleQuotes,
            StylesheetFuzzySearchMinScore = options.StylesheetFuzzySearchMinScore,
        };
    }
}
