using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Maps
{
    public class Way : System.Object
    {
        public XmlNode Node { get; private set; }
        public List<Node> Nodes { get; private set; }
        public Vector3 Center { get; private set; }
        public string Name { get; set; }
        public float Height { get; set; }
        public float Width{ get; set; }

        public Way(XmlNode node, List<Node> nodes)
        {
            Node = node;
            Nodes = nodes;
            Center = GetCenter();
        }

        public Vector3 GetCenter()
        {
            Vector3 total = Vector3.zero;

            foreach (Node node in Nodes)
            {
                total += node;
            }

            return total / Nodes.Count;
        }
    }
}
