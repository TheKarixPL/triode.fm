using System.Data.Common;
using System.Text.RegularExpressions;
using Dapper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;
using NLog.Web;
using Npgsql;
using TheKarixPL.TriodeFM.Factories;
using TheKarixPL.TriodeFM.Handlers;
using TheKarixPL.TriodeFM.Models;
using TheKarixPL.TriodeFM.Services;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    logger.Info("triode.fm by TheKarixPL");
    logger.Trace("Configuring Builder");
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Logging
        .ClearProviders()
        .SetMinimumLevel(LogLevel.Debug);
    builder.Host.UseNLog();

    if (builder.Environment.IsDevelopment())
        builder.Configuration.AddJsonFile("secret.Development.json");
    else
        builder.Configuration.AddJsonFile("secret.json");
    builder.Services.AddOptions<BotOptions>()
        .Configure<IConfiguration>((options, configuration)
            => configuration.Bind(BotOptions.SectionName, options))
        .Validate(options => !string.IsNullOrWhiteSpace(options.Token),
            $"{BotOptions.SectionName}:{nameof(BotOptions.Token)} cannot be empty")
        .Validate(options => !string.IsNullOrWhiteSpace(options.EmbedColor),
            $"{BotOptions.SectionName}:{nameof(BotOptions.EmbedColor)} cannot be empty")
        .Validate(
            options => options.EmbedColor.Length == 7 &&
                       Regex.IsMatch(options.EmbedColor, @"^#(?:[0-9a-fA-F]{3}){1,2}$"),
            $"{BotOptions.SectionName}:{nameof(BotOptions.EmbedColor)} is in invalid format");
    builder.Services.AddOptions<DatabaseOptions>()
        .Configure<IConfiguration>((options, configuration)
            => configuration.Bind(DatabaseOptions.SectionName, options))
        .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
            $"{DatabaseOptions.SectionName}:{nameof(DatabaseOptions.ConnectionString)} cannot be empty");

    try
    {
        logger.Info("Testing database");
        using var conn = new NpgsqlConnection(builder.Configuration["Database:ConnectionString"]);
        conn.Open();
        logger.Info($"Connected to {conn.QuerySingle(@"SELECT VERSION() AS version;").version}");
    }
    catch (DbException e)
    {
        logger.Error(e, "Database test failed");
        LogManager.Shutdown();
        return;
    }

    if (!Directory.Exists("./music"))
        Directory.CreateDirectory("./music");
    else
        Directory.GetFiles("./music"); //Check if bot has access to directory

    builder.Services.AddSingleton<DatabaseConnectionFactory>();

    builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Verbose,
            MessageCacheSize = 1024
        }))
        .AddSingleton(new CommandService(new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Verbose,
            DefaultRunMode = RunMode.Async,
            CaseSensitiveCommands = false
        }))
        .AddSingleton<LogHandler>()
        .AddSingleton<CommandHandler>()
        .AddSingleton<EmbedFactory>()
        .AddHostedService<BotService>();

    var app = builder.Build();

    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception e)
{
    logger.Error(e);
}
finally
{
    LogManager.Shutdown();
}