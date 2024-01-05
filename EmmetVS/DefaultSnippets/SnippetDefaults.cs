using System.IO;

namespace EmmetVS.DefaultSnippets;

/// <summary>
/// Represents the snippet locations
/// </summary>
public static class SnippetDefaults
{
    /// <summary>
    /// Gets the default snippets directory name
    /// </summary>
    public static string DefaultSnippetsDirectory = "DefaultSnippets";

    /// <summary>
    /// Gets the HTML snippets file location
    /// </summary>
    public static string HtmlSnippetsLocation = Path.Combine(DefaultSnippetsDirectory, "html-snippets.json");

    /// <summary>
    /// Gets the HTML supported file types file location
    /// </summary>
    public static string HtmlSupportedFileTypesLocation = Path.Combine(DefaultSnippetsDirectory, "html-supported-file-types.json");

    /// <summary>
    /// Gets the CSS snippets file location
    /// </summary>
    public static string CssSnippetsLocation = Path.Combine(DefaultSnippetsDirectory, "css-snippets.json");

    /// <summary>
    /// Gets the CSS supported file types file location
    /// </summary>
    public static string CssSupportedFileTypesLocation = Path.Combine(DefaultSnippetsDirectory, "css-supported-file-types.json");

    /// <summary>
    /// Gets the XSL snippets file location
    /// </summary>
    public static string XslSnippetsLocation = Path.Combine(DefaultSnippetsDirectory, "xsl-snippets.json");

    /// <summary>
    /// Gets the XSL supported file types file location
    /// </summary>
    public static string XslSupportedFileTypesLocation = Path.Combine(DefaultSnippetsDirectory, "xsl-supported-file-types.json");

    /// <summary>
    /// Gets the variables file location
    /// </summary>
    public static string VariablesLocation = Path.Combine(DefaultSnippetsDirectory, "variables.json");
}
