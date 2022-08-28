using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TheKarixPL.TriodeFM.Extensions;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Factories;

public class EmbedFactory
{
    private readonly DiscordSocketClient _discord;
    private readonly BotOptions _botOptions;

    public EmbedFactory(DiscordSocketClient discord, IOptions<BotOptions> botOptions)
    {
        _discord = discord;
        _botOptions = botOptions.Value;
    }

    public Embed CreateEmbed(string text)
        => CreateEmbed(_discord.CurrentUser.FormatUsername(), text);

    public Embed CreateEmbed(string title, string text)
    {
        var builder = new EmbedBuilder()
            .WithTitle(title)
            .WithThumbnailUrl(_discord.CurrentUser.GetAvatarUrl() ?? _discord.CurrentUser.GetDefaultAvatarUrl())
            .WithDescription(text)
            .WithColor(new Color(Convert.ToUInt32(_botOptions.EmbedColor[1..], 16)))
            .WithCurrentTimestamp();
        return builder.Build();
    }
}