using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essentials
{
    public static class GameObjectExtensions
    {
        private static readonly Dictionary<GameObject, IEnumerable<GameObject>> ForcedObjectsMap =
            new Dictionary<GameObject, IEnumerable<GameObject>>();

        /// <summary>
        /// Forces a gameObject and all its parents into active state to circumvent stupid Unity rules about stuff
        /// breaking when a gameObject is disabled
        /// </summary>
        public static void ForceActiveState(this GameObject gameObject)
        {
            var inactiveObjects = GetHierarchy(gameObject).Where(p => !p.activeSelf).ToList();
            foreach (var inactiveObject in inactiveObjects) inactiveObject.SetActive(true);
            ForcedObjectsMap[gameObject] = inactiveObjects;
        }

        /// <summary>
        /// Releases a previously forced active state and disables all gameObjects that were disabled beforce the
        /// active state was enforced
        /// </summary>
        public static void ReleaseActiveState(this GameObject gameObject)
        {
            if (!ForcedObjectsMap.TryGetValue(gameObject, out var inactiveObjects)) return;
            foreach (var inactiveObject in inactiveObjects) inactiveObject.SetActive(false);
            ForcedObjectsMap.Remove(gameObject);
        }

        private static IEnumerable<GameObject> GetHierarchy(GameObject gameObject)
        {
            yield return gameObject;
            var parent = gameObject.transform.parent;
            if (!parent) yield break;
            foreach (var parentParent in GetHierarchy(parent.gameObject))
            {
                if (!parentParent) continue;
                yield return parentParent;
            }
        }
    }
}