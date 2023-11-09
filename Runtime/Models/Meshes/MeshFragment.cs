using System.Linq;
using UnityEngine;

namespace Essentials.Meshes
{
    public class MeshFragment : ICombinableMesh
    {
        public Vector3[] vertices { get; set; }
        public Vector3[] normals { get; set; }
        public Vector2[] uv { get; set; }
        public Color[] colors { get; set; }

        public Bounds bounds
        {
            get
            {
                var min = new Vector3();
                var max = new Vector3();
                foreach (var vertex in vertices)
                {
                    min.x = Mathf.Min(min.x, vertex.x);
                    min.y = Mathf.Min(min.y, vertex.y);
                    min.z = Mathf.Min(min.z, vertex.z);
                    max.x = Mathf.Max(max.x, vertex.x);
                    max.y = Mathf.Max(max.y, vertex.y);
                    max.z = Mathf.Max(max.z, vertex.z);
                }

                var center = (min + max) / 2;
                var size = max - min;
                return new Bounds(center, size);
            }
        }

        public int[] triangles
        {
            get { return _submeshes.SelectMany(submesh => submesh.triangles).ToArray(); }
            set
            {
                if (subMeshCount < 1) subMeshCount = 1;
                _submeshes[0].triangles = value;
            }
        }

        public int subMeshCount
        {
            get { return _submeshes.Length; }
            set
            {
                var previous = _submeshes;
                _submeshes = new SubmeshPart[value];
                for (var i = 0; i < _submeshes.Length; i++)
                {
                    _submeshes[i] = previous.Length > i ? previous[i] : new SubmeshPart();
                }
            }
        }

        public Material[] Materials
        {
            get { return _submeshes.Select(submesh => submesh.material).ToArray(); }
            set
            {
                for (var i = 0; i < _submeshes.Length && i < value.Length; i++)
                {
                    _submeshes[i].material = value[i];
                }
            }
        }

        private SubmeshPart[] _submeshes = null;

        public MeshFragment(Mesh mesh) : this(new UnityMesh(mesh))
        {
        }

        public MeshFragment(ICombinableMesh mesh) : this(mesh.subMeshCount)
        {
            vertices = (Vector3[])mesh.vertices.Clone();
            normals = (Vector3[])mesh.normals.Clone();
            uv = (Vector2[])mesh.uv.Clone();
            colors = (Color[])mesh.colors.Clone();
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                _submeshes[i].indices = (int[])mesh.GetIndices(i).Clone();
                _submeshes[i].topology = mesh.GetTopology(i);
                _submeshes[i].material = mesh.GetMaterial(i);
            }
        }

        public MeshFragment() : this(1)
        {
        }

        public MeshFragment(int submeshCount)
        {
            Clear(submeshCount);
        }

        public void Clear()
        {
            Clear(1);
        }

        private void Clear(int submeshes)
        {
            vertices = new Vector3[0];
            normals = new Vector3[0];
            uv = new Vector2[0];
            colors = new Color[0];
            _submeshes = new SubmeshPart[submeshes];
            for (var i = 0; i < submeshes; i++)
            {
                _submeshes[i] = new SubmeshPart();
            }
        }

        public void SetIndices(int[] indices, MeshTopology topology, Material material, int submesh)
        {
            _submeshes[submesh].indices = indices;
            _submeshes[submesh].topology = topology;
            _submeshes[submesh].material = material;
        }

        public void SetIndices(int[] indices, MeshTopology topology)
        {
            _submeshes[0].indices = indices;
            _submeshes[0].topology = topology;
        }

        public void SetMaterial(int submesh, Material material)
        {
            _submeshes[submesh].material = material;
        }

        public int[] GetIndices(int submesh)
        {
            return _submeshes[submesh].indices;
        }

        public MeshTopology GetTopology(int submesh)
        {
            return _submeshes[submesh].topology;
        }

        public Material GetMaterial(int submesh)
        {
            return _submeshes[submesh].material;
        }

        public MeshFragment Clone()
        {
            return new MeshFragment(this);
        }
    }
}