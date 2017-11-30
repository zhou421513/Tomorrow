using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

public class BarLength : MonoBehaviour {

    public Text txt;
    void Update ()
    {
        transform.localScale = new Vector3(DataController.Instance.player_data.progress, 1, 1);
        txt.text = DataController.Instance.player_data.visited_count + " / " + DataController.Instance.player_data.hotspots_count;
	}
}
