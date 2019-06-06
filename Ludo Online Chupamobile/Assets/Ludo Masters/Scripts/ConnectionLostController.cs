using UnityEngine;
using System.Collections;

public class ConnectionLostController : MonoBehaviour {

    // Use this for initialization
    public GameObject canvas;

    void Start() {
        DontDestroyOnLoad(transform.gameObject);
        GameManager.Instance.connectionLost = this;

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            showDialog();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void destroy() {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }

    public void showDialog() {
        canvas.SetActive(true);
    }

    public void closeApp() {
        Application.Quit();
    }
}
