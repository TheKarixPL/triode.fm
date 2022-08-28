using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TheKarixPL.TriodeFM.Factories;
using TheKarixPL.TriodeFM.Interfaces;
using TheKarixPL.TriodeFM.Models;
using TheKarixPL.TriodeFM.Players;

namespace TheKarixPL.TriodeFM.Modules;

public class UserModule : ModuleBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _discord;
    private readonly BotOptions _botOptions;
    private readonly EmbedFactory _embedFactory;

    private static readonly Dictionary<ulong, IPlayer> Players = new Dictionary<ulong, IPlayer>();

    public UserModule(IServiceProvider serviceProvider, 
        ILogger<UserModule> logger, 
        DiscordSocketClient discord,
        IOptions<BotOptions> botOptions,
        EmbedFactory embedFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _discord = discord;
        _botOptions = botOptions.Value;
        _embedFactory = embedFactory;
    }

    [Command("play")]
    public async Task Play(string source, string url)
    {
        if (source.ToLower() != "ffmpeg")
        {
            await Context.Channel.SendMessageAsync(null, false, _embedFactory.CreateEmbed("Wrong source"));
            return;
        }
        else if (string.IsNullOrWhiteSpace(url))
        {
            await Context.Channel.SendMessageAsync(null, false, _embedFactory.CreateEmbed("Url is empty"));
            return;
        }

        var guildUser = await Context.Guild.GetUserAsync(Context.User.Id);

        if(guildUser.VoiceChannel == null)
        {
            await Context.Channel.SendMessageAsync(null, false, _embedFactory.CreateEmbed("You are not connected to voice channel"));
            return;
        }

        if (Players.ContainsKey(guildUser.Guild.Id))
        {
            await Players[guildUser.Guild.Id].Stop();
            Players.Remove(guildUser.Guild.Id);
        }

        var client = await guildUser.VoiceChannel.ConnectAsync();
        switch (source.ToLower())
        {
            case "ffmpeg":
                IPlayer player = new FFMPEGPlayer(client, url);
                Players.Add(guildUser.Guild.Id, player);
                await player.Play();
                break;
        }
    }

    [Command("stop")]
    public async Task Stop()
    {
        if (Players.ContainsKey(Context.Guild.Id))
        {
            await Players[Context.Guild.Id].Stop();
            Players.Remove(Context.Guild.Id);
        }
    }
    
    [Command("resume")]
    public async Task Resume()
    {
        if (Players.ContainsKey(Context.Guild.Id) && Players[Context.Guild.Id].State == PlayerState.Stopped && Players[Context.Guild.Id].Elapsed > TimeSpan.Zero)
        {
            await Players[Context.Guild.Id].Play();
        }
    }
    
    [Command("pause")]
    public async Task Pause()
    {
        if (Players.ContainsKey(Context.Guild.Id) && Players[Context.Guild.Id].State == PlayerState.Playing)
        {
            await Players[Context.Guild.Id].Pause();
        }
    }
}