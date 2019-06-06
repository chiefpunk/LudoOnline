using UnityEngine;
using System.Collections;

public class AdMobObjectSingleton {

    private static AdMobObjectSingleton instance;

    private AdMobObjectSingleton() {
    }

    public bool houseAdDisplayed = false;

    public static AdMobObjectSingleton Instance {
        get {
            if (instance == null) {
                instance = new AdMobObjectSingleton();
            }
            return instance;
        }
    }
}
