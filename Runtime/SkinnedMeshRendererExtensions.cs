using System;
using UnityEngine;

namespace Essentials
{
    public static class SkinnedMeshRendererExtensions
    {
        public static void Fill(this SkinnedMeshRenderer renderer, Transform root, Bounds wrappingBounds, MeshPosition position)
        {
            var bounds = CalculateBounds(renderer, position);
            var resize = wrappingBounds.Fit(bounds, out var offset);

            Vector3 localPosition;
            switch (position)
            {
                case MeshPosition.Center:
                    localPosition = new Vector3(offset.x * resize.x, offset.y * resize.y, offset.z * resize.z);
                    break;
                case MeshPosition.CenterBottom:
                    localPosition = new Vector3(offset.x * resize.x, 0, offset.z * resize.z);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            root.localPosition = localPosition;
            root.localRotation = Quaternion.identity;
            root.localScale = resize;
        }

        private static Bounds CalculateBounds(SkinnedMeshRenderer renderer, MeshPosition position)
        {
            if (renderer == null) return new Bounds();
            
            var mesh = renderer.sharedMesh;
            var bounds = mesh ? mesh.CalculateBoundaries() : new Bounds();

            Vector3 size;
            Vector3 center;
            switch (position)
            {
                case MeshPosition.Center:
                    size = bounds.size;
                    center = bounds.center;
                    break;
                case MeshPosition.CenterBottom:
                    size = new Vector3(bounds.size.x, bounds.max.y, bounds.size.z);
                    center = new Vector3(bounds.center.x, bounds.max.y / 2, bounds.center.z);
                    break;
                default: throw new InvalidOperationException();
            }

            return new Bounds(center, size);
        }
    }
}