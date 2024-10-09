using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.Application;

public class SingleAudioDownloader: AudioDownloader
{

    public async Task DownloadAudioAsync(string videoUrl)
    {
        var youtube = new YoutubeClient(ReadCookies());

        var video = await youtube.Videos.GetAsync(videoUrl);
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        var audioFilePath = Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, $"{video.Title}.{DEFAULT_OUTPUT_FORMAT}");

        var progressHandler = new Progress<double>(progress =>
        {
            Console.Clear();
            Console.WriteLine($"Baixando '{video.Title}' -  {progress:P2}");
        });

        await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioFilePath, progressHandler);

        Console.WriteLine($"Áudio salvo em: {audioFilePath}");
    }
}