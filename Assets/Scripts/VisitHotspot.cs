using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitHotspot {

	private PlayerData player_data;
	private int visited_count;

	// Called when a hotspot needs to be marked as visited
	// changes the `visited` value to true
	public void call (int hotspot_id) {
		player_data = DataController.Instance.player_data;
		foreach (Hotspot hotspot in player_data.hotspots) {
			// Finds the hotspot object if there's a non-visited one with that id
			if (hotspot_id == hotspot.id && hotspot.visited == false) {
				hotspot.visited = true;
				player_data.visited_count++;
				setProgress();
				break;
			}
		}
		DataController.Instance.Save();
        IsGameFinished();
	}

	private void setProgress () {
		player_data.progress = (float)player_data.visited_count/player_data.hotspots_count;
	}

    private void IsGameFinished()
    {
        if(DataController.Instance.player_data.hotspots.Length == DataController.Instance.player_data.visited_count && DataController.Instance.player_data.GameCompletionPageShown == false)
        {
            DataController.fifthScreen.SetActive(true);
            DataController.Instance.player_data.GameCompletionPageShown = true;
        }
    }
}
