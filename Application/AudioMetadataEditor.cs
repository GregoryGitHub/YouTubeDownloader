namespace YouTubeDownloader.Application;
// Classe para adicionar metadados ao arquivo
public class AudioMetadataEditor
{
    public void AddMetadata(string filePath, string title, string artist, string album)
    {
        // Carrega o arquivo de áudio
        var audioFile = TagLib.File.Create(filePath);

        // Adiciona/edita os metadados
        audioFile.Tag.Title = title;
        audioFile.Tag.Performers = new[] { artist };
        audioFile.Tag.Album = album;

        // Salva as alterações
        audioFile.Save();
    }
}