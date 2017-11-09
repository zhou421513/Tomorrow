using System;
using System.Collections.Generic;
using UnityEngine;
namespace Maps{
	public class MapFunctionality : MonoBehaviour {
        [HideInInspector, SerializeField]
        float MinLat, MaxLat, MinLon, MaxLon;
        [HideInInspector, SerializeField]
        Vector3 Center;
        [HideInInspector, SerializeField]
        public List<Way> Buildings, Areas, Lines;

        public Color building;
        public Color campusBuilding;
        public Color greenery;
        public Color water;
        public Color ground;
        public Color road;

        [Range(1, 10)]
        public float buildingHeightMultiplier = 1;

        Material buildingMaterial;
        Material campusBuildingMaterial;
        Material greeneryMaterial;
        Material waterMaterial;
        Material groundMaterial;
        Material roadMaterial;

        Transform buildingsHolder;
        Transform areasHolder;
        Transform linesHolder;

        private void Start()
        {
            if(Buildings == null && Areas == null && Lines == null)
                Debug.Log("jee");
            else
                Debug.Log(Buildings.Count);
        }

        public void SetMapFunctionality(Bounds bounds, List<Way> buildings, List<Way> areas, List<Way> lines)
        {
            Debug.Log(buildings.Count);
            MinLat = bounds.MinLat;
            MaxLat = bounds.MaxLat;
            MinLon = bounds.MinLon;
            MaxLon = bounds.MaxLon;
            Center = bounds.Center;
            Buildings = buildings;
            Debug.Log(Buildings.Count);
            Areas = areas;
            Lines = lines;
            buildingsHolder = new GameObject("Buildings").transform;
            buildingsHolder.parent = transform;
            areasHolder = new GameObject("Areas").transform;
            areasHolder.parent = transform;
            linesHolder = new GameObject("Ways").transform;
            linesHolder.parent = transform;
            primeMaterials();
            ShowMapArea((MaxLon + MinLon) / 2, (MaxLat + MinLat) / 2, 4000);
        }

        public void ShowMapArea(float lon, float lat, float radius)
        {
            Vector3 position = MapPosition(lon, lat);
            foreach (Way building in Buildings)
            {
                if(Vector3.Distance(building.Center - Center, position) < radius)
                {
                    BuildSolidBoundary(building);
                }
            }
            foreach (Way area in Areas)
            {
                if (Vector3.Distance(area.Center - Center, position) < radius)
                {
                    BuildFlatBoundary(area);
                }
            }
            foreach (Way line in Lines)
            {
                if (Vector3.Distance(line.Center - Center, position) < radius)
                {
                    BuildLine(line);
                }
            }
        }

        Vector3 MapPosition(float lon, float lat)
        {
            bool inLat = lat >= MinLat && lat <= MaxLat;
            bool inLon = lon >= MinLon && lon <= MaxLon;
            if (inLat && inLon)
            {
                return new Vector3(
                    (float)MercatorProjection.lonToX(lon),
                    transform.position.y,
                    (float)MercatorProjection.latToY(lat)
                ) - Center;
            }
            else
            {
                throw new Exception("Position not inside bounds!");
            }
        }
        /*
        public void SetMapFunctionality(float minLat, float maxLat, float minLon, float maxLon, Vector3 center)
        {
            MinLat = minLat;
            MaxLat = maxLat;
            MinLon = minLon;
            MaxLon = maxLon;
            Center = center;
            ShowMapArea((MaxLon + MinLon) / 2, (MaxLat + MinLat) / 2, 4000);
        }

		public void PlaceOnMap(Transform obj, float lon, float lat)
		{
			PlaceAtMap (obj, lon, lat);
			obj.parent = transform;
		}

		public void PlaceAtMap(Transform obj, float lon, float lat)
		{
            Vector3 position = MapPosition(lon, lat);
            obj.position = position;
        }

		public void ShowMapArea(float lon, float lat, float radius)
		{
            Vector3 position = MapPosition(lon, lat);
            foreach (Transform transform in transform.GetComponentsInChildren<Transform>())
            {
                if(transform.parent != this.transform && transform != this.transform)
                {
                    transform.gameObject.SetActive(Vector3.Distance(transform.localPosition, position) < radius);
                }
            }
        }
			
		Vector3 MapPosition(float lon, float lat){
			bool inLat = lat >= MinLat && lat <= MaxLat;
			bool inLon = lon >= MinLon && lon <= MaxLon;
			if (inLat && inLon)
			{
				return new Vector3 (
					(float)MercatorProjection.lonToX (lon),
					transform.position.y,
                    (float)MercatorProjection.latToY(lat)
                ) - Center;
			} 
			else {
				throw new Exception ("Position not inside bounds!");
			}
		}
        */

        void primeMaterials()
        {
            buildingMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", building, "Buildings");
            campusBuildingMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", campusBuilding, "CampusBuildings");
            greeneryMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", greenery, "Greenery");
            waterMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", water, "Water");
            groundMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", ground, "Ground");
            roadMaterial = CreateMaterial(Shader.Find("Standard"), "_Color", road, "Roads");
        }

        Material CreateMaterial(Shader shader, string colorKey, Color color, string name)
        {
            Material material = new Material(shader);
            material.SetColor(colorKey, color);
            material.name = name;
            return material;
        }

        void BuildLine(Way line)
        {
            Vector3 localOrigin = line.Center;
            GameObject go = PrimeObject(line, localOrigin);
            go.transform.parent = linesHolder;

            List<Vector3> path = GetLocalVectors(line.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.MeshFromLine(path, line.Width, line.Height);
        }

        void BuildFlatBoundary(Way area)
        {
            Vector3 localOrigin = area.Center;
            GameObject go = PrimeObject(area, localOrigin);
            go.transform.parent = areasHolder;

            List<Vector3> outline = GetLocalVectors(area.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.FlatMeshFromOutline(outline, area.Height);
        }

        void BuildSolidBoundary(Way building)
        {
            Vector3 localOrigin = building.Center;
            GameObject go = PrimeObject(building, localOrigin);
            go.transform.parent = buildingsHolder;

            List<Vector3> outline = GetLocalVectors(building.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.SolidMeshFromOutline(outline, building.Height * buildingHeightMultiplier);
        }

        GameObject PrimeObject(Way way, Vector3 localOrigin)
        {
            GameObject go = new GameObject(way.Name);
            go.transform.position = localOrigin - Center;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mr.material = buildingMaterial;
            return go;
        }

        List<Vector3> GetLocalVectors(List<Node> nodes, Vector3 localOrigin)
        {
            List<Vector3> vectors = new List<Vector3>();
            foreach (Node node in nodes)
            {
                vectors.Add(node - localOrigin);
            }
            return vectors;
        }
    }
}
