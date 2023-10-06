using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuBot;
internal partial class YuzuBot
{
    private readonly (IActivity Activity, int MinMinute, int MaxMinute)[] _YuzuActivities =
    {
        (new CustomStatusGame("🕹️ 404 Not Found"), 5, 15),
        (new CustomStatusGame("🕹️ 403 Forbidden"), 5, 15),
        (new CustomStatusGame("찾지 말아주세요..."), 15, 15),
        (new CustomStatusGame("UZQueen 아닙니다."), 5, 10),
        (new CustomStatusGame("아르바이트 중"), 5, 15),
        (new CustomStatusGame("디버깅 체크 중"), 25, 35),
        (new Game("냥즈 대쉬"), 30, 60),
        (new Game("테일즈 사가 크로니클: II"), 20, 20),
        (new Game("YounitEngine (2023.08.12f)"), 20, 20),
        (new Game("테일즈 사가 크로니클: III (IN-DEV)"), 20, 20)
    };

    private async Task StartUpdateActivity()
    {
        var rng = Random.Shared;
        while(true)
        {
            var (activity, minMinute, maxMinute) = _YuzuActivities[rng.Next(_YuzuActivities.Length)];
            await _Client.SetActivityAsync(activity);
            var minDelay = 1000 * 60 * minMinute;
            var maxDelay = 1000 * 60 * maxMinute;
            await Task.Delay(rng.Next(minDelay, maxDelay + 1));
        }
    }
}
