using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StartScreensController : MonoBehaviour {
    private GameObject firstScreen;
    private GameObject fourthScreen;

    public void controllFirstScreen () {
        if (DataController.Instance.player_data.registrationCompleted == true)
        {
            firstScreen = transform.GetChild(0).gameObject;
            firstScreen.SetActive(false);

            fourthScreen = transform.GetChild(3).gameObject;
            fourthScreen.SetActive(true);
        }
    }

	public void MarkRegistrationCompleted()
    {
        DataController.Instance.player_data.registrationCompleted = true;
        DataController.Instance.Save();
    }
	
    public void GetFifthScreen()
    {
        DataController.fifthScreen = transform.GetChild(4).gameObject;
    }
}
