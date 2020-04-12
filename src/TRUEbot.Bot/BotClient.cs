using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TRUEbot.Bot
{
    public interface IBotClient
    {
        Task Start();
        Task Stop();
    }

    public class BotClient : IBotClient
    {
        private readonly ILogger<BotClient> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;

        private readonly BotConfiguration _botConfiguration;

        public BotClient(ILogger<BotClient> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<BotConfiguration> botConfiguration,
            DiscordSocketClient discordSocketClient, CommandService commandService)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            _discordSocketClient = discordSocketClient;
            _commandService = commandService;

            _botConfiguration = botConfiguration.Value;
        }

        public async Task Start()
        {
            _discordSocketClient.Log += OnLog;
            _discordSocketClient.Ready += OnReady;
            _discordSocketClient.Connected += OnConnected;
            _discordSocketClient.Disconnected += OnDisconnected;
            _discordSocketClient.MessageReceived += OnMessageReceived;

            _commandService.CommandExecuted += OnCommandExecuted;

            using var scope = _serviceScopeFactory.CreateScope();
            await _commandService.AddModulesAsync(typeof(BotClient).Assembly, scope.ServiceProvider);
            await _discordSocketClient.LoginAsync(TokenType.Bot, _botConfiguration.DiscordToken);

            await _discordSocketClient.StartAsync();
        }

        private async Task StartupMessage()
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var client = scope.ServiceProvider.GetService<DiscordSocketClient>();

            var guild = client.Guilds.FirstOrDefault();

            var channel = guild?.GetTextChannel(541376685183074314);

            if (channel == null)
                channel = guild?.GetTextChannel(541693085390733317);

            if (channel == null)
                return;

            await channel.SendMessageAsync("TrueBot Online!");
        }

        private Task OnMessageReceived(SocketMessage message)
        {
            // Anything that isn't from an actual user, ignore
            if (!(message is SocketUserMessage receivedMessage)
                || message.Author.IsBot
                || message.Author.IsWebhook)
                return Task.CompletedTask;

            if (message.Channel is IDMChannel)
            {
                Task.Run(() => HandleDmMessage(receivedMessage));
                return Task.CompletedTask;
            }

            const char PREFIX = '!';

            var argPos = 0;

            var isIntendedForCommand = receivedMessage.HasCharPrefix(PREFIX, ref argPos);
            var isIntendedForMention = receivedMessage.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos);

            if (!isIntendedForCommand && !isIntendedForMention)
                return Task.CompletedTask;

            // If this is a command, we don't care where this is happening (yet?)
            // just create the context and fire off to D.NET
            if (isIntendedForCommand)
            {
                Task.Run(async () => await ForwardCommand(argPos, receivedMessage));
            }

            return Task.CompletedTask;
        }

        private async Task ForwardCommand(int argPos, SocketUserMessage message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = new SocketCommandContext(_discordSocketClient, message);
            await _commandService.ExecuteAsync(context, argPos, scope.ServiceProvider);
        }

        private async Task HandleDmMessage(SocketUserMessage message)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var client = scope.ServiceProvider.GetService<DiscordSocketClient>();

            var user = client.GetUser(434439379180716064);

            if (user != null)
            {
                await user.SendMessageAsync($"{message.Content} sent by {message.Author}");
            }

            var guild = client.Guilds.FirstOrDefault();

            var channel = guild?.GetTextChannel(604789293877035009);

            if (channel == null)
                return;

            await channel.SendMessageAsync(message.Content);
        }

        private Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed running command {CommandName} for reason {Reason} in guild {Guild} ({GuildId}) for user {User} ({UserId})",
                    commandInfo.IsSpecified ? commandInfo.Value.Name : "Unknown",
                    result.ErrorReason,
                    context.Guild?.Name ?? "DM",
                    context.Guild?.Id ?? context.User.Id,
                    context.User.Username,
                    context.User.Id);
            }

            return Task.CompletedTask;
        }

        private Task OnConnected()
        {
            _logger.LogInformation("Discord client connected");
            return Task.CompletedTask;
        }

        private Task OnDisconnected(Exception arg)
        {
            _logger.LogInformation("Discord client disconnected");
            return Task.CompletedTask;
        }

        private async Task OnReady()
        {
            _logger.LogInformation("Discord client ready");
            await StartupMessage();
        }

        private Task OnLog(LogMessage log)
        {
            var level = log.Severity switch
            {
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                _ => throw new ArgumentOutOfRangeException($"{log.Severity} log out of range")
            };

            _logger.Log(level, log.Exception,
                log.Source != null
                    ? $"{log.Message} at source {log.Source}"
                    : log.Message);

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return _discordSocketClient.LogoutAsync();
        }
    }
}
