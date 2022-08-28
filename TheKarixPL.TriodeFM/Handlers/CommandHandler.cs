using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TheKarixPL.TriodeFM.Extensions;
using TheKarixPL.TriodeFM.Factories;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Handlers;

public class CommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _discord;
    private readonly BotOptions _botOptions;
    private readonly CommandService _commandService;
    private readonly EmbedFactory _embedFactory;

    public CommandHandler(IServiceProvider serviceProvider,
        ILogger<CommandHandler> logger, 
        DiscordSocketClient discord, 
        IOptions<BotOptions> botOptions,
        CommandService commandService,
        EmbedFactory embedFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _discord = discord;
        _botOptions = botOptions.Value;
        _commandService = commandService;
        _embedFactory = embedFactory;
    }

    internal async Task Ready()
    {
        
    }

    internal async Task MessageReceived(SocketMessage arg)
    {
        var msg = (SocketUserMessage)arg;

        if (msg.Author.IsBot)
            return;
        
        int pos = 0;
        if ((msg.HasMentionPrefix(_discord.CurrentUser, ref pos) || msg.HasStringPrefix(_botOptions.Prefix, ref pos)) && msg.Channel is IGuildChannel)
        {
            _logger.LogTrace("User {Username} issued command \"{Command}\"", msg.Author.FormatUsername(), msg.Content);
            var context = new SocketCommandContext(_discord, msg);
            var result = await _commandService.ExecuteAsync(context, pos, _serviceProvider);

            if (!result.IsSuccess)
            {
                _logger.LogTrace("Error executing command \"{Command}\" by user {Username}: {Error}", 
                    msg.Content, 
                    msg.Author.FormatUsername(), 
                    result.ErrorReason);
                await msg.Channel.SendMessageAsync(null, false,
                    _embedFactory.CreateEmbed($"Error executing command \"{msg.Content}\": {result.ErrorReason}"));
            }
        }
    }
}