using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitHotspot {

	private GameController data_manager = GameController.instance;
	private PlayerData player_data;
	private int visited_count;

	// Called when a hotspot needs to be marked as visited
	// changes the `visited` value to true
	public void call (int hotspot_id) {
		player_data = data_manager.player_data;
		visited_count = data_manager.visited_count;
		foreach (Hotspot hotspot in player_data.hotspots) {
			if (hotspot_id == hotspot.id) {
				hotspot.visited = true;
				visited_count++;
				break;
			}
		}
		data_manager.Save();
	}
}
