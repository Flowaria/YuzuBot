using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace YuzuBot;
internal partial class YuzuBot
{
    private readonly static DiscordSocketConfig s_CFG = new()
    {
        MessageCacheSize = 100,
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
    };

    private readonly DiscordSocketClient _Client;
    private readonly string _Token;

    private readonly CancellationTokenSource _UpdateToken = new();
    
    public ulong BotID { get; private set; }
    public string BotMentionString { get; private set; } = string.Empty;

    public YuzuBot(string token)
    {
        _Client = new(s_CFG);
        _Token = token;
        _Client.Log += OnLog;
        _Client.Ready += OnReady;
        _Client.MessageReceived += MessageReceived;
    }

    ~YuzuBot()
    {
        _UpdateToken.Cancel();
        _UpdateToken.Dispose();
        _Client.Dispose();
    }

    private async Task OnReady()
    {
        BotID = _Client.CurrentUser.Id;
        BotMentionString = _Client.CurrentUser.Mention;
        await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
        _ = Task.Run(StartUpdateActivity, _UpdateToken.Token);
        _ = Task.Run(StartUpdateGC, _UpdateToken.Token);
        await SetupCommands();
    }

    private async Task StartUpdateGC()
    {
        while (true)
        {
            GC.Collect();
            await Task.Delay(1000 * 10);
        }
    }

    public async Task LoginAndStart()
    {
        await _Client.LoginAsync(TokenType.Bot, _Token);
        await _Client.StartAsync();
    }

    public async Task Logout()
    {
        await _Client.LogoutAsync();
        await _Client.StopAsync();
        _UpdateToken.Cancel();
    }
}
