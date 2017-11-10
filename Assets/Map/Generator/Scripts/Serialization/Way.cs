using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Maps
{
    [Serializable]
    public class Way : System.Object
    {

        public enum WayType { Building, Campus, Greenery, Water, Ground, Road};
        public XmlNode Node { get; private set; }
        [SerializeField]
        public Node[] Nodes;
        [SerializeField]
        public Vector3 Center;
        [SerializeField]
        public string Name;
        [SerializeField]
        public float Height, Width;
        [SerializeField]
        public WayType Type;
        public Way(XmlNode node, Node[] nodes)
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

            return total / Nodes.Length;
        }
    }
}
