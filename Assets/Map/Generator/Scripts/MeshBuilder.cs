using System.Collections.Generic;
using UnityEngine;

namespace Maps
{
    class MeshBuilder
    {
        public static Mesh SolidMeshFromOutline(List<Vector3> outline, float height)
        {
            Mesh mesh = new Mesh();

            if (outline.Count < 3 || outline[0] != outline[outline.Count - 1])
                return mesh;

            Vector3 heightVector = new Vector3(0, height, 0);

            List<Vector3> vectors = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int i = 1; i < outline.Count; i++)
            {
                vectors.Add(outline[i]);
                vectors.Add(outline[i - 1]);
                vectors.Add(outline[i - 1] + heightVector);

                vectors.Add(outline[i - 1] + heightVector);
                vectors.Add(outline[i - 1]);
                vectors.Add(outline[i]);

                vectors.Add(outline[i]);
                vectors.Add(outline[i] + heightVector);
                vectors.Add(outline[i - 1] + heightVector);

                vectors.Add(outline[i - 1] + heightVector);
                vectors.Add(outline[i] + heightVector);
                vectors.Add(outline[i]);
            }

            for (int i = 0; i < vectors.Count; i++)
            {
                indices.Add(i);
            }

            int len = vectors.Count;

            List<Vector3> topPoints = new List<Vector3>();
            for (int i = 0; i < outline.Count - 1; i++)
            {
                vectors.Add(outline[i] + heightVector);
                topPoints.Add(outline[i] + heightVector);
            }

            List<int> topIndices = Triangulator.TriangulatePlane(topPoints);
            foreach (int i in topIndices)
            {
                indices.Add(i + len);
            }

            mesh.vertices = vectors.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.uv.Initialize();

            return mesh;
        }

        public static Mesh FlatMeshFromOutline(List<Vector3> outline, float height)
        {
            Mesh mesh = new Mesh();

            if (outline.Count < 3 || outline[0] != outline[outline.Count - 1])
                return mesh;

            Vector3 heightVector = new Vector3(0, height, 0);

            List<Vector3> vectors = new List<Vector3>();
            for (int i = 0; i < outline.Count - 1; i++)
            {
                vectors.Add(outline[i] + heightVector);
            }
            List<int> indices = Triangulator.TriangulatePlane(vectors);

            

            mesh.vertices = vectors.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.uv.Initialize();

            return mesh;
        }

        public static Mesh MeshFromLine(List<Vector3> line, float width, float height)
        {
            Mesh mesh = new Mesh();

            if (line.Count < 2)
                return mesh;

            Vector3 heightVector = new Vector3(0, height, 0);

            List<Vector3> vectors = new List<Vector3>();
            for (int i = 0; i < line.Count; i++)
            {
                float halfWidth = width / 2;
                Vector3 right;
                if (i == 0)
                {
                    right = Vector3.Cross(line[i] - line[i + 1], Vector3.up).normalized + heightVector;
                }
                else if (i == line.Count - 1)
                {
                    right = Vector3.Cross(line[i - 1] - line[i], Vector3.up).normalized + heightVector;
                }
                else
                {
                    Vector3 previous = line[i - 1] - line[i];
                    Vector3 next = line[i + 1] - line[i];

                    Vector3 average = (previous.normalized + next.normalized).normalized;

                    Vector3 previousRight = Vector3.Cross(previous, Vector3.up).normalized;

                    if (average.magnitude == 0)
                    {
                        right = previousRight;
                    }
                    else if (Vector3.Dot(previousRight, average) > 0)
                    {
                        right = average;
                    }
                    else
                    {
                        right = -average;
                    }
                    halfWidth = halfWidth * (1 + 1 - Vector3.Angle(previous, next) / 180);
                }
                vectors.Add(line[i] + right * halfWidth);
                vectors.Add(line[i] + right * -halfWidth);
            }

            List<int> indices = new List<int>();
            for (int i = 3; i < vectors.Count; i += 2)
            {
                indices.Add(i - 3);
                indices.Add(i - 2);
                indices.Add(i - 1);

                indices.Add(i);
                indices.Add(i - 1);
                indices.Add(i - 2);
            }

            mesh.vertices = vectors.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.uv.Initialize();

            return mesh;
        }
    }
}

