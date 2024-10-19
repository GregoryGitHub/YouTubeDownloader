using System.Collections.Concurrent;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.Application;


public class PlayListAudioDownloader : AudioDownloader
{
    private ConcurrentDictionary<string, string> _progressDictionary = new ();
    
    public async Task DownloadPlaylistAsync(string playlistUrl)
    {
        var counter = 0;
        var youtube = new YoutubeClient(ReadCookies());
        
        var playListMetadata = await youtube.Playlists.GetAsync(playlistUrl);
        var totalVideos = playListMetadata.Count ?? 0;

        if(totalVideos == 0)
        {
            Console.WriteLine("A playlist não possui vídeos.");
            return;
        }

        // if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, DEFAULT_RAW_DOWNLOADED_DIRECTORY, playListMetadata.Title)))
        // {
        //     Console.WriteLine("A pasta já existe. A playlist não será baixada.");
        //     return;
        // }

        Console.WriteLine($"Baixando a playlist: {playListMetadata.Title}");
        IAsyncEnumerable<PlaylistVideo> videos = youtube.Playlists.GetVideosAsync(playlistUrl);
        
        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, DEFAULT_RAW_DOWNLOADED_DIRECTORY, playListMetadata.Title));
        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, playListMetadata.Title));

        var downloads = new List<Task>();

        await foreach (var video in videos)
        {
            counter++;
            downloads.Add(DoDownloadAsync(video, youtube, playListMetadata, counter, totalVideos));
            
        } 

        await Task.WhenAll(downloads); 
        ClearRawDowloadDirectory();
    }

    private async Task DoDownloadAsync(PlaylistVideo video, YoutubeClient youtubeClient, Playlist playlistMetadata, int counter, int totalVideos)
    {   
        var progressHandler = new Progress<double>(progress =>
        {
            _progressDictionary[GetCounterString(counter, totalVideos)] = $"{progress:P2}";
            RenderProgress(totalVideos);
        });

        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var title = SanitizeTitle(video.Title);
        var folderName = SanitizeTitle(playlistMetadata.Title);

        var originalAudioPath = Path.Combine(Environment.CurrentDirectory, DEFAULT_RAW_DOWNLOADED_DIRECTORY, folderName, $"{GetCounterString(counter, totalVideos)} - {title}.{RAW_FORMAT}");
        var convertedAudioPath = Path.Combine(Environment.CurrentDirectory, DEFAULT_OUTPUT_DIRECTORY, folderName, $"{GetCounterString(counter, totalVideos)} - {title}.{DEFAULT_OUTPUT_FORMAT}");

        await youtubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, originalAudioPath, progressHandler);

        // Obtém informações do autor (artista) e canal (álbum)
        string artist = video.Author.ChannelTitle; // O nome do autor do vídeo (artista)
        string album = folderName;

        var audioConverter = new AudioConverter();
        await audioConverter.ConvertToMp3(originalAudioPath, convertedAudioPath);

        // Adiciona metadados ao arquivo de áudio
        var metadataEditor = new AudioMetadataEditor();
        metadataEditor.AddMetadata(convertedAudioPath, video.Title, artist, album);
    }

    private string GetCounterString(int counter, int total)
    {
        var totalPadLeftIncrement = total.ToString().Length;

        return counter.ToString().PadLeft(totalPadLeftIncrement, '0');
    }


    private void RenderProgress(int totalVideos)
    {
        Console.Clear();
        foreach (var (key, value) in _progressDictionary)
        {
            Console.WriteLine($"Baixando '{key}' - {value}");

        }
    }

    
}