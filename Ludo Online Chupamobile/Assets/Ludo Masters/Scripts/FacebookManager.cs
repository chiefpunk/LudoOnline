using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine.UI;
using AssemblyCSharp;

public class FacebookManager : MonoBehaviour
{
    public GameObject facebookLoginButton;
    public GameObject guestLoginButton;
    private PlayFabManager playFabManager;
    public string name;
    public Sprite sprite;
    private GameObject FbLoginButton;
    private bool LoggedIn = false;
    private FacebookFriendsMenu facebookFriendsMenu;
    private bool alreadyGotFriends = false;
    public GameObject splashCanvas;
    public GameObject loginCanvas;
    public GameObject fbButton;
    public GameObject matchPlayersCanvas;
    public GameObject menuCanvas;
    public GameObject gameTitle;
    public GameObject idLoginDialog;
    public GameObject idRegisterDialog;
    public GameObject forgetPasswordDialog;


    public InputField loginEmail;
    public InputField loginPassword;
    public GameObject loginInvalidEmailorPassword;

    public InputField regiterEmail;
    public InputField registerPassword;
    public InputField registerNickname;
    public GameObject registerInvalidInput;

    public InputField resetPasswordEmail;
    public GameObject resetPasswordInformationText;



    void Start()
    {
        Debug.Log("FBManager start");
        GameManager.Instance.facebookManager = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        FbLoginButton = GameObject.Find("FbLoginButton");

        facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu; //.GetComponent <FacebookFriendsMenu> ();
    }

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        Debug.Log("FBManager awake");
        GameManager.Instance.facebookManager = this;
        DontDestroyOnLoad(transform.gameObject);
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        if (!GameManager.Instance.logged)
        {

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
    		if (!FB.IsInitialized) {
    			// Initialize the Facebook SDK
    			FB.Init(InitCallback, OnHideUnity);
    		} else {
    			// Already initialized, signal an app activation App Event
    			FB.ActivateApp();
                initSession();
    		}
#else
            initSession();
#endif

            GameManager.Instance.logged = true;
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

            //			if (PlayerPrefs.GetString ("LoggedType").Equals ("Facebook")) {
            //				Debug.Log ("Already logged to facebook!!!!");
            initSession();
            //			}

        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void startRandomGame()
    {
        //		menuCanvas.SetActive (false);
        //		gameTitle.SetActive (false);

        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().setBackButton(true);
        //		matchPlayersCanvas.GetComponent <SetMyData> ().MatchPlayer ();
        //		matchPlayersCanvas.GetComponent <SetMyData> ().setBackButton (true);
        playFabManager.JoinRoomAndStartGame();
    }


    public void FBLogin()
    {
        if (!LoggedIn)
        {
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        else
        {
            playFabManager.JoinRoomAndStartGame();
        }

    }

    public void FBLinkAccount()
    {
        GameManager.Instance.LinkFbAccount = true;
        FBLogin();
    }

    public void FBLoginWithoutLink()
    {
        GameManager.Instance.LinkFbAccount = false;
        FBLogin();
    }




    public void GuestLogin()
    {
        if (!LoggedIn)
        {
            playFabManager.Login();
        }
    }

    public void showRegisterDialog()
    {
        idLoginDialog.SetActive(false);
        idRegisterDialog.SetActive(true);
    }

    public void CloseLoginDialog()
    {
        loginInvalidEmailorPassword.SetActive(false);
        loginEmail.text = "";
        loginPassword.text = "";

        loginCanvas.SetActive(true);
        idLoginDialog.SetActive(false);
    }

    public void CloseRegisterDialog()
    {
        regiterEmail.text = "";
        registerPassword.text = "";
        registerNickname.text = "";
        registerInvalidInput.SetActive(false);
        loginCanvas.SetActive(true);
        idRegisterDialog.SetActive(false);
    }

    public void CloseForgetPasswordDialog()
    {
        resetPasswordEmail.text = "";
        resetPasswordInformationText.SetActive(false);
        forgetPasswordDialog.SetActive(false);
        loginCanvas.SetActive(true);
    }

    public void showForgetPasswordDialog()
    {
        forgetPasswordDialog.SetActive(true);
        idLoginDialog.SetActive(false);



    }

    public void IDLoginButtonPressed()
    {
        loginCanvas.SetActive(false);
        idLoginDialog.SetActive(true);
    }

    public void IDLogin()
    {
        if (!LoggedIn)
        {
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID

            GameManager.Instance.facebookIDMy = aToken.UserId;
            // Print current access token's granted permissions
            Debug.Log(aToken.ToJson());
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            //			getFacebookInvitableFriends ();
            //			getFacebookFriends ();
            //			callApiToGetName ();
            //			getMyProfilePicture (GameManager.Instance.facebookIDMy);
            //
            //
            //			LoggedIn = true;
            //
            //			GameObject.Find ("FbLoginButtonText").GetComponent <Text>().text = "Play";

            PlayerPrefs.SetString("LoggedType", "Facebook");
            PlayerPrefs.Save();

            if (!GameManager.Instance.LinkFbAccount)
            {
                loginCanvas.SetActive(false);
                splashCanvas.SetActive(true);

            }

            initSession();
        }
        else
        {
            facebookLoginButton.GetComponent<Button>().interactable = true;
            guestLoginButton.GetComponent<Button>().interactable = true;
            Debug.Log("User cancelled login");
        }
    }

    private void initSession()
    {
        Debug.Log("FbManager init session");

        string logType = PlayerPrefs.GetString("LoggedType");
        if (logType.Equals("Facebook"))
        {
            GameManager.Instance.facebookIDMy = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
            callApiToGetName();
            getMyProfilePicture(GameManager.Instance.facebookIDMy);

            LoggedIn = true;

        }
        else if (logType.Equals("Guest"))
        {
            playFabManager.Login();
        }
        else if (logType.Equals("EmailAccount"))
        {
            playFabManager.LoginWithEmailAccount();
        }

    }

    private void callApiToGetName()
    {
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, APICallbackName);
    }


    void APICallbackName(IResult response)
    {
        GameManager.Instance.nameMy = response.ResultDictionary["name"].ToString();
        Debug.Log("My name " + GameManager.Instance.nameMy);
    }

    public void getMyProfilePicture(string userID)
    {

        //FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate(IGraphResult result) {
        FB.API("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Error == null)
            {

                // use texture

                Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

                if (reqResult == null) Debug.Log("JEST NULL"); else Debug.Log("nie null");


                GameManager.Instance.avatarMyUrl = ((reqResult["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
                Debug.Log("My avatar " + GameManager.Instance.avatarMyUrl);

                StartCoroutine(loadImageMy(GameManager.Instance.avatarMyUrl));

                //GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

                if (GameManager.Instance.LinkFbAccount)
                {
                    playFabManager.LinkFacebookAccount();
                }
                else
                {
                    playFabManager.LoginWithFacebook();
                }
            }
            else
            {
                Debug.Log("Error retreiving image: " + result.Error);
            }
        });
    }

    public IEnumerator loadImageMy(string url)
    {
        // Load avatar image

        // Start a download of the given URL
        WWW www = new WWW(url);

        // Wait for download to complete
        yield return www;


        GameManager.Instance.avatarMy = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
        GameManager.Instance.facebookAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
    }

    public void getOpponentProfilePicture(string userID)
    {

        FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Texture != null)
            {
                // use texture

                GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

                playFabManager.LoginWithFacebook();
            }
        });
    }





