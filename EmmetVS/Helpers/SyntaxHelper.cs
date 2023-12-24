using EmmetVS.Enums;
using System.Linq;

namespace EmmetVS.Helpers;

internal static class SyntaxHelper
{
    /// <summary>
    /// Gets the list of markup syntaxes supported by Emmet.
    /// </summary>
    /// <returns>Array of markup syntaxes.</returns>
    internal static string[] GetMarkupSyntaxes() => new[] { "html", "xml", "xsl", "jsx", "js", "pug", "slim", "haml", "vue", "svelte" };

    /// <summary>
    /// Gets the list of stylesheet syntaxes supported by Emmet.
    /// </summary>
    /// <returns>Array of stylesheet syntaxes.</returns>
    internal static string[] GetStylesheetSyntaxes() => new[] { "css", "scss", "sass", "less", "sss", "stylus" };

    /// <summary>
    /// Gets the syntax type of the document.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>FileType</returns>
    internal static FileType GetSyntaxType(string extension)
    {
        if (GetMarkupSyntaxes().Contains(extension.TrimStart('.')))
            return FileType.Markup;

        if (GetStylesheetSyntaxes().Contains(extension.TrimStart('.')))
            return FileType.Stylesheet;

        return FileType.None;
    }
}
