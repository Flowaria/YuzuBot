using Discord;
using Discord.WebSocket;

namespace YuzuBot;
internal partial class YuzuBot
{
    private readonly static DiscordSocketConfig s_CFG = new()
    {
        MessageCacheSize = 100,
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.GuildMembers | GatewayIntents.GuildEmojis
    };

    private readonly DiscordSocketClient _Client;
    private readonly string _Token;

    private readonly CancellationTokenSource _UpdateToken = new();

    private DateTime _ReadyTime;

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
        _IntService?.Dispose();
    }

    private async Task OnReady()
    {
        BotID = _Client.CurrentUser.Id;
        BotMentionString = _Client.CurrentUser.Mention;
        await _Client.SetStatusAsync(UserStatus.DoNotDisturb);
        _ = Task.Run(StartUpdateActivity, _UpdateToken.Token);
        _ = Task.Run(StartUpdateGC, _UpdateToken.Token);
        await SetupInteractions();
        _ReadyTime = DateTime.Now;
    }

    private async Task StartUpdateGC()
    {
        while (true)
        {
            GC.Collect();
            await Task.Delay(TimeSpan.FromSeconds(10));
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
        _IntService?.Dispose();
        _IntService = null;
    }
}
