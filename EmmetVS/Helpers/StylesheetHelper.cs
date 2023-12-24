using EmmetVS.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the Stylesheet helper class.
/// </summary>
internal static class StylesheetHelper
{
    /// <summary>
    /// Generates vendor properties.
    /// </summary>
    /// <param name="selector">Selector</param>
    /// <param name="properties">Properties</param>
    /// <param name="baseProperty">Base property</param>
    /// <param name="indentation">Indentation</param>
    /// <returns>CSS string</returns>
    internal static string GenerateVendorProperties(string selector, Dictionary<string, string> properties, string baseProperty, string indentation)
    {
        var formattedRule = new StringBuilder();
        formattedRule.AppendLine(indentation + selector + " {");

        var vendorOrder = new List<string> { "-webkit-", "-moz-", "-ms-", "-o-" };

        var orderedKeys = properties.Keys.ToList();

        orderedKeys.Remove(baseProperty);
        orderedKeys.Add(baseProperty);

        foreach (var prefix in vendorOrder)
        {
            var vendorKey = prefix + baseProperty;
            orderedKeys.Remove(vendorKey);
            orderedKeys.Insert(orderedKeys.IndexOf(baseProperty), vendorKey);
        }

        var options = ConfigurationOptions.Instance;

        foreach (var key in orderedKeys)
        {
            if (properties.ContainsKey(key))
                formattedRule.AppendLine(indentation + $"\t{key}{options.StylesheetBetween}{properties[key]}{options.StylesheetAfter}");
        }

        formattedRule.Append(indentation + "}");
        return formattedRule.ToString();
    }

    /// <summary>
    /// Gets the indentation.
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>A string</returns>
    internal static string GetIndentation(string text)
    {
        var match = Regex.Match(text, @"^[\t\s]*");
        return match.Value;
    }

    /// <summary>
    /// Formats stylesheet rule.
    /// </summary>
    /// <param name="selector">Selector name</param>
    /// <param name="properties">Properties</param>
    /// <param name="indentation">Indentation</param>
    /// <returns>Formatted stylesheet rule string</returns>
    public static string FormatRule(string selector, Dictionary<string, string> properties, string indentation)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"{indentation}{selector} {{");

        foreach (var property in properties)
        {
            builder.AppendLine($"{indentation}\t{property.Key}: {property.Value};");
        }

        builder.Append($"{indentation}}}");
        return builder.ToString();
    }
}
