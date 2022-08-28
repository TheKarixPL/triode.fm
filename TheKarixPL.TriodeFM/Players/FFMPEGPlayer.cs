using System.Diagnostics;
using Discord.Audio;
using TheKarixPL.TriodeFM.Extensions;
using TheKarixPL.TriodeFM.Interfaces;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Players;

public class FFMPEGPlayer : IPlayer
{
    public IAudioClient Client { get; }
    public virtual PlayerState State { get; private set; } = PlayerState.Stopped;

    public virtual TimeSpan? Elapsed => _stopwatch.Elapsed;
    public virtual TimeSpan? Total { get; }
    public virtual string TrackName { get; }

    private string _url;
    private CancellationTokenSource _cancellationTokenSource = null;
    private Stopwatch _stopwatch = new Stopwatch();

    public FFMPEGPlayer(IAudioClient client, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));
        _url = url;
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public virtual async Task Play()
    {
        if (State == PlayerState.Stopped)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Worker(TimeSpan.Zero, _cancellationTokenSource.Token);
        }
        else if (State == PlayerState.Stopped && _stopwatch.Elapsed > TimeSpan.Zero)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Worker(_stopwatch.Elapsed, _cancellationTokenSource.Token);
        }
    }

    private async Task Worker(TimeSpan time, CancellationToken cancellationToken)
    {
        Process process = null;
        try
        {
            State = PlayerState.Playing;
            process = Process.Start(new ProcessStartInfo()
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{_url.Replace("\"", "")}\" -ss \"{time:hh\\:mm\\:ss}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            using var output = process.StandardOutput.BaseStream;
            using var audioStream = Client.CreatePCMStream(AudioApplication.Music);
            try
            {
                _stopwatch.Start();
                await output.CopyToAsync(audioStream, cancellationToken);
            }
            finally
            {
                if(process != null && process.IsRunning())
                    process.Kill();
                await audioStream.FlushAsync();
            }
        }
        finally
        {
            if(process != null && process.IsRunning())
                process.Kill();
            State = PlayerState.Stopped;
        }
    }

    public virtual async Task Seek(long time)
    {
        throw new NotImplementedException();
    }

    public virtual async Task Pause()
    {
        if(State == PlayerState.Playing)
            _cancellationTokenSource.Cancel();
        _stopwatch.Stop();
    }

    public virtual async Task Stop()
    {
        if(State == PlayerState.Playing)
            _cancellationTokenSource.Cancel();
        _stopwatch.Stop();
        _stopwatch.Reset();
    }
}