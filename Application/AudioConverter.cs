using System.Diagnostics;
using System.Text.RegularExpressions;

namespace YouTubeDownloader.Application;

public class AudioConverter
{
    public async Task ConvertToMp3(string inputFilePath, string outputFilePath)
    {
        var ffmpegPath = Path.Combine(Environment.CurrentDirectory, "ffmpeg", "bin", "ffmpeg.exe");

        // Inicia o processo FFmpeg
        var process = new Process();
        process.StartInfo.FileName = ffmpegPath;
        process.StartInfo.Arguments = $"-i \"{inputFilePath}\" \"{outputFilePath}\" -y";  // O -y sobrescreve o arquivo de saída sem perguntar
        process.StartInfo.RedirectStandardError = true;  // FFmpeg escreve o progresso no stderr
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        
        process.ErrorDataReceived += async (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Tenta extrair o tempo atual de processamento do FFmpeg usando Regex
                var match = Regex.Match(e.Data, @"time=(\d{2}):(\d{2}):(\d{2})\.(\d{2})");
                if (match.Success)
                {
                    var hours = int.Parse(match.Groups[1].Value);
                    var minutes = int.Parse(match.Groups[2].Value);
                    var seconds = int.Parse(match.Groups[3].Value);
                    var milliseconds = int.Parse(match.Groups[4].Value);

                    // Converte o tempo atual para segundos
                    var currentTimeInSeconds = (hours * 3600) + (minutes * 60) + seconds + (milliseconds / 100.0);

                    // Suponha que você já saiba a duração do arquivo (em segundos). Caso contrário, precisaria obter isso de outra forma.
                    double durationInSeconds = await GetDurationInSeconds(inputFilePath);

                    // Calcula o percentual de progresso
                    double progressPercentage = (currentTimeInSeconds / durationInSeconds) * 100;

                    // Exibe o progresso no console
                    Console.Clear();
                    Console.WriteLine($"Conversão: {progressPercentage:F2}%.");
                }
            }
        };

        process.Start();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            Console.WriteLine("Falha ao converter o arquivo de áudio.");
        }
        else
        {
            Console.WriteLine($"Conversão concluída: {outputFilePath}");
        }
    }

    // Método para obter a duração total do arquivo de áudio usando FFprobe (parte do FFmpeg)
    public async Task<double> GetDurationInSeconds(string inputFilePath)
    {
        var ffprobePath = Path.Combine(Environment.CurrentDirectory, "ffmpeg", "bin", "ffprobe.exe");

        var process = new Process();
        process.StartInfo.FileName = ffprobePath;
        process.StartInfo.Arguments = $"-i \"{inputFilePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        return double.Parse(result.Trim());
    }
}
