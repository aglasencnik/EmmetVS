using System.Net.Http;
using System.Threading.Tasks;

namespace EmmetVS.Helpers;

/// <summary>
/// Represents the HTTP client helper class.
/// </summary>
internal static class HttpClientHelper
{
    /// <summary>
    /// Gets file from the specified URL.
    /// </summary>
    /// <param name="url">Url</param>
    /// <returns>Byte array</returns>
    internal static async Task<byte[]> GetFileAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            await ex.LogAsync();
            return [];
        }
    }
}
