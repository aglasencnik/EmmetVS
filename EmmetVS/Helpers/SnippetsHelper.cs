using EmmetVS.Models;
using EmmetVS.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the snippets helper.
/// </summary>
internal static class SnippetsHelper
{
    /// <summary>
    /// Gets the name of the folder that contains the snippets.
    /// </summary>
    private const string SnippetsFolderName = "Snippets";

    /// <summary>
    /// Gets the name of the placeholder snippet.
    /// </summary>
    private const string PlaceholderSnippetName = "placeholder.snippet";

    /// <summary>
    /// Gets the name of the folder that contains the CSS snippets.
    /// </summary>
    private const string CssSnippetsFolderName = "EmmetVS CSS Snippets";

    /// <summary>
    /// Gets the name of the folder that contains the HTML snippets.
    /// </summary>
    private const string HtmlSnippetsFolderName = "EmmetVS HTML Snippets";

    /// <summary>
    /// Gets the name of the folder that contains the XML snippets.
    /// </summary>
    private const string XslSnippetsFolderName = "EmmetVS XML Snippets";

    /// <summary>
    /// Removes the placeholder snippets.
    /// </summary>
    internal static void RemovePlaceholderSnippets()
    {
        var extensionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var cssSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, CssSnippetsFolderName);
        var htmlSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, HtmlSnippetsFolderName);
        var xslSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, XslSnippetsFolderName);

        if (Directory.Exists(cssSnippetsPath) && File.Exists(Path.Combine(cssSnippetsPath, PlaceholderSnippetName)))
            File.Delete(Path.Combine(cssSnippetsPath, PlaceholderSnippetName));

        if (Directory.Exists(htmlSnippetsPath) && File.Exists(Path.Combine(htmlSnippetsPath, PlaceholderSnippetName)))
            File.Delete(Path.Combine(htmlSnippetsPath, PlaceholderSnippetName));

        if (Directory.Exists(xslSnippetsPath) && File.Exists(Path.Combine(xslSnippetsPath, PlaceholderSnippetName)))
            File.Delete(Path.Combine(xslSnippetsPath, PlaceholderSnippetName));
    }

    /// <summary>
    /// Updates the snippets.
    /// </summary>
    internal static void UpdateSnippets()
    {
        // Get paths to snippets folders
        var extensionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var cssSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, CssSnippetsFolderName);
        var htmlSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, HtmlSnippetsFolderName);
        var xslSnippetsPath = Path.Combine(extensionPath, SnippetsFolderName, XslSnippetsFolderName);

        if (GeneralOptions.Instance.EnableSnippets)
        {
            // Get snippets from options
            var cssSnippets = GetRealSnippets(CssOptions.Instance.Snippets);
            var htmlSnippets = GetRealSnippets(HtmlOptions.Instance.Snippets);
            var xslSnippets = GetRealSnippets(XslOptions.Instance.Snippets);

            // Create snippets folders if they don't exist
            if (!Directory.Exists(cssSnippetsPath))
                Directory.CreateDirectory(cssSnippetsPath);

            if (!Directory.Exists(htmlSnippetsPath))
                Directory.CreateDirectory(htmlSnippetsPath);

            if (!Directory.Exists(xslSnippetsPath))
                Directory.CreateDirectory(xslSnippetsPath);

            // Get existing snippets
            var cssSnippetsFiles = Directory.GetFiles(cssSnippetsPath, "*.snippet");
            var htmlSnippetsFiles = Directory.GetFiles(htmlSnippetsPath, "*.snippet");
            var xslSnippetsFiles = Directory.GetFiles(xslSnippetsPath, "*.snippet");

            // Get existing snippet file names
            var cssOldSnippetsFilesNames = cssSnippetsFiles.Select(f => Path.GetFileNameWithoutExtension(f));
            var htmOldlSnippetsFilesNames = htmlSnippetsFiles.Select(f => Path.GetFileNameWithoutExtension(f));
            var xslOldSnippetsFilesNames = xslSnippetsFiles.Select(f => Path.GetFileNameWithoutExtension(f));

            // Get new snippet file names
            var cssNewSnippetsFilesNames = cssSnippets.Select(s => SanitizeFileName(s.Key));
            var htmlNewSnippetsFilesNames = htmlSnippets.Select(s => SanitizeFileName(s.Key));
            var xslNewSnippetsFilesNames = xslSnippets.Select(s => SanitizeFileName(s.Key));

            // Get snippets to remove
            var cssSnippetsToRemove = cssOldSnippetsFilesNames.Except(cssNewSnippetsFilesNames);
            var htmlSnippetsToRemove = htmOldlSnippetsFilesNames.Except(htmlNewSnippetsFilesNames);
            var xslSnippetsToRemove = xslOldSnippetsFilesNames.Except(xslNewSnippetsFilesNames);

            // Remove snippets
            foreach (var snippet in cssSnippetsToRemove)
            {
                File.Delete(Path.Combine(cssSnippetsPath, $"{snippet}.snippet"));
            }

            foreach (var snippet in htmlSnippetsToRemove)
            {
                File.Delete(Path.Combine(htmlSnippetsPath, $"{snippet}.snippet"));
            }

            foreach (var snippet in xslSnippetsToRemove)
            {
                File.Delete(Path.Combine(xslSnippetsPath, $"{snippet}.snippet"));
            }

            // Get snippets to add
            var cssSnippetsToAdd = cssNewSnippetsFilesNames.Except(cssOldSnippetsFilesNames);
            var htmlSnippetsToAdd = htmlNewSnippetsFilesNames.Except(htmOldlSnippetsFilesNames);
            var xslSnippetsToAdd = xslNewSnippetsFilesNames.Except(xslOldSnippetsFilesNames);

            // Add snippets
            foreach (var snippet in cssSnippetsToAdd)
            {
                var snippetCode = cssSnippets.FirstOrDefault(s => SanitizeFileName(s.Key) == snippet).Value;
                var parsedCode = ParseCode(snippetCode);
                var snippetObject = new Snippet
                {
                    Name = snippet,
                    Author = "EmmetVS",
                    Prefix = snippet,
                    Code = parsedCode.Item1,
                    Language = "CSS",
                    Literals = parsedCode.Item2
                };

                var snippetXml = SerializeSnippet(snippetObject);
                File.WriteAllText(Path.Combine(cssSnippetsPath, $"{snippet}.snippet"), snippetXml);
            }

            foreach (var snippet in htmlSnippetsToAdd)
            {
                var snippetCode = htmlSnippets.FirstOrDefault(s => SanitizeFileName(s.Key) == snippet).Value;
                var parsedCode = ParseCode(snippetCode);
                var snippetObject = new Snippet
                {
                    Name = snippet,
                    Author = "EmmetVS",
                    Prefix = snippet,
                    Code = parsedCode.Item1,
                    Language = "HTML",
                    Literals = parsedCode.Item2
                };

                var snippetXml = SerializeSnippet(snippetObject);
                File.WriteAllText(Path.Combine(htmlSnippetsPath, $"{snippet}.snippet"), snippetXml);
            }

            foreach (var snippet in xslSnippetsToAdd)
            {
                var snippetCode = xslSnippets.FirstOrDefault(s => SanitizeFileName(s.Key) == snippet).Value;
                var parsedCode = ParseCode(snippetCode);
                var snippetObject = new Snippet
                {
                    Name = snippet,
                    Author = "EmmetVS",
                    Prefix = snippet,
                    Code = parsedCode.Item1,
                    Language = "XML",
                    Literals = parsedCode.Item2
                };

                var snippetXml = SerializeSnippet(snippetObject);
                File.WriteAllText(Path.Combine(xslSnippetsPath, $"{snippet}.snippet"), snippetXml);
            }
        }
        else
        {
            if (Directory.Exists(cssSnippetsPath))
            {
                var snippets = Directory.GetFiles(cssSnippetsPath, "*.snippet");
                foreach (var snippet in snippets)
                {
                    File.Delete(snippet);
                }
            }

            if (Directory.Exists(htmlSnippetsPath))
            {
                var snippets = Directory.GetFiles(htmlSnippetsPath, "*.snippet");
                foreach (var snippet in snippets)
                {
                    File.Delete(snippet);
                }
            }

            if (Directory.Exists(xslSnippetsPath))
            {
                var snippets = Directory.GetFiles(xslSnippetsPath, "*.snippet");
                foreach (var snippet in snippets)
                {
                    File.Delete(snippet);
                }
            }
        }
    }

    /// <summary>
    /// Gets the snippets.
    /// </summary>
    /// <param name="originalSnippets">Original snippets dictionary</param>
    /// <returns>A dictionary of real snippets.</returns>
    private static Dictionary<string, string> GetRealSnippets(Dictionary<string, string> originalSnippets)
    {
        var realSnippets = new Dictionary<string, string>();

        foreach (var snippet in originalSnippets)
        {
            var snippetNames = snippet.Key.Split('|');
            foreach (var snippetName in snippetNames)
            {
                if (!realSnippets.ContainsKey(snippetName))
                {
                    realSnippets.Add(snippetName, snippet.Value);
                }
            }
        }

        return realSnippets;
    }

    /// <summary>
    /// Sanitizes the file name.
    /// </summary>
    /// <param name="fileName">Proposed filename</param>
    /// <returns>Sanitized file name string.</returns>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(ch => !invalidChars.Contains(ch)).ToArray());
    }

    /// <summary>
    /// Serializes the snippet to xml.
    /// </summary>
    /// <param name="snippet">Snippet object</param>
    /// <returns>String containing snippet in xml form.</returns>
    private static string SerializeSnippet(Snippet snippet)
    {
        try
        {
            XNamespace ns = "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet";

            var declarations = new List<XElement>();

            if (snippet.Literals != null && snippet.Literals.Length > 0)
            {
                declarations.Add(new XElement(ns + "Declarations",
                    snippet.Literals.Select(literal =>
                        new XElement(ns + "Literal",
                            new XElement(ns + "ID", literal.Id ?? string.Empty),
                            new XElement(ns + "Default", literal.Default ?? string.Empty)
                        )
                    )
                ));
            }

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(ns + "CodeSnippets",
                    new XElement(ns + "CodeSnippet",
                        new XAttribute("Format", "1.0.0"),
                        new XElement(ns + "Header",
                            new XElement(ns + "Title", snippet.Name ?? string.Empty),
                            new XElement(ns + "Author", snippet.Author ?? string.Empty),
                            new XElement(ns + "Shortcut", snippet.Prefix ?? string.Empty)
                        ),
                        new XElement(ns + "Snippet",
                            new XElement(ns + "Code",
                                new XAttribute("Language", snippet.Language ?? string.Empty),
                                new XCData(snippet.Code ?? string.Empty)
                            ),
                            declarations
                        )
                    )
                )
            );

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true }))
                    {
                        doc.WriteTo(xmlWriter);
                    }
                }

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Parses the snippet code.
    /// </summary>
    /// <param name="code">Snippet code string</param>
    /// <returns>A tuple containing parsed code string and an array of Literal objects.</returns>
    public static (string, Literal[]) ParseCode(string code)
    {
        try
        {
            if (code.StartsWith("{") && code.EndsWith("}"))
                code = code.Substring(1, code.Length - 2);

            var regex = new Regex(@"\$\{(\d+)(?:\:([^}|]+))?(\|([^}]+)\|)?\}|(\$0)");

            var literalsDict = new Dictionary<string, Literal>();

            var vsFormatted = regex.Replace(code, match =>
            {
                // Handle the special case of $0 (final cursor position in VS Code)
                if (match.Value == "$0")
                    return "$end$";

                var id = match.Groups[1].Value;
                var defaultValue = match.Groups[2].Value;

                // Handle choices: We'll default to the first choice for VS 
                // as VS doesn't support choices directly in the snippet
                if (string.IsNullOrWhiteSpace(defaultValue))
                {
                    var choices = match.Groups[4].Value?.Split(',');
                    if (choices != null && choices.Length > 0)
                    {
                        defaultValue = choices[0];
                    }
                }

                if (string.IsNullOrWhiteSpace(defaultValue))
                    defaultValue = "value";

                if (!literalsDict.ContainsKey(id))
                {
                    literalsDict[id] = new Literal { Id = id, Default = defaultValue };
                }

                return $"${id}$";
            });

            return (vsFormatted, literalsDict.Values.ToArray());
        }
        catch
        {
            return (string.Empty, Array.Empty<Literal>());
        }
    }
}
