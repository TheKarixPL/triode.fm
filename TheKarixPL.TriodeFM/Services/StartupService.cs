using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TheKarixPL.TriodeFM.Extensions;
using TheKarixPL.TriodeFM.Handlers;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Services;

public class BotService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commandService;
    private readonly BotOptions _botOptions;
    private readonly ILogger _logger;
    private readonly CommandHandler _commandHandler;
    private readonly LogHandler _logHandler;

    public BotService(IServiceProvider serviceProvider, 
        DiscordSocketClient discord, 
        CommandService commandService, 
        IOptions<BotOptions> botOptions, 
        ILogger<BotService> logger,
        CommandHandler commandHandler,
        LogHandler logHandler)
    {
        _serviceProvider = serviceProvider;
        _discord = discord;
        _commandService = commandService;
        _botOptions = botOptions.Value;
        _logger = logger;
        _commandHandler = commandHandler;
        _logHandler = logHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogTrace("Executing startup");
        _discord.Log += _logHandler.Log;
        _commandService.Log += _logHandler.Log;
        _discord.Ready += () =>
        {
            _logger.LogInformation("Connected to Discord as {Username}", _discord.CurrentUser.FormatUsername());
            return Task.CompletedTask;
        };
        _discord.Ready += _commandHandler.Ready;
        _discord.MessageReceived += _commandHandler.MessageReceived;
        await _discord.LoginAsync(TokenType.Bot, _botOptions.Token);
        await _discord.StartAsync();
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        await Task.Delay(-1);
    }
}