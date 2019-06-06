using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager
{


    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AdsManager();
            }
            return _instance;
        }
    }

    public AdsController interstitialAds;

    public AdMobObjectController adsScript;

    public void showAd(AdLocation location)
    {
        adsScript.ShowAd(location);
    }

}
