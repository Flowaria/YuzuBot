using Discord;
using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuBot.Interactions;
using YuzuBot.Modules;
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
        LogDebug("Modules Created");
        await _IntService.AddModulesGloballyAsync(deleteMissing: true, gachaModule, gachaMenuModule);
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
