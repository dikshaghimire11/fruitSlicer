using UnityEngine;
using Unity.Services.LevelPlay;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    public bool canShowInterstitialAd;

    public enum AdvertisementType_ { AddLife, x2Reward };
    public AdvertisementType_ currentAdvertisement;


    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);   // destroy NEW copy
            return;               // stop execution
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private LevelPlayRewardedAd addLifeRewardVideo;

    // private LevelPlayRewardedAd x2RewardVideo;
    private LevelPlayInterstitialAd interstitialAd;
    // bool isAdsEnabled = false;

    private GameObject GameCanvas;
    public GameObject errorMessagePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Debug.Log("[LevelPlaySample] LevelPlay.ValidateIntegration");
        LevelPlay.ValidateIntegration();

        Debug.Log($"[LevelPlaySample] Unity version {LevelPlay.UnityVersion}");

        Debug.Log("[LevelPlaySample] Register initialization callbacks");
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // SDK init
        Debug.Log("[LevelPlaySample] LevelPlay SDK initialization");
        LevelPlay.Init(LevelPlayConfig.AppKey);
        StartCoroutine(interstitialAdCounter());


    }

    public IEnumerator interstitialAdCounter()
    {
        Debug.Log("Reset");
        yield return new WaitForSeconds(120f);
        canShowInterstitialAd = true;
        Debug.Log("Can be shown");
        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log($"[LevelPlaySample] Received SdkInitializationCompletedEvent with Config: {config}");
        EnableAds();
        // isAdsEnabled = true;
    }

    void EnableAds()
    {
        // Register to ImpressionDataReadyEvent
        LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

        // Create Rewarded Video object
        addLifeRewardVideo = new LevelPlayRewardedAd(LevelPlayConfig.addlifeRewardedVideoKey);

        // Register to Rewarded Video events
        addLifeRewardVideo.OnAdLoaded += RewardedVideoOnLoadedEvent;
        addLifeRewardVideo.OnAdLoadFailed += RewardedVideoOnAdLoadFailedEvent;
        addLifeRewardVideo.OnAdDisplayed += RewardedVideoOnAdDisplayedEvent;
        addLifeRewardVideo.OnAdDisplayFailed += RewardedVideoOnAdDisplayedFailedEvent;
        addLifeRewardVideo.OnAdRewarded += RewardedVideoOnAdRewardedEvent;
        addLifeRewardVideo.OnAdClicked += RewardedVideoOnAdClickedEvent;
        addLifeRewardVideo.OnAdClosed += RewardedVideoOnAdClosedEvent;
        addLifeRewardVideo.OnAdInfoChanged += RewardedVideoOnAdInfoChangedEvent;
        addLifeRewardVideo.LoadAd();


        // x2RewardVideo = new LevelPlayRewardedAd(LevelPlayConfig.x2RewardKey);

        // // Register to Rewarded Video events
        // x2RewardVideo.OnAdLoaded += x2RewardVideoOnLoadedEvent;
        // x2RewardVideo.OnAdLoadFailed += x2RewardVideoOnAdLoadFailedEvent;
        // x2RewardVideo.OnAdDisplayed += x2RewardVideoOnAdDisplayedEvent;
        // x2RewardVideo.OnAdDisplayFailed += x2RewardVideoOnAdDisplayedFailedEvent;
        // x2RewardVideo.OnAdRewarded += x2RewardVideoOnAdRewardedEvent;
        // x2RewardVideo.OnAdClicked += x2RewardVideoOnAdClickedEvent;
        // x2RewardVideo.OnAdClosed += x2RewardVideoOnAdClosedEvent;
        // x2RewardVideo.OnAdInfoChanged += x2RewardVideoOnAdInfoChangedEvent;
        // x2RewardVideo.LoadAd();

        // Create Banner object
        // bannerAd = new LevelPlayBannerAd(AdConfig.BannerAdUnitId);

        // // Register to Banner events
        // bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        // bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        // bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        // bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        // bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        // bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        // bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        // bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        // Create Interstitial object
        interstitialAd = new LevelPlayInterstitialAd(LevelPlayConfig.InterstitialAdKey);

        // Register to Interstitial events
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
        interstitialAd.LoadAd();
    }

    void RewardedVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    {

        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnLoadedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdLoadFailedEvent With Error: {error}");
    }

    void RewardedVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdDisplayedFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdDisplayedFailedEvent With AdInfo: {adInfo} and Error: {error}");
    }

    void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdRewardedEvent With AdInfo: {adInfo} and Reward: {reward}");

        if (currentAdvertisement == AdvertisementType_.AddLife)
        {
            ScoreManager.instance.addLifeAndResumeGame();
        }

        if (currentAdvertisement == AdvertisementType_.x2Reward)
        {
            ScoreManager.instance.x2Reward();
        }
    
        addLifeRewardVideo.LoadAd();

    }

    void RewardedVideoOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClickedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdClosedEvent With AdInfo: {adInfo}");
    }

    void RewardedVideoOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received RewardedVideoOnAdInfoChangedEvent With AdInfo {adInfo}");
    }




    // void x2RewardVideoOnLoadedEvent(LevelPlayAdInfo adInfo)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnLoadedEvent With AdInfo: {adInfo}");
    // }

    // void x2RewardVideoOnAdLoadFailedEvent(LevelPlayAdError error)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdLoadFailedEvent With Error: {error}");
    // }

    // void x2RewardVideoOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdDisplayedEvent With AdInfo: {adInfo}");
    // }

    // void x2RewardVideoOnAdDisplayedFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdDisplayedFailedEvent With AdInfo: {adInfo} and Error: {error}");
    // }

    // void x2RewardVideoOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdRewardedEvent With AdInfo: {adInfo} and Reward: {reward}");
    //     ScoreManager.instance.x2Reward();
    // }

    // void x2RewardVideoOnAdClickedEvent(LevelPlayAdInfo adInfo)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdClickedEvent With AdInfo: {adInfo}");
    // }

    // void x2RewardVideoOnAdClosedEvent(LevelPlayAdInfo adInfo)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdClosedEvent With AdInfo: {adInfo}");
    // }

    // void x2RewardVideoOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    // {
    //     Debug.Log($"[LevelPlaySample] Received x2RewardVideoOnAdInfoChangedEvent With AdInfo {adInfo}");
    // }



    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialLoadedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialAdLoadFailedEvent With Error: {error}");
    }

    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Auto Time stamp: " + Time.timeScale);
        Debug.Log($"[LevelPlaySample] Received InterstitialAdDisplayedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialAdDisplayedFailedEvent With AdInfo: {adInfo} and Error: {error}");
    }

    void InterstitialOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log("Auto Time stamp: " + Time.timeScale);
        Debug.Log($"[LevelPlaySample] Received InterstitialAdRewardedEvent With AdInfo: {adInfo} and Reward: {reward}");
        interstitialAd.LoadAd();
    }

    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialAdClickedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Auto Time stamp: " + Time.timeScale);
        Debug.Log($"[LevelPlaySample] Received InterstitialAdClosedEvent With AdInfo: {adInfo}");
    }

    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"[LevelPlaySample] Received InterstitialAdInfoChangedEvent With AdInfo {adInfo}");
    }



    void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
    {
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent ToString(): {impressionData}");
        Debug.Log($"[LevelPlaySample] Received ImpressionDataReadyEvent allData: {impressionData.AllData}");
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log($"[LevelPlaySample] Received SdkInitializationFailedEvent with Error: {error}");
    }


    public void onClickAddLife()
    {
        GameCanvas = GameObject.Find("GameCanvas");
        if (!addLifeRewardVideo.IsAdReady())
        {
            Debug.Log("Gamecanvas: " + GameCanvas);
            GameObject error = Instantiate(errorMessagePrefab, GameCanvas.transform);
            error.GetComponentInChildren<TextMeshProUGUI>().text = "Sorry! Cant Load Advertisement Video";
            return;
        }
        addLifeRewardVideo.ShowAd();
    }

    // public void onClickx2Video()
    // {
    //     GameCanvas = GameObject.Find("GameCanvas");
    //     if (!x2RewardVideo.IsAdReady())
    //     {
    //         GameObject error = Instantiate(errorMessagePrefab, GameCanvas.transform);
    //         error.GetComponentInChildren<TextMeshProUGUI>().text = "Sorry! Cant Load Advertisement Video";
    //         return;
    //     }
    //     x2RewardVideo.ShowAd();
    //     x2RewardVideo.LoadAd();
    // }

    public void playInterestialAd()
    {
        if (canShowInterstitialAd)
        {
            if (interstitialAd.IsAdReady())
            {
                interstitialAd.ShowAd();
            }
            canShowInterstitialAd = false;
            StartCoroutine(interstitialAdCounter());
        }
    }


}


