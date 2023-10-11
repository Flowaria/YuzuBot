using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot.Modules;
internal static partial class Gacha
{
    private static readonly Color s_ColorBlue = new(37, 190, 255);
    private static readonly Color s_ColorYellow = new(255, 244, 37);
    private static readonly Color s_ColorPink = new(255, 90, 195);

    public static void GetYuzuReactions(in GachaResult gachaResult, out string message, out YuzuExpression expression, out Color color)
    {
        switch (gachaResult.Type)
        {
            case GachaType.Regular:
                GetYuzuReactions_Regular(in gachaResult, out message, out expression, out color);
                return;

            case GachaType.Pickup:
                GetYuzuReactions_Pickup(in gachaResult, out message, out expression, out color);
                return;

            case GachaType.Festival:
                GetYuzuReactions_Festival(in gachaResult, out message, out expression, out color);
                return;

            default:
                throw new NotImplementedException($"{gachaResult.Type} is not supported!");
        }
    }

    private static void GetYuzuReactions_Regular(in GachaResult data, out string message, out YuzuExpression expression, out Color color)
    {
        var c3Star = data.ThreeStarCount;
        var c2Star = data.TwoStarCount;

        if (c3Star >= 4)
        {
            message = $"3성이 {c3Star}개나 나오다니 굉장해요!!!!";
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star >= 2)
        {
            message = $"3성이 {c3Star}개나 나오다니! 선생님은 정말로 운이 좋으시네요!";
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star == 1)
        {
            message = $"3성이 나왔어요! 오늘은 운이 좋은 날이 될거같네요";
            expression = YuzuExpression.SmallSmile;
            color = s_ColorPink;
        }
        else if (c2Star == 10)
        {
            message = $"엣... 그 이게.... 에엣...???";
            expression = YuzuExpression.Fear;
            color = s_ColorYellow;
            return;
        }
        else if (c2Star >= 6)
        {
            message = $"비록 3성이 나오진 않았지만 2성이 {c2Star}개나 나와서 최악은 면한거 같네요";
            expression = YuzuExpression.SmallSmile;
            color = s_ColorYellow;
        }
        else if (c2Star >= 2)
        {
            message = $"고작 2성 {c2Star}개라니...";
            expression = YuzuExpression.Mataku;
            color = s_ColorYellow;
        }
        else
        {
            message = $"올블루라니... 그런...";
            expression = YuzuExpression.Cry;
            color = s_ColorBlue;
        }
    }

    private static void GetYuzuReactions_Pickup(in GachaResult data, out string message, out YuzuExpression expression, out Color color)
    {
        var c3Star = data.ThreeStarCount;
        var c2Star = data.TwoStarCount;
        var cPickup = data.PickupCount;
        var msg = new StringBuilder();

        if (c3Star >= 4)
        {
            msg.Append($"3성이 {c3Star}개가 한번에..! 대단해요...!!");
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star >= 2)
        {
            msg.Append($"3성이 {c3Star}개나..! 대단해요...!!");
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star == 1)
        {
            msg.Append($"3성이 나왔네요..! 오늘은 운이 좋은 날인 것 같아요..!");
            expression = YuzuExpression.SmallSmile;
            color = s_ColorPink;
        }
        else if (c2Star == 10)
        {
            message = $"엣... 그 이게.... 에엣...???";
            expression = YuzuExpression.Fear;
            color = s_ColorYellow;
            return;
        }
        else if (c2Star >= 6)
        {
            message = $"비록 3성이 나오진 않았지만 2성이 {c2Star}개나 나와서 최악은 면한거 같네요";
            expression = YuzuExpression.Smile;
            color = s_ColorYellow;
            return;
        }
        else if (c2Star >= 2)
        {
            message = $"고작 2성 {c2Star}개라니...";
            expression = YuzuExpression.Mataku;
            color = s_ColorYellow;
            return;
        }
        else
        {
            message = $"올블루라니... 그런...";
            expression = YuzuExpression.Cry;
            color = s_ColorBlue;
            return;
        }

        //픽업 메시지 추가
        if (cPickup >= 3)
        {
            msg.Append($" 게다가 픽업이 {cPickup}개나 나왔어요..! 선생님은 운이 정말 좋으신거 같아요..!");
        }
        else if (cPickup == 2)
        {
            msg.Append($" 게다가 픽업이 두개나 나왔어요..!");
        }
        else if (cPickup == 1)
        {
            msg.Append($" 그리고 픽업도 나왔어요..!");
        }
        else if (cPickup == 0)
        {
            msg.Append($" 하지만 픽업이 나오지 않은건 아쉽네요...");
        }
        message = msg.ToString();
        return;
    }

    private static void GetYuzuReactions_Festival(in GachaResult data, out string message, out YuzuExpression expression, out Color color)
    {
        var c3Star = data.ThreeStarCount;
        var c2Star = data.TwoStarCount;
        var cPickup = data.PickupCount;
        var msg = new StringBuilder();

        if (c3Star >= 5)
        {
            msg.Append($"아무리 확률이 두배라지만 3성이 {c3Star}개가 한번에..! 대단해요...!!");
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star >= 3)
        {
            msg.Append($"3성이 {c3Star}개나..! 대단해요...!!");
            expression = YuzuExpression.Smile;
            color = s_ColorPink;
        }
        else if (c3Star == 2)
        {
            msg.Append($"역시 3성 확률이 두배라 3성이 두개나 나오네요..!");
            expression = YuzuExpression.SmallSmile;
            color = s_ColorPink;
        }
        else if (c3Star == 1)
        {
            msg.Append($"3성이 나왔네요..! 오늘은 운이 좋은 날인 것 같아요..!");
            expression = YuzuExpression.SmallSmile;
            color = s_ColorPink;
        }
        else if (c2Star == 10)
        {
            message = $"엣... 그 이게.... 에엣...???";
            expression = YuzuExpression.Fear;
            color = s_ColorYellow;
            return;
        }
        else if (c2Star >= 6)
        {
            message = $"비록 3성이 나오진 않았지만 2성이 {c2Star}개나 나와서 최악은 면한거 같네요";
            expression = YuzuExpression.Smile;
            color = s_ColorYellow;
            return;
        }
        else if (c2Star >= 2)
        {
            message = $"고작 2성 {c2Star}개라니...";
            expression = YuzuExpression.Mataku;
            color = s_ColorYellow;
            return;
        }
        else
        {
            message = $"올블루라니... 그런...";
            expression = YuzuExpression.Cry;
            color = s_ColorYellow;
            return;
        }

        //픽업 메시지 추가
        if (cPickup >= 3)
        {
            msg.Append($" 게다가 픽업이 {cPickup}개나 나왔어요..! 선생님은 운이 정말 좋으신거 같아요..!");
        }
        else if (cPickup == 2)
        {
            msg.Append($" 게다가 픽업이 두개나 나왔어요..!");
        }
        else if (cPickup == 1)
        {
            msg.Append($" 그리고 픽업도 나왔어요..!");
        }
        else if (cPickup == 0)
        {
            msg.Append($" 하지만 픽업이 나오지 않은건 아쉽네요...");
        }
        message = msg.ToString();
        return;
    }
}
