using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace YuzuBot.Interactions;
internal class DeleteConfirmModule : InteractionModuleBase
{
    private const string ID_DeleteConfirm = "delete-message-confirm";
    private const string ID_DeleteDecline = "delete-message-decline";

    public static async Task Send(IMessage targetMessage, IDiscordInteraction context)
    {
        var comps = new ComponentBuilder()
            .WithButton("확인", $"{ID_DeleteConfirm};{targetMessage.Id}", ButtonStyle.Danger)
            .WithButton("취소하기", ID_DeleteDecline, ButtonStyle.Secondary)
            .Build();

        await context.RespondAsync("정말로 삭제할까요?", components: comps, ephemeral: true);
    }

    [ComponentInteraction(ID_DeleteConfirm + ";*")]
    public async Task Confirm(string messageID)
    {
        if (Context.Interaction is not SocketMessageComponent arg)
        {
            await DeferAsync();
            return;
        }

        if (!ulong.TryParse(messageID, out var id))
        {
            await DeferAsync();
            return;
        }

        await arg.Message.Channel.DeleteMessageAsync(id);
        await DeferAsync();
        await Context.Interaction.DeleteOriginalResponseAsync();
    }

    [ComponentInteraction(ID_DeleteDecline)]
    public async Task Decline()
    {
        await DeferAsync();
        await Context.Interaction.DeleteOriginalResponseAsync();
    }
}
