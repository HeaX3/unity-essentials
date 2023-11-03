using UnityEngine;

namespace Essentials.Meshes
{
    public class UnityMesh : ICombinableMesh
    {
        private readonly Mesh _mesh;
        private Material[] _materials;

        public string name
        {
            get => _mesh.name;
            set => _mesh.name = value;
        }

        public Vector3[] vertices
        {
            get => _mesh.vertices;
            set => _mesh.vertices = value;
        }

        public Vector3[] normals
        {
            get => _mesh.normals;
            set => _mesh.normals = value;
        }

        public Vector2[] uv
        {
            get => _mesh.uv;
            set => _mesh.uv = value;
        }

        public int[] triangles
        {
            get => _mesh.triangles;
            set => _mesh.triangles = value;
        }

        public Color[] colors
        {
            get => _mesh.colors;
            set => _mesh.colors = value;
        }

        public int subMeshCount
        {
            get => _mesh.subMeshCount;
            set
            {
                _mesh.subMeshCount = value;
                var oldMaterials = _materials;
                _materials = new Material[value];
                if (oldMaterials == null || oldMaterials.Length <= 0) return;
                for (var i = 0; i < oldMaterials.Length && i < _materials.Length; i++)
                {
                    _materials[i] = oldMaterials[i];
                }
            }
        }

        public Material[] Materials
        {
            get => _materials;
            set => _materials = value;
        }

        public UnityMesh(Mesh mesh, Material[] materials = null)
        {
            _mesh = mesh;
            _materials = materials != null ? materials : new Material[mesh.subMeshCount];
        }

        public void SetIndices(int[] indices, MeshTopology topology, Material material, int submesh)
        {
            _mesh.SetIndices(indices, topology, submesh);
            _materials[submesh] = material;
        }

        public void SetIndices(int[] indices, MeshTopology topology)
        {
            _mesh.SetIndices(indices, topology, 0);
        }

        public void SetMaterial(int submesh, Material material)
        {
            _materials[submesh] = material;
        }

        public int[] GetIndices(int submesh)
        {
            return _mesh.GetIndices(submesh);
        }

        public Material GetMaterial(int submesh)
        {
            return _materials != null && _materials.Length > submesh && submesh >= 0 ? _materials[submesh] : null;
        }

        public void Clear()
        {
            _mesh.Clear();
            _materials = new Material[1];
        }

        public MeshTopology GetTopology(int submesh)
        {
            return _mesh.GetTopology(submesh);
        }

        public void RecalculateNormals()
        {
            _mesh.RecalculateNormals();
        }

        public Mesh Handle => _mesh;
    }
}