using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using AssemblyCSharp;

public class InterstitialAdsControllerScript : MonoBehaviour {


    private InterstitialAd interstitial;
    // Use this for initialization
    void Start() {

        DontDestroyOnLoad(transform.gameObject);
        GameManager.Instance.interstitialAds = this;
        RequestInterstitial();
    }

    private void RequestInterstitial() {
#if UNITY_ANDROID
        string adUnitId = StaticStrings.adMobAndroidID;
#elif UNITY_IPHONE
        string adUnitId = StaticStrings.adMobiOSID;;
#else
            string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.

        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);

        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        interstitial.OnAdClosed += HandleOnAdClosed;

    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
        print("OnAdLoaded event received.");
        GameManager.Instance.adsScript.loadedAdmob = true;
        // Handle the ad loaded event.
    }

    public void HandleOnAdOpened(object sender, EventArgs args) {
        print("OnAdOpened event received.");
        // Handle the ad loaded event.
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        print("OnAdClosed event received.");
        // Handle the ad loaded event.
        RequestInterstitial();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        print("Interstitial Failed to load: " + args.Message);
        // Handle the ad failed to load event.
    }

    public void showInterstitial() {
        if (interstitial.IsLoaded()) {
            interstitial.Show();
        }
    }
}
