using System.IO;

namespace EmmetVS.Snippets;

/// <summary>
/// Represents the snippet locations
/// </summary>
public static class SnippetDefaults
{
    /// <summary>
    /// Gets the snippets directory name
    /// </summary>
    public static string SnippetsDirectory = "Snippets";

    /// <summary>
    /// Gets the HTML snippets file location
    /// </summary>
    public static string HtmlSnippetsLocation = Path.Combine(SnippetsDirectory, "html-snippets.json");

    /// <summary>
    /// Gets the HTML supported file types file location
    /// </summary>
    public static string HtmlSupportedFileTypesLocation = Path.Combine(SnippetsDirectory, "html-supported-file-types.json");

    /// <summary>
    /// Gets the CSS snippets file location
    /// </summary>
    public static string CssSnippetsLocation = Path.Combine(SnippetsDirectory, "css-snippets.json");

    /// <summary>
    /// Gets the CSS supported file types file location
    /// </summary>
    public static string CssSupportedFileTypesLocation = Path.Combine(SnippetsDirectory, "css-supported-file-types.json");

    /// <summary>
    /// Gets the XSL snippets file location
    /// </summary>
    public static string XslSnippetsLocation = Path.Combine(SnippetsDirectory, "xsl-snippets.json");

    /// <summary>
    /// Gets the XSL supported file types file location
    /// </summary>
    public static string XslSupportedFileTypesLocation = Path.Combine(SnippetsDirectory, "xsl-supported-file-types.json");

    /// <summary>
    /// Gets the variables file location
    /// </summary>
    public static string VariablesLocation = Path.Combine(SnippetsDirectory, "variables.json");
}
