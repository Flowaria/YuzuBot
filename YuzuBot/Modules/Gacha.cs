using System.Text;

namespace YuzuBot.Modules;

internal enum PaperType : byte
{
    ThreeStar,
    ThreeStar_Pickup,
    TwoStar,
    OneStar
}

internal enum GachaType : byte
{
    Regular,
    Pickup,
    Festival
}

internal struct GachaResult
{
    public readonly GachaType Type;
    public readonly PaperType[] Results;
    public int PickupCount;
    public int ThreeStarCount;
    public int TwoStarCount;
    public int OneStarCount;
    public int EligmaCount;

    public GachaResult(GachaType type, int pullCount)
    {
        Type = type;
        Results = new PaperType[pullCount];
        PickupCount = 0;
        ThreeStarCount = 0;
        TwoStarCount = 0;
        OneStarCount = 0;
        EligmaCount = 0;
    }
}

internal static partial class Gacha
{
    public const double PickupProb = 0.007;
    public const double ThreeStarProb = 0.03;
    public const double TwoStarProb = 0.185;
    public const double FestivalMult = 2.0;

    public const double PickupThreshold = PickupProb;
    public const double ThreeStarThreshold = ThreeStarProb;
    public const double TwoStarThreshold = ThreeStarProb + TwoStarProb;

    public const double ThreeStarThreshold_Festival = ThreeStarProb * FestivalMult;
    public const double TwoStarThreshold_Festival = ThreeStarProb * FestivalMult + TwoStarProb;

    public static void CreateRegularResult(out GachaResult gachaResult)
    {
        gachaResult = new GachaResult(GachaType.Regular, 10);

        var rand = Random.Shared;
        for (int i = 0; i < 10; i++)
        {
            var param = rand.NextDouble();
            if (param < ThreeStarThreshold)
            {
                gachaResult.Results[i] = PaperType.ThreeStar;
                gachaResult.ThreeStarCount++;
                gachaResult.EligmaCount += 50;
            }
            else if (param < TwoStarThreshold || i == 9)
            {
                gachaResult.Results[i] = PaperType.TwoStar;
                gachaResult.TwoStarCount++;
                gachaResult.EligmaCount += 10;
            }
            else
            {
                gachaResult.Results[i] = PaperType.OneStar;
                gachaResult.OneStarCount++;
                gachaResult.EligmaCount += 1;
            }
        }
    }

    public static void CreatePickupResult(out GachaResult gachaResult)
    {
        gachaResult = new GachaResult(GachaType.Pickup, 10);

        var rand = Random.Shared;
        for (int i = 0; i < 10; i++)
        {
            var param = rand.NextDouble();
            if (param < PickupThreshold)
            {
                gachaResult.Results[i] = PaperType.ThreeStar_Pickup;
                gachaResult.ThreeStarCount++;
                gachaResult.PickupCount++;
                gachaResult.EligmaCount += 50;
            }
            else if (param < ThreeStarThreshold)
            {
                gachaResult.Results[i] = PaperType.ThreeStar;
                gachaResult.ThreeStarCount++;
                gachaResult.EligmaCount += 50;
            }
            else if (param < TwoStarThreshold || i == 9)
            {
                gachaResult.Results[i] = PaperType.TwoStar;
                gachaResult.TwoStarCount++;
                gachaResult.EligmaCount += 10;
            }
            else
            {
                gachaResult.Results[i] = PaperType.OneStar;
                gachaResult.OneStarCount++;
                gachaResult.EligmaCount += 1;
            }
        }
    }

    public static void CreateFestivalResult(out GachaResult gachaResult)
    {
        gachaResult = new GachaResult(GachaType.Festival, 10);

        var rand = Random.Shared;
        for (int i = 0; i < 10; i++)
        {
            var param = rand.NextDouble();
            if (param < PickupThreshold)
            {
                gachaResult.Results[i] = PaperType.ThreeStar_Pickup;
                gachaResult.ThreeStarCount++;
                gachaResult.PickupCount++;
                gachaResult.EligmaCount += 50;
            }
            else if (param < ThreeStarThreshold_Festival)
            {
                gachaResult.Results[i] = PaperType.ThreeStar;
                gachaResult.ThreeStarCount++;
                gachaResult.EligmaCount += 50;
            }
            else if (param < TwoStarThreshold_Festival || i == 9)
            {
                gachaResult.Results[i] = PaperType.TwoStar;
                gachaResult.TwoStarCount++;
                gachaResult.EligmaCount += 10;
            }
            else
            {
                gachaResult.Results[i] = PaperType.OneStar;
                gachaResult.OneStarCount++;
                gachaResult.EligmaCount += 1;
            }
        }
    }

    public static string BuildEmoteTextFromResult(in GachaResult gachaResult)
    {
        var strBuilder = new StringBuilder();
        int length = gachaResult.Results.Length;
        for (int i = 0; i < length; i++)
        {
            if (i != 0 && i % 5 == 0)
            {
                strBuilder.AppendLine();
            }
            strBuilder.Append(gachaResult.Results[i] switch
            {
                PaperType.ThreeStar => Resources.Emote_Gacha3Star,
                PaperType.ThreeStar_Pickup => Resources.Emote_Gacha3StarPickup,
                PaperType.TwoStar => Resources.Emote_Gacha2Star,
                PaperType.OneStar => Resources.Emote_Gacha1Star,
                _ => throw new NotImplementedException($"{gachaResult.Results[i]} is not supported!"),
            });
        }

        return strBuilder.ToString();
    }
}
