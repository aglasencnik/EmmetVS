using EmmetNetSharp.Models;
using EmmetVS.Enums;
using EmmetVS.Options;
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
    /// <param name="fileType">File type</param>
    /// <returns>Syntax</returns>
    internal static string GetFileSyntax(string extension, FileType fileType)
    {
        return fileType switch
        {
            FileType.Markup => GetMarkupSyntaxes().FirstOrDefault(s => s == extension.TrimStart('.')),
            FileType.Stylesheet => GetStylesheetSyntaxes().FirstOrDefault(s => s == extension.TrimStart('.')),
            _ => fileType == FileType.Markup ? "html" : fileType == FileType.Stylesheet ? "css" : "",
        };
    }

    /// <summary>
    /// Gets the file type based on the file extension.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>FileType</returns>
    internal static FileType GetFileType(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return FileType.None;

        if (HtmlOptions.Instance.SupportedFileTypes.Contains(extension))
            return FileType.Markup;
        else if (CssOptions.Instance.SupportedFileTypes.Contains(extension))
            return FileType.Stylesheet;
        else if (XslOptions.Instance.SupportedFileTypes.Contains(extension))
            return FileType.Xsl;
        else
            return FileType.None;
    }
}
