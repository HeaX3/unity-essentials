using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essentials.Meshes
{
    public static class MeshExtensions
    {
        private static readonly List<Vector3> vertices = new();
        private static readonly List<Vector3> normals = new();
        private static readonly List<Vector2> uvs = new();
        private static readonly List<Color> colors = new();
        private static readonly Dictionary<int, List<int>> combinedIndices = new();

        private static readonly List<int[]> indices = new();
        private static readonly List<MeshTopology> topologies = new();
        private static readonly List<Material> materials = new();
        
        public static ICombinableMesh Merge(this Mesh self, params Mesh[] meshes)
        {
            var unityMeshes = new ICombinableMesh[meshes.Length];
            for (var i = 0; i < meshes.Length; i++)
            {
                unityMeshes[i] = new UnityMesh(meshes[i]);
            }

            return self.Merge(unityMeshes);
        }

        public static ICombinableMesh Append(this Mesh self, params Mesh[] meshes)
        {
            var unityMeshes = new ICombinableMesh[meshes.Length];
            for (var i = 0; i < meshes.Length; i++)
            {
                unityMeshes[i] = new UnityMesh(meshes[i]);
            }

            return self.Append(unityMeshes);
        }

        public static ICombinableMesh Merge(this Mesh self, params ICombinableMesh[] meshes)
        {
            return (new UnityMesh(self)).Merge(meshes);
        }

        public static ICombinableMesh Append(this Mesh self, params ICombinableMesh[] meshes)
        {
            return (new UnityMesh(self)).Append(meshes);
        }

        public static ICombinableMesh Merge(this ICombinableMesh self, params ICombinableMesh[] meshes)
        {
            vertices.Clear();
            normals.Clear();
            uvs.Clear();
            colors.Clear();
            foreach (var list in combinedIndices.Values) list.Clear();

            var submeshes = new ICombinableMesh[meshes.Length + 1];
            submeshes[0] = self;
            for (var i = 0; i < meshes.Length; i++) submeshes[i + 1] = meshes[i];

            if (self.subMeshCount > 0 && self.GetTopology(0) != MeshTopology.Triangles)
            {
                self.triangles = self.triangles;
            }

            var materials = submeshes
                .Where(m => m.Materials != null)
                .SelectMany(m => m.Materials)
                .Where(m => m)
                .Distinct()
                .ToArray();
            var materialsMap = materials
                .Select((m, index) => new KeyValuePair<Material, int>(m, index))
                .ToDictionary(e => e.Key, e => e.Value);
            self.subMeshCount = materials.Length;

            foreach (var submesh in submeshes)
            {
                var submeshVertices = submesh.vertices;
                if (submeshVertices.Length == 0) continue;
                var submeshColors = submesh.colors;
                var submeshNormals = submesh.normals;
                var submeshUVs = submesh.uv;
                var start = vertices.Count;
                vertices.AddRange(submeshVertices);
                normals.AddRange(submeshNormals.Length == submeshVertices.Length
                    ? submeshNormals
                    : new Vector3[submeshVertices.Length]);
                uvs.AddRange(submeshUVs.Length == submeshVertices.Length
                    ? submeshUVs
                    : new Vector2[submeshVertices.Length]);
                colors.AddRange(submeshColors.Length == submeshVertices.Length
                    ? submeshColors
                    : MeshUtil.BuildColors(Color.white, submeshVertices.Length));

                for (var localIndex = 0; localIndex < submesh.Materials.Length; localIndex++)
                {
                    var material = submesh.Materials[localIndex];
                    if (!material) continue;
                    var submeshIndex = materialsMap[material];
                    if (!combinedIndices.TryGetValue(submeshIndex, out var list))
                    {
                        list = new List<int>();
                    }

                    list.AddRange(submesh.GetIndices(localIndex).Select(indice => start + indice));
                    combinedIndices[submeshIndex] = list;
                }
            }

            self.vertices = vertices.ToArray();
            self.normals = normals.ToArray();
            self.uv = uvs.ToArray();
            self.colors = colors.ToArray();
            for (var i = 0; i < self.subMeshCount; i++)
            {
                if (!combinedIndices.TryGetValue(i, out var list)) continue;
                var material = materials[i];
                self.SetIndices(list.ToArray(), MeshTopology.Triangles, material, i);
            }

            return self;
        }

        public static ICombinableMesh Append(this ICombinableMesh self, params ICombinableMesh[] meshes)
        {
            var submeshes = new ICombinableMesh[meshes.Length + 1];
            submeshes[0] = self;
            for (var i = 0; i < meshes.Length; i++) submeshes[i + 1] = meshes[i];

            vertices.Clear();
            uvs.Clear();
            colors.Clear();
            indices.Clear();
            topologies.Clear();
            materials.Clear();

            foreach (var submesh in submeshes)
            {
                var submeshVertices = submesh.vertices;
                if (submeshVertices.Length == 0) continue;
                var submeshColors = submesh.colors;
                var submeshUVs = submesh.uv;
                var start = vertices.Count;
                vertices.AddRange(submeshVertices);
                uvs.AddRange(submeshUVs.Length == submeshVertices.Length
                    ? submeshUVs
                    : new Vector2[submeshVertices.Length]);
                colors.AddRange(submeshColors.Length == submeshVertices.Length
                    ? submeshColors
                    : MeshUtil.BuildColors(Color.white, submeshVertices.Length));

                for (var i = 0; i < submesh.subMeshCount; i++)
                {
                    var submeshIndices = submesh.GetIndices(i).Select(indice => start + indice).ToArray();
                    indices.Add(submeshIndices);
                    topologies.Add(submesh.GetTopology(i));
                    materials.Add(submesh.GetMaterial(i));
                }
            }

            self.Clear();
            self.subMeshCount = indices.Count;
            self.vertices = vertices.ToArray();
            self.uv = uvs.ToArray();
            self.colors = colors.ToArray();

            for (var i = 0; i < indices.Count; i++)
            {
                var indicesList = indices[i];
                self.SetIndices(indicesList.ToArray(), topologies[i], materials[i], i);
            }

            return self;
        }

        public static Bounds CalculateBoundaries(this Mesh mesh)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var vertex in mesh.vertices)
            {
                min = new Vector3(Mathf.Min(min.x, vertex.x), Mathf.Min(min.y, vertex.y), Mathf.Min(min.z, vertex.z));
                max = new Vector3(Mathf.Max(max.x, vertex.x), Mathf.Max(max.y, vertex.y), Mathf.Max(max.z, vertex.z));
            }

            var size = max - min;
            var extents = size / 2;
            var center = min + extents;
            return new Bounds(center, size);
        }
    }
}