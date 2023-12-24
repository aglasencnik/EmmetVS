using Community.VisualStudio.Toolkit;
using Community.VisualStudio.Toolkit.DependencyInjection;
using Community.VisualStudio.Toolkit.DependencyInjection.Core;
using EmmetVS.Enums;
using EmmetVS.Helpers;
using EmmetVS.Options;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace EmmetVS.Commands;

/// <summary>
/// Represents the Encode/Decode Image to Data URL command.
/// </summary>
[Command(PackageIds.EncodeDecodeImageToDataURLCommand)]
internal sealed class EncodeDecodeImageToDataURLCommand(DIToolkitPackage package) : BaseDICommand(package)
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

            // Check for CSS URLs first
            var cssUrlMatch = Regex.Match(lineText, @"url\(['""]?(.*?)['""]?\)");
            if (cssUrlMatch.Success)
            {
                extractedUrl = cssUrlMatch.Groups[1].Value;
                urlPosStart = cssUrlMatch.Groups[1].Index;
                urlPosEnd = urlPosStart + extractedUrl.Length - 1;
                allowCaretAtEnd = true;
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
                }
            }

            if (extractedUrl.StartsWith("data:"))
                sourceType = ImageSourceType.Base64;
            else if (Uri.TryCreate(extractedUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                sourceType = ImageSourceType.RemoteFile;
            else
                sourceType = ImageSourceType.LocalFile;

            // Check if extracted UTR is empty or caret is not inside URL
            if (string.IsNullOrWhiteSpace(extractedUrl) || sourceType == ImageSourceType.None || caretPos < urlPosStart || (caretPos > urlPosEnd && (!allowCaretAtEnd || caretPos > urlPosEnd + 1)))
                return;
            var returnedUrl = string.Empty;

            if (sourceType == ImageSourceType.LocalFile)
            {
                var activeDocumentPath = DocumentHelper.GetActiveDocumentPath();
                if (string.IsNullOrEmpty(activeDocumentPath))
                    return;

                var folderPath = Path.GetDirectoryName(activeDocumentPath);
                var filePath = Path.Combine(folderPath, extractedUrl);

                if (!File.Exists(filePath))
                    return;

                var fileBytes = File.ReadAllBytes(filePath);
                if (fileBytes is null)
                    return;

                var base64String = Convert.ToBase64String(fileBytes);
                if (string.IsNullOrWhiteSpace(base64String))
                    return;

                returnedUrl = $"data:image/{Path.GetExtension(extractedUrl).ToLowerInvariant().Replace(".", "")};base64,{base64String}";
            }
            else if (sourceType == ImageSourceType.RemoteFile)
            {
                var fileBytes = await HttpClientHelper.GetFileAsync(extractedUrl);
                if (fileBytes is null)
                    return;

                var base64String = Convert.ToBase64String(fileBytes);
                if (string.IsNullOrWhiteSpace(base64String))
                    return;

                returnedUrl = $"data:image/{Path.GetExtension(extractedUrl).ToLowerInvariant().Replace(".", "")};base64,{base64String}";
            }
            else if (sourceType == ImageSourceType.Base64)
            {
                var fileBytes = Convert.FromBase64String(extractedUrl.Substring(extractedUrl.IndexOf(',') + 1));

                returnedUrl = Interaction.InputBox("Enter path to file (absolute or relative):", "Path to file");
                if (string.IsNullOrWhiteSpace(returnedUrl))
                    return;

                var activeDocumentDir = Path.GetDirectoryName(DocumentHelper.GetActiveDocumentPath());

                string fullPath;

                if (returnedUrl.StartsWith("/"))
                    fullPath = Path.GetFullPath(Path.Combine(activeDocumentDir, returnedUrl.TrimStart('/')));

                else if (returnedUrl.StartsWith("..") || !Path.IsPathRooted(returnedUrl))
                    fullPath = Path.GetFullPath(Path.Combine(activeDocumentDir, returnedUrl));
                else
                    fullPath = returnedUrl;

                var folderPath = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                File.WriteAllBytes(fullPath, fileBytes);
            }

            var spanToReplace = new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + urlPosStart, urlPosEnd - urlPosStart + 1);
            using (var edit = docView.TextView.TextBuffer.CreateEdit())
            {
                edit.Replace(spanToReplace, returnedUrl);
                edit.Apply();
            }

            var newSpan = new SnapshotSpan(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + urlPosStart, returnedUrl.Length);
            docView.TextView.Selection.Select(newSpan, isReversed: false);
            docView.TextView.Caret.MoveTo(new SnapshotPoint(docView.TextView.TextBuffer.CurrentSnapshot, currentLine.Start + urlPosStart));
        }
        catch (Exception ex)
        {
            await ex.LogAsync("Error while encoding/decoding image to data:URL.");
        }
    }
}
