using UnityEngine;

public class LevelPlayConfig
{

    public static string AppKey => GetAppKey();
    public static string addlifeRewardedVideoKey => GetAddLifeRewardedVideo();
    public static string InterstitialAdKey => getInterstitialAd();

    static string GetAppKey()
    {
#if UNITY_ANDROID
        return "24d17d595";
        // return "85460dcd"; // Test only
#else
            return "unexpected_platform";
#endif
    }

    static string GetAddLifeRewardedVideo()
    {
#if UNITY_ANDROID
        return "ux5u0c5odzxzptkn";
        // return "76yy3nay3ceui2a3"; // Test Only
#else
            return "unexpected_platform";
#endif
    }


    static string getInterstitialAd()
    {
#if UNITY_ANDROID
        return "zrr51mm97kely1i7";
        // return "76yy3nay3ceui2a3"; // Test Only
#else
            return "unexpected_platform";
#endif
    }

}
