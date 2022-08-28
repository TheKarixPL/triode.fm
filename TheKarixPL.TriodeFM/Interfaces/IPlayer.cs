using Discord;
using Discord.Audio;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Interfaces;

public interface IPlayer
{
    IAudioClient Client { get; }
    PlayerState State { get; }
    TimeSpan? Elapsed { get; }
    TimeSpan? Total { get; }
    string TrackName { get; }

    Task Play();
    Task Seek(long time);
    Task Pause();
    Task Stop();
}