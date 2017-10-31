using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController control;

	public int visited_count;
	public int hotspots_count;
	public HotspotsList hotspots_data;

	private string jsonString;
	private Hotspot visited_hotspot;

	// Calls Load() function
	void Start () {
		Load();
		// VisitHotspot(123);
		// Debug.Log(hotspots_data.hotspots[1].visited);
	}

	void Awake () {
		if (control == null) {
			DontDestroyOnLoad(gameObject);
			control = this;
		}
		else if (control != this) {
			Destroy(gameObject);
		}
	}

	// If it's the first time that user opens the game, it calls FirstLoad function
	// otherwise, it loads the saved data from `playerInfo.dat` file
	void Load () {
		// When Game has been opened before
		if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			hotspots_data = (HotspotsList)bf.Deserialize(file);
			file.Close();
		}
		else { // When the app starts for the first time
			FirstLoad();
		}
	}

	// Gets called when user opens the app for the first time
	// loads, parses and saves the json into `playerInfo.dat` file
	void FirstLoad () {
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/hotspots_list.json");
		hotspots_data = JsonUtility.FromJson<HotspotsList>(jsonString);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		bf.Serialize(file, hotspots_data);
		file.Close();
	}

	// Saves the current `hotspots_data` into the file
	void Save () {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
		bf.Serialize(file, hotspots_data);
		file.Close();
	}

	// Called when a hotspot needs to be marked as visited
	// changes the `visited` value to true
	void VisitHotspot (int hotspot_id) {
		foreach (Hotspot hotspot in hotspots_data.hotspots) {
			if (hotspot_id == hotspot.id) {
				hotspot.visited = true;
				break;
			}
		}
		Save();
	}
}
