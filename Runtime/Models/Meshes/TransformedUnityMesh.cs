using System;
using System.Linq;
using UnityEngine;

namespace Essentials.Meshes
{
    public class TransformedUnityMesh : ICombinableMesh
    {
        private readonly Mesh _mesh;
        private Matrix4x4 _matrix;
        private Material[] _materials;

        public string name
        {
            get => _mesh.name;
            set => throw new NotSupportedException();
        }

        public Vector3[] vertices
        {
            get => _mesh.vertices.Select(v => _matrix.MultiplyPoint3x4(v)).ToArray();
            set => throw new NotSupportedException();
        }

        public Vector3[] normals
        {
            get => _mesh.normals.Select(n => _matrix.MultiplyVector(n)).ToArray();
            set => throw new NotSupportedException();
        }

        public Vector2[] uv
        {
            get => _mesh.uv;
            set => throw new NotSupportedException();
        }

        public int[] triangles
        {
            get => _mesh.triangles;
            set => throw new NotSupportedException();
        }

        public Color[] colors
        {
            get => _mesh.colors;
            set => throw new NotSupportedException();
        }

        public int subMeshCount
        {
            get => _mesh.subMeshCount;
            set => throw new NotSupportedException();
        }

        public Material[] Materials
        {
            get => _materials;
            set => _materials = value;
        }

        public TransformedUnityMesh(Mesh mesh, Matrix4x4 matrix, Material[] materials = null)
        {
            _mesh = mesh;
            _matrix = matrix;
            _materials = materials != null ? materials : new Material[mesh.subMeshCount];
        }

        public void SetIndices(int[] indices, MeshTopology topology, Material material, int submesh)
        {
            throw new NotSupportedException();
        }

        public void SetIndices(int[] indices, MeshTopology topology)
        {
            throw new NotSupportedException();
        }

        public void SetMaterial(int submesh, Material material)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
        }

        public Mesh Handle => _mesh;
    }
}