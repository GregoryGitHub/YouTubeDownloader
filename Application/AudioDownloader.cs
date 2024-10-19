using System.Net;
using System.Text.RegularExpressions;

namespace YouTubeDownloader.Application;

public class AudioDownloader
{
    protected readonly string DEFAULT_OUTPUT_DIRECTORY = "G:/Meu Drive/Músicas";
    protected readonly string DEFAULT_RAW_DOWNLOADED_DIRECTORY = "G:/Meu Drive/Músicas/Raw";
    protected readonly string DEFAULT_OUTPUT_FORMAT = "mp3";
    protected readonly string RAW_FORMAT = "webm";
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

    protected string SanitizeTitle(string title)
    {
        return Regex.Replace(title, "[^0-9a-zA-ZóôõòáàâãèéêíìúùûÓÒÕÔÁÀÃÂÉÈÊÍÌÚÙÛçÇ\\s\\-]","");
    }

    protected void ClearRawDowloadDirectory()
    {
        // Verifica se o diretório existe
        if (Directory.Exists(DEFAULT_RAW_DOWNLOADED_DIRECTORY))
        {
            // Apaga todos os arquivos no diretório
            foreach (var file in Directory.GetFiles(DEFAULT_RAW_DOWNLOADED_DIRECTORY))
            {
                File.Delete(file);
            }

            // Apaga todos os subdiretórios e seus conteúdos
            foreach (var subDirectory in Directory.GetDirectories(DEFAULT_RAW_DOWNLOADED_DIRECTORY))
            {
                Directory.Delete(subDirectory, true); // true para excluir recursivamente
            }

            Console.WriteLine($"Conteúdo do diretório {DEFAULT_RAW_DOWNLOADED_DIRECTORY} apagado.");
        }
        else
        {
            Console.WriteLine($"O diretório {DEFAULT_RAW_DOWNLOADED_DIRECTORY} não existe.");
        }
    }
}
