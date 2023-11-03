using UnityEngine;

namespace Essentials
{
    public static class MeshExtensions
    {
        public static Bounds CalculateBoundaries(this Mesh mesh)
        {
            var min = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
            var max = new Vector3(float.MinValue,float.MinValue,float.MinValue);
            
            foreach (var vertex in mesh.vertices)
            {
                min = new Vector3(Mathf.Min(min.x,vertex.x), Mathf.Min(min.y,vertex.y), Mathf.Min(min.z,vertex.z));
                max = new Vector3(Mathf.Max(max.x,vertex.x), Mathf.Max(max.y,vertex.y), Mathf.Max(max.z,vertex.z));
            }

            var size = max - min;
            var extents = size / 2;
            var center = min + extents;
            return new Bounds(center,size);
        }
    }
}