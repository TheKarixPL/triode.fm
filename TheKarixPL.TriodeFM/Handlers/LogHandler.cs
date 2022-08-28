using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TheKarixPL.TriodeFM.Handlers;

public class LogHandler
{
    private ILogger _logger;

    public LogHandler(ILogger<LogHandler> logger, DiscordSocketClient discord, CommandService commandService)
    {
        _logger = logger;
    }

    internal Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(message.ToString());
                break;
            case LogSeverity.Error:
                _logger.LogError(message.ToString());
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(message.ToString());
                break;
            case LogSeverity.Info:
                _logger.LogInformation(message.ToString());
                break;
            case LogSeverity.Verbose:
                _logger.LogTrace(message.ToString());
                break;
            case LogSeverity.Debug:
                _logger.LogDebug(message.ToString());
                break;
        }

        return Task.CompletedTask;
    }
}