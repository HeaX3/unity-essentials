using UnityEngine;

namespace Essentials.Meshes
{
    public interface ICombinableMesh
    {
        Vector3[] vertices { get; set; }
        Vector3[] normals { get; set; }
        Vector2[] uv { get; set; }
        int[] triangles { get; set; }
        Color[] colors { get; set; }
        int subMeshCount { get; set; }
        MeshTopology GetTopology(int submesh);
        Material[] Materials { get; set; }

        void SetIndices(int[] indices, MeshTopology topology, Material material, int submesh);
        void SetMaterial(int submesh, Material material);
        int[] GetIndices(int submesh);
        Material GetMaterial(int submesh);
        void Clear();
    }
}