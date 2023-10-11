namespace YuzuBot;

internal static class EntryPoint
{
    private static string s_BotToken = string.Empty;
    private static YuzuBot? s_Bot;

    public static void Main()
    {
        var tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "botToken");
        if (!File.Exists(tokenPath))
        {
            Console.WriteLine("Error::file 'botToken' is missing!");
            return;
        }
        s_BotToken = File.ReadAllText(tokenPath).Trim();

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        MainAsync().GetAwaiter().GetResult();
    }


    public static async Task MainAsync()
    {
        s_Bot = new(s_BotToken);
        await s_Bot.LoginAndStart();
        await Task.Delay(-1);
    }

    private static void OnProcessExit(object? sender, EventArgs e)
    {
        s_Bot?.Logout().GetAwaiter().GetResult();
    }
}
