using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetMyData : MonoBehaviour
{

    public GameObject avatar;
    public GameObject name;
    public GameObject matchCanvas;
    public GameObject controlAvatars;
    public GameObject backButton;


    // Use this for initialization


    public void MatchPlayer()
    {

        //name.GetComponent<Text>().text = GameManager.Instance.nameMy;
        if (GameManager.Instance.avatarMy != null)
            avatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;


        controlAvatars.GetComponent<ControlAvatars>().reset();

    }

    public void setBackButton(bool active)
    {
        backButton.SetActive(active);
    }
}
