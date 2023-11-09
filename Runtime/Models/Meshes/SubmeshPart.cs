using System;
using UnityEngine;

namespace Essentials.Meshes
{
    public class SubmeshPart
    {
        public int[] indices { get; set; }
        public Material material { get; set; }

        public int[] triangles
        {
            get { return ConvertToTriangles(indices, topology); }
            set
            {
                indices = value;
                topology = MeshTopology.Triangles;
            }
        }

        public MeshTopology topology { get; set; }

        public SubmeshPart()
        {
            indices = new int[0];
            topology = MeshTopology.Triangles;
            material = null;
        }

        private int[] ConvertToTriangles(int[] indices, MeshTopology previousTopology)
        {
            switch (previousTopology)
            {
                case MeshTopology.Triangles: return indices;
                case MeshTopology.Quads: return ConvertQuadsToTriangles(indices);
                default: throw new NotImplementedException();
            }
        }

        private int[] ConvertQuadsToTriangles(int[] indices)
        {
            var result = new int[indices.Length / 2 * 3];
            for (var i = 0; i < indices.Length / 4; i++)
            {
                var a = indices[i * 4 + 0];
                var b = indices[i * 4 + 1];
                var c = indices[i * 4 + 2];
                var d = indices[i * 4 + 3];
                
                result[i * 6 + 0] = a;
                result[i * 6 + 1] = c;
                result[i * 6 + 2] = d;
                
                result[i * 6 + 3] = a;
                result[i * 6 + 4] = b;
                result[i * 6 + 5] = c;
            }
            return result;
        }
    }
}