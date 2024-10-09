using System.Net;

namespace YouTubeDownloader.Application;

public class AudioDownloader
{
    protected readonly string DEFAULT_OUTPUT_DIRECTORY = "Downloads/Audio";
    protected readonly string DEFAULT_OUTPUT_FORMAT = "mp3";
    protected readonly string cookieFilePath = "cookies.txt";

    protected List<Cookie> ReadCookies()
    {
        var cookies = new List<Cookie>();

        if (File.Exists(cookieFilePath))
        {
            string content = File.ReadAllText(cookieFilePath);
            var cookiePairs = content.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in cookiePairs)
            {
                var keyValue = pair.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    cookies.Add(new Cookie(keyValue[0], keyValue[1], "/", "youtube.com"));
                }
            }
        }

        return cookies;
    }
}
