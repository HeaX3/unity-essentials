using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Essentials
{
    public static class TransformExtensions
    {
        private static readonly Dictionary<Transform, IEnumerable<KeyValuePair<Transform, TransformState>>>
            ForcedTransformsMap = new Dictionary<Transform, IEnumerable<KeyValuePair<Transform, TransformState>>>();

        /// <summary>
        /// Forces a gameObject and all its parents into active state to circumvent stupid Unity rules about stuff
        /// breaking when a gameObject is disabled
        /// </summary>
        public static void ForceUniformState(this Transform transform)
        {
            var hierarchy = GetHierarchy(transform)
                .Select(t => new KeyValuePair<Transform, TransformState>(t, TransformState.Get(t)))
                .ToList();
            foreach (var e in hierarchy) e.Key.Reset();
            ForcedTransformsMap[transform] = hierarchy;
        }

        /// <summary>
        /// Releases a previously forced active state and disables all gameObjects that were disabled beforce the
        /// active state was enforced
        /// </summary>
        public static void ReleaseUniformState(this Transform transform)
        {
            if (!ForcedTransformsMap.TryGetValue(transform, out var hierarchy)) return;
            foreach (var e in hierarchy) e.Key.Apply(e.Value);
            ForcedTransformsMap.Remove(transform);
        }

        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Copy(this Transform transform, Transform other)
        {
            transform.position = other.position;
            transform.rotation = other.rotation;
            transform.localScale = other.localScale;
        }

        public static void Interpolate(this Transform transform, Transform a, Transform b, float t)
        {
            transform.position = Vector3.Lerp(a.position, b.position, t);
            transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
            transform.localScale = Vector3.one;

            var worldScale = Vector3.Lerp(a.lossyScale, b.lossyScale, t);
            var ownScale = transform.lossyScale;
            transform.localScale = new Vector3(worldScale.x / ownScale.x, worldScale.y / ownScale.y,
                worldScale.z / ownScale.z);
        }

        public static string GetPath(this Transform t)
        {
            var parent = t.parent;
            return parent && parent != null ? parent.GetPath() + "/" + t.name : t.name;
        }

        public static void DetachDestroyedChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject) child.parent = null;
                else child.DetachDestroyedChildren();
            }
        }

        public static IEnumerable<Transform> GetHierarchy(Transform transform)
        {
            yield return transform;
            var parent = transform.parent;
            if (!parent) yield break;
            foreach (var parentParent in GetHierarchy(parent))
            {
                if (!parentParent) continue;
                yield return parentParent;
            }
        }

        public static string HierarchyToString(this Transform transform)
        {
            return HierarchyToString(transform, 0);
        }

        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            return transform.Cast<Transform>();
        }

        public static IEnumerable<Transform> GetChildrenRecursive(this Transform transform)
        {
            return transform.Cast<Transform>().SelectMany(c => c.GetChildrenRecursive().Prepend(c));
        }

        private static string HierarchyToString(Transform transform, int depth)
        {
            return GetOffset(depth) + "- " + transform.name + "\n" + string.Join("",
                transform.GetChildren().Select(c => HierarchyToString(c, depth + 1)));
        }

        private static string GetOffset(int depth)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < depth; i++) builder.Append("  ");
            return builder.ToString();
        }

        private static void Apply(this Transform transform, TransformState state)
        {
            transform.localPosition = state.localPosition;
            transform.localRotation = state.localRotation;
            transform.localScale = state.localScale;
        }

        private struct TransformState
        {
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;

            public TransformState(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
                this.localPosition = localPosition;
                this.localRotation = localRotation;
                this.localScale = localScale;
            }

            public static TransformState Get(Transform transform)
            {
                return new TransformState(transform.localPosition, transform.localRotation,
                    transform.localScale);
            }
        }
    }
}