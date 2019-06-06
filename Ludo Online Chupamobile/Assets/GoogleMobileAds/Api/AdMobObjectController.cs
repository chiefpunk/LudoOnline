using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AdMobObjectController : MonoBehaviour
{

    private bool AdShowed = true;
    private bool myGame = false;


    private int showAttempts = 0;
    public bool loadedAdmob = false;

    private string AndroidCallerID = "android";
    private string IOSCallerID = "ios";
    private string WPCallerID = "wp";
    private WWW www_image = null;
    public string[] frames;
    public int[] adsShowed;

    private string APIMainURL = "http://houseadsserver.com/ServerPlay/";

    private string APIUrl = "";

    private bool canPushButtons = true;

    public GameObject admobAdsObject;
    public GameObject adView;
    public GameObject loadingPanel;
    public GameObject adsController;
    private AdsController adControl;
    private string storeAppID;

    private int attempts = 0;
    private bool isVisible = false;
    private float volume;

    // Use this for initialization
    void Start()
    {


    }

    public void Init()
    {
        APIUrl = APIMainURL + "/default.php?";
        adControl = adsController.GetComponent<AdsController>();
        AdsManager.Instance.adsScript = this;

        if (!AdMobObjectSingleton.Instance.houseAdDisplayed && AdShowed)
        {
            StartCoroutine(DownloadAdData());
        }
    }

    public void destroy()
    {
        if (admobAdsObject != null)
            DestroyImmediate(admobAdsObject);
    }

    IEnumerator DownloadAdData()
    {
        string os;
        string calling_app;// = AndroidCallerID;

#if UNITY_ANDROID
        os = "android";
        calling_app = AndroidCallerID;
#elif UNITY_IOS
        os = "ios";
        calling_app = IOSCallerID;
#else
        os = "wp";
        calling_app = WPCallerID;
#endif

        yield return new WaitForSeconds(2);

        WWW www = new WWW(APIUrl + "os=" + os + "&calling_app=" + calling_app);
        yield return www;

        if (www.error == null && www.text.Contains("API_DATA_BEGIN|"))
        {
            string image_url = www.text.Substring(www.text.IndexOf("API_DATA_BEGIN|") + "API_DATA_BEGIN|".Length, www.text.Length - "API_DATA_BEGIN|".Length - "|API_DATA_END".Length - 1);

            string[] splitted = image_url.Split(';');



            frames = splitted[1].Split('-');

            adsShowed = new int[frames.Length];

            for (int i = 0; i < frames.Length; i++)
            {
                adsShowed[i] = Int32.Parse(frames[i]);
            }

            image_url = splitted[0];

            storeAppID = image_url;
            //Debug.Log (www.text.Substring(www.text.IndexOf("API_DATA_BEGIN|") + "API_DATA_BEGIN|".Length, www.text.Length - "API_DATA_BEGIN|".Length - "|API_DATA_END".Length - 1));

            // Downloading image
            string img_url = "";
            if (Screen.orientation == ScreenOrientation.Landscape)
            {
#if UNITY_ANDROID
                img_url = APIMainURL + "Android_PNG/Landscape/" + image_url + ".png";
#elif UNITY_IOS
                img_url = APIMainURL + "iOS_PNG/Landscape/" + image_url + ".png";
#else
                img_url = APIMainURL + "WP_PNG/Landscape/" + image_url + ".png";
#endif

            }
            else if (Screen.orientation == ScreenOrientation.Portrait)
            {
#if UNITY_ANDROID
                img_url = APIMainURL + "Android_PNG/Portrait/" + image_url + ".png";
#elif UNITY_IOS
                img_url = APIMainURL + "iOS_PNG/Portrait/" + image_url + ".png";
#else
                img_url = APIMainURL + "WP_PNG/Portrait/" + image_url + ".png";
#endif
            }



            www_image = new WWW(img_url);





        }
    }

    public void ShowAd(AdLocation location)
    {

        if (loadedAdmob)
        {

            if (location == AdLocation.GameOver && !adControl.ShowAdOnGameOver) return;
            if (location == AdLocation.GameStart && !adControl.ShowAdOnMenuScene) return;
            if (location == AdLocation.Pause && !adControl.ShowAdOnPause) return;
            if (location == AdLocation.LevelComplete && !adControl.ShowAdOnLevelFinish) return;
            if (location == AdLocation.FacebookFriends && !adControl.ShowAdOnFacebookFriends) return;
            if (location == AdLocation.GameFinishWindow && !adControl.ShowAdOnGameFinishWindow) return;
            if (location == AdLocation.StoreWindow && !adControl.ShowAdOnStoreWindow) return;
            if (location == AdLocation.GamePropertiesWindow && !adControl.ShowAdOnGamePropertiesWindow) return;


            showAttempts++;

            bool showH = false;

            for (int i = 0; i < adsShowed.Length; i++)
            {
                if (adsShowed[i] == showAttempts)
                {
                    showH = true;
                    break;
                }
            }

            if (myGame) showH = false;


            if (showH)
            {
#if !UNITY_EDITOR
                if (www_image != null && www_image.error == null && www_image.texture.width != 8 && www_image.texture.height != 8)
                {

                    adView.GetComponent<Image>().sprite = Sprite.Create(www_image.texture, new Rect(0, 0, www_image.texture.width, www_image.texture.height), new Vector2(0.5f, 0.5f));

                    loadingPanel.SetActive(true);

                    AdMobObjectSingleton.Instance.houseAdDisplayed = true;
                    Invoke("enableButtons", 2);
                    Screen.fullScreen = false;
                    isVisible = true;
                    volume = AudioListener.volume;
                    AudioListener.volume = 0;

                }
                else
                {
                    GameManager.Instance.adsController.loadAd(location);
                }
#else
                GameManager.Instance.adsController.loadAd(location);
#endif
            }
            else
            {
                GameManager.Instance.adsController.loadAd(location);
            }



        }
    }

    public void LoadHouse()
    {

        if (PlayerPrefs.GetInt("HouseTryLoad", 0) >= 2 && adsShowed[0] == 1)
        {

            if (www_image != null && www_image.error == null && www_image.texture.width != 8 && www_image.texture.height != 8)
            {
#if UNITY_ANDROID
                if (storeAppID == Application.identifier)
                {
                    return;
                }
#endif

                adView.GetComponent<Image>().sprite = Sprite.Create(www_image.texture, new Rect(0, 0, www_image.texture.width, www_image.texture.height), new Vector2(0.5f, 0.5f));

                loadingPanel.SetActive(true);

                AdMobObjectSingleton.Instance.houseAdDisplayed = true;
                Invoke("enableButtons", 2);
                Screen.fullScreen = false;
                isVisible = true;
                volume = AudioListener.volume;
                AudioListener.volume = 0;

            }
        }
        else
        {
            PlayerPrefs.SetInt("HouseTryLoad", PlayerPrefs.GetInt("HouseTryLoad", 0) + 1);
        }

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isVisible)
        {
            loadingPanel.SetActive(false);
            Screen.fullScreen = true;
            isVisible = false;
            AudioListener.volume = volume;

        }
    }


    public void openAppStore()
    {
        if (canPushButtons)
        {
            Debug.Log("Open store!!");

#if UNITY_ANDROID
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "market://details?id=" + storeAppID);

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject(
                            "android.content.Intent",
                            intentClass.GetStatic<string>("ACTION_VIEW"),
                            uriObject
            );

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/id" + storeAppID);
#else
            Application.OpenURL("ms-windows-store:navigate?appid=" + storeAppID);
#endif



        }
    }

    public void closeAd()
    {
        if (canPushButtons)
        {
            loadingPanel.SetActive(false);
            Screen.fullScreen = true;
            isVisible = false;
            AudioListener.volume = volume;

        }
    }

    private void enableButtons()
    {
        canPushButtons = true;
    }
}
