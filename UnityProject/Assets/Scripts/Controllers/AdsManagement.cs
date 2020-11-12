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
        MobileAds.Initialize(initStatus => {
            Debug.Log("MobileAds.Initialize");
            ShowBanner();
        });



        this.rewardedAd = new RewardedAd(rewardedId);
        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);


    }

    public void ShowRewardedAds(Action<bool> onAdsFinished)
    {
        //Debug.Log("ShowRewardedAds");

        //rewardedAd = new RewardedAd(rewardedId);

        //rewardedAd.OnAdLoaded += (sender, args) => rewardedAd.Show();
        //rewardedAd.OnAdFailedToShow += (sender, args) => onAdsFinished?.Invoke(false);
        //rewardedAd.OnUserEarnedReward += (sender, args) => onAdsFinished?.Invoke(true);


        //if (PlayerPrefs.HasKey("Consent"))
        //{
        //    var request = new AdRequest.Builder().Build();
        //    rewardedAd.LoadAd(request);
        //}
        //else
        //{
        //    var request = new AdRequest.Builder().AddExtra("npa", "1").Build();
        //    rewardedAd.LoadAd(request);
        //}
        onAdsFinished?.Invoke(false);
    }

    public void ShowBanner()
    {
        Debug.Log("Showing banner");
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);

        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        var request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
    }

    //public void HandleOnAdLoaded(object sender, EventArgs args)
    //{
    //    //MonoBehaviour.print("HandleAdLoaded event received");
    //    bannerView.Show();
    //}

    //public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    //}
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }




    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);


    }
}
