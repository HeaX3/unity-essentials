// NavigationHelper - Allows you to create Unity Nav Meshes using Box Colliders - 2014-01-09
// released under MIT License
// http://www.opensource.org/licenses/mit-license.php
//
//@author		Devin Reimer - Owlchemy Labs
//@website 		http://blog.almostlogical.com
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

/*
 * Usage: To use set COLLIDER_LAYER const (below) to whatever layer you wish for the Box Colliders to be used from. This allows you to use some box colliders and
 *        not others. If you don't change it (-1) it will use all layers.
 */

//Note: This class uses UnityEditorInternal which is an undocumented internal feature, it uses it to make this script less error prone

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Essentials
{
    public class NavigationHelper : EditorWindow
    {
        private const string
            COLLIDER_LAYER = "Ground"; //edit this valve to set which layer a box collider needs to be on to be read

        private static readonly Dictionary<PrimitiveType, GameObject> primitivePrefabs = new();

        [MenuItem("Window/Navigation Helper")]
        static void Init()
        {
            GetWindow(typeof(NavigationHelper));
        }

        void OnGUI()
        {
            GUILayout.Label(
                "Build a NavMesh baking mold using Colliders on the layer '" + COLLIDER_LAYER +
                "'. After molding, disable all other MeshRenderers leaving only the mold, then bake the NavMesh.",
                EditorStyles.boldLabel);

            if (GUILayout.Button("Build Nav Mesh Mold!"))
            {
                BakeColliders();
            }
        }


        public static GameObject BakeColliders()
        {
            CleanUpOldNavMeshItems();
            var allColliders = FindObjectsOfType<Collider>();
            var layerMask = LayerMask.GetMask(COLLIDER_LAYER);

            var mold = new GameObject("NavMesh Mold");
            var transform = mold.transform;

            foreach (var c in allColliders)
            {
                if (!c.enabled || !c.gameObject.activeInHierarchy || layerMask != (layerMask | (1 << c.gameObject.layer))) continue;

                var fragment = c switch
                {
                    BoxCollider bc => BakeBoxCollider(bc),
                    SphereCollider sc => BakeSphereCollider(sc),
                    CapsuleCollider cc => BakeCapsuleCollider(cc),
                    MeshCollider mc => BakeMeshCollider(mc),
                    _ => null
                };

                if (!fragment || fragment == null) continue;

                fragment.SetParent(transform, false);
            }
            CleanUpOldNavMeshItems();
            return mold;
        }

        private static Transform BakeBoxCollider(BoxCollider collider)
        {
            var colliderTransform = collider.transform;
            var wrapperTransform = CreateClone(colliderTransform);
            var transform = BakePrimitive(PrimitiveType.Cube);
            transform.localPosition = collider.center;
            transform.localScale = collider.size;
            transform.SetParent(wrapperTransform, false);
            return wrapperTransform;
        }

        private static Transform BakeSphereCollider(SphereCollider collider)
        {
            var colliderTransform = collider.transform;
            var wrapperTransform = CreateClone(colliderTransform);
            var transform = BakePrimitive(PrimitiveType.Sphere);
            transform.localPosition = collider.center;
            transform.localScale = Vector3.one * (collider.radius * 2);
            transform.SetParent(wrapperTransform, false);
            return wrapperTransform;
        }

        private static Transform BakeCapsuleCollider(CapsuleCollider collider)
        {
            var colliderTransform = collider.transform;
            var wrapperTransform = CreateClone(colliderTransform);
            var radius = collider.radius;
            var upperSphere = BakePrimitive(PrimitiveType.Sphere);
            upperSphere.localPosition = collider.center + Vector3.up * (collider.height / 2 - radius);
            upperSphere.localScale = Vector3.one * (radius * 2);
            upperSphere.SetParent(wrapperTransform, false);
            var cylinder = BakePrimitive(PrimitiveType.Cylinder);
            cylinder.localPosition = collider.center;
            cylinder.localScale = new Vector3(radius * 2, (collider.height - radius * 2) / 2, radius * 2);
            cylinder.SetParent(wrapperTransform, false);
            var lowerSphere = BakePrimitive(PrimitiveType.Sphere);
            lowerSphere.localPosition = collider.center - Vector3.up * (collider.height / 2 - radius);
            lowerSphere.localScale = Vector3.one * (radius * 2);
            lowerSphere.SetParent(wrapperTransform, false);
            return wrapperTransform;
        }

        private static Transform BakePrimitive(PrimitiveType type)
        {
            if (!primitivePrefabs.ContainsKey(type))
            {
                var navMeshPrefab = GameObject.CreatePrimitive(type);
                DestroyImmediate(navMeshPrefab.GetComponent<Collider>());
                GameObjectUtility.SetStaticEditorFlags(navMeshPrefab, StaticEditorFlags.NavigationStatic);
                primitivePrefabs.Add(type, navMeshPrefab);
            }

            return Instantiate(primitivePrefabs[type]).transform;
        }

        private static Transform BakeMeshCollider(MeshCollider collider)
        {
            var wrapperTransform = CreateClone(collider.transform);
            var gameObject = new GameObject("MeshCollider");
            GameObjectUtility.SetStaticEditorFlags(gameObject, StaticEditorFlags.NavigationStatic);
            var transform = gameObject.transform;
            var tempMeshFilter = gameObject.AddComponent<MeshFilter>();
            tempMeshFilter.sharedMesh = collider.sharedMesh;
            gameObject.AddComponent<MeshRenderer>();
            transform.SetParent(wrapperTransform, false);
            return wrapperTransform;
        }

        private static Transform CreateClone(Transform reference)
        {
            var wrapper = new GameObject();
            var wrapperTransform = wrapper.transform;
            wrapperTransform.position = reference.position;
            wrapperTransform.rotation = reference.rotation;
            wrapperTransform.localScale = reference.lossyScale;
            return wrapperTransform;
        }

        private static void CleanUpOldNavMeshItems()
        {
            foreach (var prefab in primitivePrefabs.Values)
            {
                try
                {
                    DestroyImmediate(prefab);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            primitivePrefabs.Clear();
        }
    }
}