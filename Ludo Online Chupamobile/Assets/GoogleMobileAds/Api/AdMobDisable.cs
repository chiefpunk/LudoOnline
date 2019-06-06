using UnityEngine;
using System.Collections;

public class HouseAdsControllerDisablePanel : MonoBehaviour {

    private int hitCount = 0;
	public void disable ()
    {
        hitCount++;
        if (hitCount % 2 == 0) {
            gameObject.SetActive (false);
        }
    }
}
