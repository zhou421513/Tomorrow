using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStateController : MonoBehaviour {

    public GameObject[] regScreens;
    public GameObject[] screens;

    // Use this for initialization
    void Awake () {
        InputManager.Instance.back += Close;
        InputManager.Instance.back += GoBack;
    }

    public void GoBack()
    {
        if (regScreens[1].activeSelf)
        {
            regScreens[0].SetActive(true);
            regScreens[1].SetActive(false);
        }
        else if (regScreens[2].activeSelf)
        {
            regScreens[1].SetActive(true);
            regScreens[2].SetActive(false);
        }
    }

    public void Close()
    {
        foreach(GameObject screen in screens)
        {
            if (screen.activeSelf)
            {
                screen.SetActive(false);
                return;
            }
        }
        Application.Quit();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
