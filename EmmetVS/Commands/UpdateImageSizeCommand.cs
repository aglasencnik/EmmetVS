using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualStudio.Text;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Update Image Size command.
/// </summary>
[Command(PackageIds.UpdateImageSizeCommand)]
internal sealed class UpdateImageSizeCommand(DIToolkitPackage package) : BaseDICommand(package)
{
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

            if (caretPos == lineText.Length)
                caretPos--;

            var urlPosStart = -1;
            var urlPosEnd = -1;
            var allowCaretAtEnd = false;

            var extractedUrl = string.Empty;
            var sourceType = ImageSourceType.None;
            var fileType = FileType.None;

            // Check for CSS URLs first
            var cssUrlMatch = Regex.Match(lineText, @"url\(['""]?(.*?)['""]?\)");
            if (cssUrlMatch.Success)
            {
                extractedUrl = cssUrlMatch.Groups[1].Value;
                urlPosStart = cssUrlMatch.Groups[1].Index;
                urlPosEnd = urlPosStart + extractedUrl.Length - 1;
                allowCaretAtEnd = true;
                fileType = FileType.Stylesheet;
            }

            // If not found check HTML
            if (string.IsNullOrWhiteSpace(extractedUrl))
            {
                var srcMatch = Regex.Match(lineText, @"src=""([^""]+)""");
                if (srcMatch.Success)
                {
                    extractedUrl = srcMatch.Groups[1].Value;
                    urlPosStart = srcMatch.Groups[1].Index;
                    urlPosEnd = urlPosStart + extractedUrl.Length - 1;
                    allowCaretAtEnd = true;
                    fileType = FileType.Markup;
                }
            }

            if (extractedUrl.StartsWith("data:"))
                sourceType = ImageSourceType.Base64;
            else if (Uri.TryCreate(extractedUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                sourceType = ImageSourceType.RemoteFile;
            else
                sourceType = ImageSourceType.LocalFile;

            // Check if extracted UTR is empty or caret is not inside URL
            if (string.IsNullOrWhiteSpace(extractedUrl) || sourceType == ImageSourceType.None || fileType == FileType.None || (caretPos > urlPosEnd && (!allowCaretAtEnd || caretPos > urlPosEnd + 1)))
                return;

            var size = new Size(0, 0);

            if (sourceType == ImageSourceType.LocalFile)
            {
                var activeDocumentPath = DocumentHelper.GetActiveDocumentPath();
                if (string.IsNullOrEmpty(activeDocumentPath))
                    return;

                var folderPath = Path.GetDirectoryName(activeDocumentPath);
                var filePath = Path.Combine(folderPath, extractedUrl);

                if (!File.Exists(filePath))
                    return;

                using var img = Image.FromFile(filePath);
                if (img is null)
                    return;

                size = img.Size;
            }
            else if (sourceType == ImageSourceType.RemoteFile)
            {
                var fileBytes = await HttpClientHelper.GetFileAsync(extractedUrl);
                if (fileBytes is null)
                    return;

                using var ms = new MemoryStream(fileBytes);
                using var img = Image.FromStream(ms);
                if (img is null)
                    return;

                size = img.Size;
            }
            else if (sourceType == ImageSourceType.Base64)
            {
                var fileBytes = Convert.FromBase64String(extractedUrl.Substring(extractedUrl.IndexOf(',') + 1));
                if (fileBytes is null)
                    return;

                using var ms = new MemoryStream(fileBytes);
                using var img = Image.FromStream(ms);
                if (img is null)
                    return;

                size = img.Size;
            }

            if (fileType == FileType.Markup)
            {
                var imgTagMatch = Regex.Match(lineText, @"<img [^>]+>", RegexOptions.IgnoreCase);
                if (!imgTagMatch.Success)
                    return;

                var modifiedImgTag = imgTagMatch.Value;

                var widthRegex = new Regex("width=\"[^\"]*\"", RegexOptions.IgnoreCase);
                var heightRegex = new Regex("height=\"[^\"]*\"", RegexOptions.IgnoreCase);

                var endPartMatch = new Regex(@"[^""]*$").Match(modifiedImgTag);
                var endPart = endPartMatch.Value;

                modifiedImgTag = modifiedImgTag.Substring(0, endPartMatch.Index);

                if (widthRegex.IsMatch(modifiedImgTag))
                    modifiedImgTag = widthRegex.Replace(modifiedImgTag, $"width=\"{size.Width}\"");
                else
                    modifiedImgTag += $" width=\"{size.Width}\"";

                if (heightRegex.IsMatch(modifiedImgTag))
                    modifiedImgTag = heightRegex.Replace(modifiedImgTag, $"height=\"{size.Height}\"");
                else
                    modifiedImgTag += $" height=\"{size.Height}\"";

                modifiedImgTag += endPart;

                var startOfTag = imgTagMatch.Index;
                var precedingWhitespaceLength = lineText.Substring(0, startOfTag).LastIndexOfAny([' ', '\t']) + 1;
                var precedingWhitespace = lineText.Substring(0, precedingWhitespaceLength);

                var modifiedLineText = precedingWhitespace +
                                       modifiedImgTag +
                                       lineText.Substring(imgTagMatch.Index + imgTagMatch.Length).TrimStart();

                docView.TextView.TextBuffer.Replace(new Span(currentLine.Start.Position, currentLine.Length), modifiedLineText);
            }
            else if (fileType == FileType.Stylesheet)
            {
                var entireContent = docView.TextView.TextBuffer.CurrentSnapshot.GetText();
                var cursorPosition = position.Position;

                var openBracePosition = entireContent.LastIndexOf('{', cursorPosition);
                if (openBracePosition == -1)
                    return;

                var closeBracePosition = entireContent.IndexOf('}', cursorPosition);
                if (closeBracePosition == -1)
                    return;

                var selectorStartPosition = entireContent.LastIndexOf('\n', openBracePosition);
                if (selectorStartPosition == -1) // If there's no newline, use the start of the document
                    selectorStartPosition = 0;
                else
                    selectorStartPosition++; // Skip the newline character

                var cssBlock = entireContent.Substring(selectorStartPosition, (closeBracePosition - selectorStartPosition) + 1);

                var propertyRegex = new Regex(@"(?<name>[\w-]+)\s*:\s*(?<value>([^;]+;?)+);");
                var properties = propertyRegex.Matches(cssBlock)
                    .Cast<Match>()
                    .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value);
                properties["width"] = $"{size.Width}px";
                properties["height"] = $"{size.Height}px";

                var indentation = StylesheetHelper.GetIndentation(cssBlock);
                var formattedCssBlock = StylesheetHelper.FormatRule(cssBlock.Substring(0, cssBlock.IndexOf('{')).Trim(), properties, indentation);

                using var edit = docView.TextView.TextBuffer.CreateEdit();
                edit.Replace(new Span(selectorStartPosition, (closeBracePosition - selectorStartPosition) + 1), formattedCssBlock);
                edit.Apply();
            }

            var point = new SnapshotPoint(docView.TextView.TextSnapshot, currentLine.Start + caretPos);
            docView.TextView.Caret.MoveTo(point);
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while updating image size.");
        }
    }
}
