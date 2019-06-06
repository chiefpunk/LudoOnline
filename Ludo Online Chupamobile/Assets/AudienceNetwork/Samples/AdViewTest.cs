using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;

public class AdViewTest : MonoBehaviour
{

    private AdView adView;

    void Awake ()
    {
        // Create a banner's ad view with a unique placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        AdView adView = new AdView ("YOUR_PLACEMENT_ID", AdSize.BANNER_HEIGHT_50);
        this.adView = adView;
        this.adView.Register (this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.adView.AdViewDidLoad = (delegate() {
            Debug.Log ("Ad view loaded.");
            this.adView.Show (100);
        });
        adView.AdViewDidFailWithError = (delegate(string error) {
            Debug.Log ("Ad view failed to load with error: " + error);
        });
        adView.AdViewWillLogImpression = (delegate() {
            Debug.Log ("Ad view logged impression.");
        });
        adView.AdViewDidClick = (delegate() {
            Debug.Log ("Ad view clicked.");
        });

        // Initiate a request to load an ad.
        adView.LoadAd ();
    }

    void OnDestroy ()
    {
        // Dispose of banner ad when the scene is destroyed
        if (this.adView) {
            this.adView.Dispose ();
        }
        Debug.Log ("AdViewTest was destroyed!");
    }

    // Next button
    public void NextScene ()
    {
        SceneManager.LoadScene ("NativeAdScene");
    }
}
