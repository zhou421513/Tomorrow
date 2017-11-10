using System;
using System.Collections.Generic;
using UnityEngine;
namespace Maps{
	public class MapFunctionality : MonoBehaviour {
        [HideInInspector, SerializeField]
        float MinLat, MaxLat, MinLon, MaxLon;
        [HideInInspector, SerializeField]
        Vector3 Center;
        [SerializeField]
        public Way[] Buildings, Areas, Lines;
        
        public Color building;
        public Color campusBuilding;
        public Color greenery;
        public Color water;
        public Color ground;
        public Color road;

        [Range(1, 10)]
        public float buildingHeightMultiplier = 1;

        Material[] materials;

        Transform buildingsHolder;
        Transform areasHolder;
        Transform linesHolder;
        
        private void Awake()
        {
            buildingsHolder = new GameObject("Buildings").transform;
            buildingsHolder.localScale = transform.localScale;
            buildingsHolder.parent = transform;
            areasHolder = new GameObject("Areas").transform;
            areasHolder.localScale = transform.localScale;
            areasHolder.parent = transform;
            linesHolder = new GameObject("Ways").transform;
            linesHolder.localScale = transform.localScale;
            linesHolder.parent = transform;
            primeMaterials();
            ShowMapArea((MaxLon + MinLon) / 2, (MaxLat + MinLat) / 2, 2000);
        }
        
        public void SetMapFunctionality(Bounds bounds, Way[] buildings, Way[] areas, Way[] lines)
        {
            MinLat = bounds.MinLat;
            MaxLat = bounds.MaxLat;
            MinLon = bounds.MinLon;
            MaxLon = bounds.MaxLon;
            Center = bounds.Center;
            Buildings = buildings;
            Areas = areas;
            Lines = lines;
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

        void primeMaterials()
        {
            materials = new Material[6];
            materials[0] = CreateMaterial(Shader.Find("Standard"), "_Color", building, "Buildings");
            materials[1] = CreateMaterial(Shader.Find("Standard"), "_Color", campusBuilding, "CampusBuildings");
            materials[2] = CreateMaterial(Shader.Find("Standard"), "_Color", greenery, "Greenery");
            materials[3] = CreateMaterial(Shader.Find("Standard"), "_Color", water, "Water");
            materials[4] = CreateMaterial(Shader.Find("Standard"), "_Color", ground, "Ground");
            materials[5] = CreateMaterial(Shader.Find("Standard"), "_Color", road, "Roads");
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
            GameObject go = PrimeObject(line, localOrigin, linesHolder);

            List<Vector3> path = GetLocalVectors(line.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.MeshFromLine(path, line.Width, line.Height);
        }

        void BuildFlatBoundary(Way area)
        {
            Vector3 localOrigin = area.Center;
            GameObject go = PrimeObject(area, localOrigin, areasHolder);

            List<Vector3> outline = GetLocalVectors(area.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.FlatMeshFromOutline(outline, area.Height);
        }

        void BuildSolidBoundary(Way building)
        {
            Vector3 localOrigin = building.Center;
            GameObject go = PrimeObject(building, localOrigin, buildingsHolder);

            List<Vector3> outline = GetLocalVectors(building.Nodes, localOrigin);
            go.GetComponent<MeshFilter>().mesh = MeshBuilder.SolidMeshFromOutline(outline, building.Height * buildingHeightMultiplier);
        }

        GameObject PrimeObject(Way way, Vector3 localOrigin, Transform parent)
        {
            GameObject go = new GameObject(way.Name);
            go.transform.position = localOrigin - Center;
            go.transform.parent = parent;
            go.transform.localPosition = go.transform.position;
            go.transform.localRotation = go.transform.rotation;
            go.transform.localScale = go.transform.lossyScale;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mr.material = materials[(int)way.Type];
            return go;
        }

        List<Vector3> GetLocalVectors(Node[] nodes, Vector3 localOrigin)
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
