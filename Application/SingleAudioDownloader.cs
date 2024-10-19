using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.Application;

public class SingleAudioDownloader : AudioDownloader
{
    public async Task DownloadAudioAsync(string videoUrl)
    {
        var youtube = new YoutubeClient(ReadCookies());

        // Obtém as informações do vídeo
        var video = await youtube.Videos.GetAsync(videoUrl);
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

        // Obtém o áudio com maior taxa de bits
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        // Define o nome do arquivo com base no título do vídeo
        var title = SanitizeTitle(video.Title);
        var originalAudioPath = Path.Combine(Environment.CurrentDirectory, DEFAULT_RAW_DOWNLOADED_DIRECTORY, $"{title}.{RAW_FORMAT}");
        var convertedAudioPath = Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, $"{title}.{DEFAULT_OUTPUT_FORMAT}");

        // Baixa o áudio e mostra o progresso
        var progressHandler = new Progress<double>(progress =>
        {
            Console.Clear();
            Console.WriteLine($"Baixando '{video.Title}' -  {progress:P2}");
        });
        await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, originalAudioPath, progressHandler);

        Console.WriteLine($"Áudio salvo em: {originalAudioPath}");

        // Obtém informações do autor (artista) e canal (álbum)
        string artist = video.Author.ChannelTitle; // O nome do autor do vídeo (artista)
        string album = video.Author.ChannelTitle;  // O nome do canal como álbum

        var audioConverter = new AudioConverter();
        audioConverter.ConvertToMp3(originalAudioPath, convertedAudioPath);

        // Adiciona metadados ao arquivo de áudio
        var metadataEditor = new AudioMetadataEditor();
        metadataEditor.AddMetadata(convertedAudioPath, video.Title, artist, album);

        ClearRawDowloadDirectory();

    }
}
