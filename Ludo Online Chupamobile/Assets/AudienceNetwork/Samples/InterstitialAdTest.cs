using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;

public class InterstitialAdTest : MonoBehaviour
{

    private InterstitialAd interstitialAd;
    private bool isLoaded;

    // UI elements in scene
    public Text statusLabel;

    // Load button
    public void LoadInterstitial ()
    {
        this.statusLabel.text = "Loading interstitial ad...";

        // Create the interstitial unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        InterstitialAd interstitialAd = new InterstitialAd ("YOUR_PLACEMENT_ID");
        this.interstitialAd = interstitialAd;
        this.interstitialAd.Register (this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.interstitialAd.InterstitialAdDidLoad = (delegate() {
            Debug.Log ("Interstitial ad loaded.");
            this.isLoaded = true;
            this.statusLabel.text = "Ad loaded. Click show to present!";
        });
        interstitialAd.InterstitialAdDidFailWithError = (delegate(string error) {
            Debug.Log ("Interstitial ad failed to load with error: " + error);
            this.statusLabel.text = "Interstitial ad failed to load. Check console for details.";
        });
        interstitialAd.InterstitialAdWillLogImpression = (delegate() {
            Debug.Log ("Interstitial ad logged impression.");
        });
        interstitialAd.InterstitialAdDidClick = (delegate() {
            Debug.Log ("Interstitial ad clicked.");
        });

        // Initiate the request to load the ad.
        this.interstitialAd.LoadAd ();
    }

    // Show button
    public void ShowInterstitial ()
    {
        if (this.isLoaded) {
            this.interstitialAd.Show ();
            this.isLoaded = false;
            this.statusLabel.text = "";
        } else {
            this.statusLabel.text = "Ad not loaded. Click load to request an ad.";
        }
    }

    void OnDestroy ()
    {
        // Dispose of interstitial ad when the scene is destroyed
        if (this.interstitialAd != null) {
            this.interstitialAd.Dispose ();
        }
        Debug.Log ("InterstitialAdTest was destroyed!");
    }

    // Next button
    public void NextScene ()
    {
        SceneManager.LoadScene ("AdViewScene");
    }
}
