using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManagement : MonoBehaviourSingleton<AdsManagement>
{
    public string bannerId = "ca-app-pub-2240818931996169/4121211288";
    public string interstitialId = "ca-app-pub-2240818931996169/3063280481";
    public string rewardedId = "ca-app-pub-2240818931996169/7022706792";

    BannerView bannerView;
    RewardedAd rewardedAd;

    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        ShowBanner();
    }

    public void ShowRewardedAds(Action<bool> onAdsFinished)
    {
        rewardedAd = new RewardedAd(rewardedId);

        rewardedAd.OnAdLoaded += (sender, args) => rewardedAd.Show();
        rewardedAd.OnAdFailedToShow += (sender, args) => onAdsFinished?.Invoke(false);
        rewardedAd.OnUserEarnedReward += (sender, args) => onAdsFinished?.Invoke(true);


        if (PlayerPrefs.HasKey("Consent"))
        {
            var request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request);
        }
        else
        {
            var request = new AdRequest.Builder().AddExtra("npa", "1").Build();
            rewardedAd.LoadAd(request);
        }
    }

    public void ShowBanner()
    {
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);

        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        //this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        //this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        //this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        var request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdLoaded event received");
        bannerView.Show();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

}
