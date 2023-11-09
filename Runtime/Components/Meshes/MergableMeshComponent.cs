using UnityEngine;

namespace Essentials.Meshes
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MergableMeshComponent : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private MeshFilter _meshFilter;
        [SerializeField] [HideInInspector] private MeshRenderer _meshRenderer;

        private MeshFilter meshFilter => _meshFilter;
        private MeshRenderer meshRenderer => _meshRenderer;

        private Transform _transform;
        private ICombinableMesh _mesh;

        public ICombinableMesh mesh
        {
            get
            {
                if (_mesh == null) UpdateCombineMesh();
                return _mesh;
            }
        }

        public new Transform transform
        {
            get
            {
                if (!_transform) _transform = base.transform;
                return _transform;
            }
        }

        public void UpdateCombineMesh()
        {
            var mesh = meshFilter.sharedMesh;
            if (!mesh)
            {
                _mesh = null;
                return;
            }

            _mesh = CalculateCurrentMesh();
        }

        public ICombinableMesh CalculateCurrentMesh()
        {
            var mesh = meshFilter.sharedMesh;
            if (!mesh)
            {
                return null;
            }

            return new TransformedUnityMesh(
                mesh,
                transform.localToWorldMatrix,
                meshRenderer.sharedMaterials
            );
        }

        private void OnValidate()
        {
            if (!_meshFilter || _meshFilter.gameObject != gameObject) _meshFilter = GetComponent<MeshFilter>();
            if (!_meshRenderer || _meshRenderer.gameObject != gameObject) _meshRenderer = GetComponent<MeshRenderer>();
        }
    }
}