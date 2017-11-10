using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace Maps
{
    public class MapGenerator : MonoBehaviour
    {
        public TextAsset file;

        private static List<string> greenValues = new List<string>
        {
            "park","wood","forest","valley","grasland","grass","scrub","garden","meadow","cemetery"
        };
        private static List<string> waterValues = new List<string>
        {
            "bay","water","marina","dam","riverbank","fountain"
        };
        private static List<string> groundValues = new List<string>
        {
            "beach","sand","gravel","playground","pitch"
        };
        private static List<string> roadValues = new List<string>
        {
            "parking","pier","footway","asphalt","roundabout","service","driveway","parking_aisle","paved","parking_space"
        };
        private static List<string> campusValues = new List<string>
        {
            "Tampereen yliopisto","Pirkanmaan ammattikorkeakoulu Oy","Tampereen ammattikorkeakoulu","Tampereen teknillinen yliopisto"
        };
        private static List<string> buildingValues = new List<string>
        {
            "tower","chimney","observation","lighting","church","hotel","garages","fast_food","library","school","stadium","monument"
        };
        private static List<string> buildingKeys = new List<string>
        {
            "building","tower:type","rooms"
        };
        private static List<string> nameKeys = new List<string>
        {
            "name","name:en","addr:housenumber"
        };
        private static List<string[]> heightKeysMultipliers = new List<string[]>
        {
            new string[]{"building:levels","4"},new string[]{"height","1"}
        };
        private static List<string> terminateIfSingle = new List<string>
        {
            "residential","name"
        };
        private static List<string> roadlineKeys = new List<string>
        {
            "highway","railway"
        };
        private static List<string> waterlineKeys = new List<string>
        {
            "waterway"
        };
        private static List<string[]> widthKeysMultipliers = new List<string[]>
        {
            new string[]{"lanes","2"},new string[]{"width","1"}
        };

        public void GenerateMap()
        {
            XmlDocument mapData = new XmlDocument();
            mapData.LoadXml(file.text);

            Bounds bounds = new Bounds(mapData.SelectSingleNode("/osm/bounds"));
            List<Way> boundaries = new List<Way>();
            List<Way> ways = new List<Way>();

            Dictionary<ulong, Node> nodesDictionary = GetNodes(mapData.SelectNodes("/osm/node"));

            XmlNodeList xmlWays = mapData.SelectNodes("/osm/way");
            foreach (XmlNode xmlWay in xmlWays)
            {
                List<Node> nodes = GetWayNodes(xmlWay, nodesDictionary);

                if (nodes[0] == nodes[nodes.Count - 1])
                {
                    boundaries.Add(new Way(xmlWay, nodes.ToArray()));
                }
                else
                {
                    ways.Add(new Way(xmlWay, nodes.ToArray()));
                }
            }

            List<Way> buildings = new List<Way>();
            List<Way> areas = new List<Way>();
            List<Way> lines = new List<Way>();
            

            GenerateBoundaries(boundaries, buildings, areas);
            GenerateLines(ways, lines);
            GameObject map = new GameObject("Map");
            map.AddComponent<MapFunctionality>().SetMapFunctionality(bounds, buildings.ToArray(), areas.ToArray(), lines.ToArray());
        }

        void GenerateBoundaries(List<Way> boundaries, List<Way> buildings, List<Way> areas)
        {
            foreach (Way boundary in boundaries)
            {
                XmlNodeList tags = boundary.Node.SelectNodes("tag");
                if (tags.Count == 0) continue;
                if (tags.Count == 1)
                {
                    bool doContinue = false;
                    foreach (string val in terminateIfSingle)
                    {
                        if (val == XmlGetter.GetAttribute<string>("k", tags[0].Attributes) ||
                            val == XmlGetter.GetAttribute<string>("v", tags[0].Attributes))
                        {
                            doContinue = true;
                            break;
                        }
                    }
                    if (doContinue) continue;
                }


                bool isBuilding = false;
                bool isCampus = false;
                string name = null;
                float height = 0;

                foreach (XmlNode tag in tags)
                {
                    string key = XmlGetter.GetAttribute<string>("k", tag.Attributes);
                    string val = XmlGetter.GetAttribute<string>("v", tag.Attributes);
                    if (!isBuilding)
                    {
                        foreach (string buildingValue in buildingValues)
                        {
                            if (buildingValue == val)
                            {
                                isBuilding = true;
                                name = val;
                                break;
                            }
                        }
                        foreach (string buildingKey in buildingKeys)
                        {
                            if (buildingKey == key)
                            {
                                isBuilding = true;
                                break;
                            }
                        }
                    }
                    foreach (string[] keyMult in heightKeysMultipliers)
                    {
                        if (key == keyMult[0])
                        {
                            height = float.Parse(Regex.Split(val, @"\s|\D")[0]) * float.Parse(keyMult[1]);
                            isBuilding = true;
                        }
                    }
                    foreach (string nameKey in nameKeys)
                    {
                        if (key == nameKey)
                        {
                            name = val;
                        }
                    }
                    if (!isCampus)
                    {
                        foreach (string campusValue in campusValues)
                        {
                            if (val == campusValue)
                            {
                                isCampus = true;
                                break;
                            }
                        }
                    }

                }
                if (isBuilding)
                {
                    if (name == null) name = "building";
                    if (height == 0) height = 5;
                    boundary.Name = name;
                    boundary.Height = height;
                    boundary.Type = isCampus ? Way.WayType.Campus : Way.WayType.Building;
                    buildings.Add(boundary);
                }
                else
                {
                    bool typeFound = false;
                    Way.WayType type = Way.WayType.Water;
                    foreach (XmlNode tag in tags)
                    {
                        string key = XmlGetter.GetAttribute<string>("k", tag.Attributes);
                        string val = XmlGetter.GetAttribute<string>("v", tag.Attributes);
                        if (!typeFound)
                        {
                            foreach (string waterValue in waterValues)
                            {
                                if (val == waterValue)
                                {
                                    typeFound = true;
                                    height = 0f;
                                    if (name == null) name = val;
                                    break;
                                }
                            }
                            foreach (string greenValue in greenValues)
                            {
                                if (val == greenValue)
                                {
                                    typeFound = true;
                                    height = 0.1f;
                                    type = Way.WayType.Greenery;
                                    if (name == null) name = val;
                                    break;
                                }
                            }
                            foreach (string groundValue in groundValues)
                            {
                                if (val == groundValue)
                                {
                                    typeFound = true;
                                    height = 0.2f;
                                    type = Way.WayType.Ground;
                                    if (name == null) name = val;
                                    break;
                                }
                            }
                            foreach (string roadValue in roadValues)
                            {
                                if (val == roadValue)
                                {
                                    typeFound = true;
                                    height = 0.3f;
                                    type = Way.WayType.Road;
                                    if (name == null) name = val;
                                    break;
                                }
                            }
                        }
                    }
                    if (typeFound)
                    {
                        boundary.Name = name;
                        boundary.Height = height;
                        boundary.Type = type;
                        areas.Add(boundary);
                    }
                }
            }
        }

        void GenerateLines(List<Way> ways, List<Way> lines)
        {
            foreach (Way line in ways)
            {
                XmlNodeList tags = line.Node.SelectNodes("tag");
                if (tags.Count == 0) continue;
                bool road = false;
                bool water = false;
                float width = 2;
                foreach (XmlNode tag in tags)
                {
                    string key = XmlGetter.GetAttribute<string>("k", tag.Attributes);
                    string val = XmlGetter.GetAttribute<string>("v", tag.Attributes);
                    foreach (string lineKey in roadlineKeys)
                    {
                        if (lineKey == key)
                        {
                            road = true;
                            break;
                        }
                    }
                    foreach (string lineKey in waterlineKeys)
                    {
                        if (lineKey == key)
                        {
                            water = true;
                            break;
                        }
                    }
                    foreach (string[] keyMult in widthKeysMultipliers)
                    {
                        if (key == keyMult[0])
                        {
                            width = float.Parse(Regex.Split(val, @"\s|\D")[0]) * float.Parse(keyMult[1]) * 1.5f;
                        }
                    }
                }
                if (road || water)
                {
                    line.Width = width;
                    line.Height = road ? 0.3f : 0f;
                    line.Type = road ? Way.WayType.Road : Way.WayType.Water;
                    lines.Add(line);
                }
            }
        }

        static List<Node> GetWayNodes(XmlNode node, Dictionary<ulong, Node> nodesDictionary)
        {
            List<Node> nodes = new List<Node>();

            XmlNodeList nds = node.SelectNodes("nd");

            foreach (XmlNode n in nds)
            {
                nodes.Add(nodesDictionary[XmlGetter.GetAttribute<ulong>("ref", n.Attributes)]);
            }

            return nodes;
        }

        static Dictionary<ulong, Node> GetNodes(XmlNodeList xmlNodeList)
        {
            Dictionary<ulong, Node> nodes = new Dictionary<ulong, Node>();

            foreach (XmlNode n in xmlNodeList)
            {
                Node node = new Node(n);
                nodes[node.ID] = node;
            }

            return nodes;
        }
    }
}
