using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maps;
using System.Linq;

public class NearestHotspot : MonoBehaviour {
	public MapFunctionality map;
	public GameObject player;
    public GameObject compass;
    public GameObject tooFar;

    private Hotspot[] hotspots;
	private Dictionary<Hotspot, float> distances;
	private float minimumDistance = -1;
    private Hotspot closestHotspot;

    private void Start()
    {
        hotspots = DataController.Instance.player_data.hotspots;
    }
    private void Update()
    {
        Call();
		if (distances.Count == 0) // Deactivates compass if all hotspots are already visited
		{
			compass.SetActive(false);
			tooFar.SetActive(false);
			return;
		}
        Vector3 hotspot_pos = map.MapPositionAt((float)closestHotspot.longitude, (float)closestHotspot.latitude);
        compass.transform.LookAt(hotspot_pos, Vector3.up);
        if(minimumDistance > 140)
        {
            tooFar.SetActive(true);
        }
        else
        {
            tooFar.SetActive(false);
        }
    }

    // Finds the minimum distance to the nearest hotspot and the hotspot itself
    private void Call ()
	{
        distances = new Dictionary<Hotspot, float>();
        foreach (Hotspot hotspot in hotspots) {
            if(hotspot.visited == false)
            {
                distances.Add(hotspot, CalculateDistance(hotspot));
            }
		}
		if (distances.Count == 0) return;

        //Finds the minimum value from the distances dictionary along with respective hotspot object
        KeyValuePair<Hotspot, float> firstHotspot = distances.First();
        minimumDistance = firstHotspot.Value;
        closestHotspot = firstHotspot.Key;
        foreach (KeyValuePair<Hotspot, float> distance in distances)
        {
            if(minimumDistance > distance.Value)
            {
                minimumDistance = distance.Value;
                closestHotspot = distance.Key;
            }
        }
	}

	// Returns false if the player is too far from a hotspot, true otherwise
	private bool CloseEnough (float minimum)
	{
		if (minimum < minimumDistance) {
			return false;
		}
		return true;
	}

	// Calculates the distance to the nearest hotspot
	private float CalculateDistance (Hotspot hotspot)
	{
		Vector3 hotspot_pos = map.MapPositionAt((float)hotspot.longitude, (float)hotspot.latitude);
		Vector3 player_pos = player.transform.position;
		return Vector3.Distance(hotspot_pos, player_pos);
	}
}
