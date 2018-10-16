using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeliosBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeliosBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace HeliosBot
{
    public class HeliosBot
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private CommandService _commands;
        private IConfiguration _configuration;

        public async Task Start()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            _commands = new CommandService();

            await InitDI();

            _client.Log += Logger.Log;
            _client.MessageReceived += HandleCommand;

            await _client.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InitDI()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("HELIOS_");

            _configuration = configBuilder.Build();

            _services = new ServiceCollection()

                .AddSingleton<IDiscordClient>(_client)
                .AddSingleton(_commands)

                .AddSingleton<IASXService, ASXService>()
                .AddSingleton<IChartService, ChartService>()
                .AddSingleton<IPhantomJSService, PhantomJSService>()
                .AddSingleton<IPapertradeService, PapertradeService>()

                .AddSingleton<IConfiguration>(_configuration)

                .AddEntityFrameworkMySQL()
                .AddDbContext<DatabaseContext>(options => options.UseMySQL(_configuration["ConnectionString"]))

                .BuildServiceProvider();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            
            int argPos = 0;
            if (!(message.HasCharPrefix('.', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(_client, message);
            
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
