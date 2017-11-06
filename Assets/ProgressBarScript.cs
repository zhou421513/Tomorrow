using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour {
    public float MaxNHotSpots;
    public float NCollectedHotSpots;

    public Slider ProgressBar;
	// Use this for initialization
	void Start () {
        MaxNHotSpots = 5f;
        NCollectedHotSpots = 0f;

        ProgressBar.value = CurrentProgress();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.X))
        {
            NewHotSpotReached();
        }
	}

    void NewHotSpotReached()
    {
        if (NCollectedHotSpots >= MaxNHotSpots)
        {
            return;
        }
        NCollectedHotSpots++;
        ProgressBar.value = CurrentProgress();
    }

    float CurrentProgress()
    {
        return NCollectedHotSpots / MaxNHotSpots;
    }
}
