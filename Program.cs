using YouTubeDownloader.Application;

Console.WriteLine("Bem vindo ao YouTube Downloader!");

Console.WriteLine("1. Baixar um único áudio");
Console.WriteLine("2. Baixar uma playlist de áudios");
Console.WriteLine("3. Sair");

Console.Write("Digite o número correspondente à opção desejada: ");
var playListDownloader = new PlayListAudioDownloader();
var singleDownloader = new SingleAudioDownloader();

var option = Console.ReadLine();

if(string.IsNullOrEmpty(option))
{
    Console.WriteLine("A opção não pode ser vazia.");
    return;
}

if (option == "1")
{
    Console.WriteLine("Por favor, insira o link do vídeo que deseja baixar o áudio:");

    var urlSingle = Console.ReadLine();

    if (string.IsNullOrEmpty(urlSingle))
    {
        Console.WriteLine("O link do vídeo não pode ser vazio.");
        return;
    }

    await singleDownloader.DownloadAudioAsync(urlSingle);
}
else if (option == "2")
{
    
    Console.WriteLine("Por favor, insira o link da playlist que deseja baixar os áudios:");
    
    var playListUrl = Console.ReadLine();
    
    if(string.IsNullOrEmpty(playListUrl))
    {
        Console.WriteLine("O link da playlist não pode ser vazio.");
        return;
    }

    await playListDownloader.DownloadPlaylistAsync(playListUrl);
}

else if (option == "3")
{
    Console.WriteLine("Saindo do programa...");
}

else
{
    Console.WriteLine("Opção inválida. Por favor, selecione uma opção válida.");
}



Console.WriteLine("Todas as tarefas foram concluídas com sucesso! Pressione qualquer tecla para sair.");

