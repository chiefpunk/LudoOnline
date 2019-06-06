using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoinsFrameValue : MonoBehaviour
{

    private int currentValue = 0;
    private Text text;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        InvokeRepeating("CheckAndUpdateValue", 0.2f, 0.2f);
    }

    private void CheckAndUpdateValue()
    {
        if (currentValue != GameManager.Instance.myPlayerData.GetCoins())
        {
            currentValue = GameManager.Instance.myPlayerData.GetCoins();
            if (currentValue != 0)
            {
                text.text = GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
            }
            else
            {
                text.text = "0";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