    public void getFacebookInvitableFriends()
    {
        if (alreadyGotFriends)
        {
            facebookFriendsMenu.showFriends();
        }
        else
        {
            //&fields=picture.width(100).height(100)
            FB.API("/me/invitable_friends?limit=5000&fields=id,name,picture.width(100).height(100)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
            {

                if (result.Error == null)
                {
                    List<string> friendsNames = new List<string>();
                    List<string> friendsIDs = new List<string>();
                    List<string> friendsAvatars = new List<string>();
                    //Grab all requests in the form of a dictionary. 

                    Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    //Grab 'data' and put it in a list of objects. 


                    List<object> newObj = reqResult["data"] as List<object>;
                    //For every item in newObj is a separate request, so iterate on each of them separately. 


                    Debug.Log("Friends Count: " + newObj.Count);
                    for (int xx = 0; xx < newObj.Count; xx++)
                    {
                        Dictionary<string, object> reqConvert = newObj[xx] as Dictionary<string, object>;

                        string name = reqConvert["name"] as string;
                        string id = reqConvert["id"] as string;

                        //friendsNames.Add (name);
                        //friendsIDs.Add (id);

                        Dictionary<string, object> avatarDict = reqConvert["picture"] as Dictionary<string, object>;
                        avatarDict = avatarDict["data"] as Dictionary<string, object>;

                        string avatarUrl = avatarDict["url"] as string;
                        //friendsAvatars.Add (avatarUrl);
                        //Debug.Log("URL: " + avatarUrl);
                        GameManager.Instance.facebookFriendsMenu.AddFacebookFriend(name, id, avatarUrl);
                    }

                    //					alreadyGotFriends = true;
                    //					facebookFriendsMenu.showFriends (friendsNames, friendsIDs, friendsAvatars);



                }
                else
                {
                    Debug.Log("Something went wrong. " + result.Error + "  " + result.ToString());
                }

            });

        }
    }


    public void destroy()
    {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }

    public void showLoadingCanvas()
    {
        loginCanvas.SetActive(false);
        splashCanvas.SetActive(true);
    }



}
