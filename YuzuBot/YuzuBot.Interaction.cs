using Discord.Interactions;
using YuzuBot.Interactions;
using RunMode = Discord.Interactions.RunMode;

namespace YuzuBot;
internal partial class YuzuBot
{
    private InteractionService? _IntService;

    private async Task SetupInteractions()
    {
        _IntService = new InteractionService(_Client.Rest, new InteractionServiceConfig()
        {
            DefaultRunMode = RunMode.Async
        });

        LogDebug("Creating Interaction Modules");

        var gachaModule = await _IntService.AddModuleAsync<GachaModule>(null);
        var gachaMenuModule = await _IntService.AddModuleAsync<GachaMenuModule>(null);
        var deleteConfirmModule = await _IntService.AddModuleAsync<DeleteConfirmModule>(null);
        //var aEmoteModule = await _IntService.AddModuleAsync<AnimatedEmoteModule>(null);

        LogDebug("Modules Created");

        await _IntService.AddModulesGloballyAsync(deleteMissing: true, gachaModule, gachaMenuModule, deleteConfirmModule);

        LogDebug("Modules Registered!");

        _IntService.Log += OnLog;
        _Client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(_Client, x);
            await _IntService.ExecuteCommandAsync(ctx, null);
        };

        //_Client.SlashCommandExecuted += SlashCommandExecuted;
        //_Client.ButtonExecuted += ButtonPressed;
        //_Client.SelectMenuExecuted += SelectedMenuItem;
    }
}
