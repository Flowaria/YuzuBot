using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot;
internal partial class YuzuBot
{
    private async Task SetupCommands()
    {
        var gachaCmd = new SlashCommandBuilder()
            .WithName("gacha")
            .WithDescription("10챠 가챠 시뮬을 돌립니다.");
        await _Client.CreateGlobalApplicationCommandAsync(gachaCmd.Build());
        _Client.SlashCommandExecuted += SlashCommandExecuted;
    }

    private async Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        if (!arg.GuildId.HasValue)
            return;

        if (arg.Channel == null)
            return;

        switch (arg.CommandName)
        {
            case "gacha":
                var embed = Embeds.BuildGachaResult(out var gachaResult);
                await arg.RespondAsync(text: gachaResult, embed: embed);
                return;
        }

        await arg.RespondAsync("명령어를 실행하는 중 문제가 발생했어요...", ephemeral: true);
    }
}
