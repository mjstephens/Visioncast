using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Utility
{
    /// <summary>
    /// FROM:
    /// https://forum.unity.com/threads/creating-a-cone-shaped-mesh-through-code-how-do-i-dynamically-get-the-triangle-points.1456237/
    /// </summary>
    public class ConeMesh
    {
        #region VARIABLES

        static Vector2 trig(float rad) => new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        private static Vector3[] GetBasePoints(int vertices, float radius)
        {
            const float TAU = 2f * Mathf.PI;
            Vector3[] pts = new Vector3[vertices];
            float step = TAU / vertices; // angular step between two vertices
            for (int i = 0; i < vertices; i++)
            {
                pts[i] = radius * trig(i * step); // convert polar coordinate to cartesian space
            }

            return pts;
        }

        private static Vector3[] BuildConeVertices(Vector3[] baseVerts, float coneHeight)
        {
            if (baseVerts is null || baseVerts.Length < 3)
                throw new InvalidOperationException("Requires at least 3 base vertices.");
            var verts = new Vector3[baseVerts.Length + 1];
            verts[0] = new Vector3(0f, coneHeight, 0f);
            for (int i = 0; i < baseVerts.Length; i++)
            {
                verts[i + 1] = new Vector3(baseVerts[i].x, 0f, baseVerts[i].y);
            }

            return verts;
        }

        private static void ConstructCone(
            Vector3[] coneVerts,
            List<Vector3> finalVerts,
            List<int> triangles)
        {
            if (coneVerts is null || coneVerts.Length < 4) 
                throw new InvalidOperationException("Requires at least 4 vertices.");
            if (finalVerts is null || triangles is null) 
                throw new ArgumentNullException();

            finalVerts.Clear();
            triangles.Clear();

            var rimVertices = coneVerts.Length - 1;

            for (int i = 1; i <= rimVertices; i++)
            {
                int a = i, b = i < rimVertices - 1 ? i + 1 : 1;
                AddTriangle(coneVerts[a], coneVerts[b], coneVerts[0]);
            }

            void AddTriangle(Vector3 t1, Vector3 t2, Vector3 t3)
            {
                finalVerts.Add(t1);
                finalVerts.Add(t2);
                finalVerts.Add(t3);
                triangles.Add(finalVerts.Count - 3);
                triangles.Add(finalVerts.Count - 2);
                triangles.Add(finalVerts.Count - 1);
            }
        }

        public Mesh CreateConeMesh(
            string name,
            int sides,
            Vector3 apex,
            Quaternion rotation,
            float baseRadius,
            float height)
        {
            Vector3[] baseVerts = GetBasePoints(sides, baseRadius);
            Vector3[] coneVerts = BuildConeVertices(baseVerts, height);

            var verts = new List<Vector3>();
            var tris = new List<int>();
            ConstructCone(coneVerts, verts, tris);

            for (int i = 0; i < verts.Count; i++)
            {
                verts[i] = (apex - coneVerts[0]) + rotation * (verts[i] - coneVerts[0]);
            }

            Mesh mesh = new Mesh
            {
                name = name
            };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();

            return mesh;
        }

        #endregion VARIABLES
    }
}