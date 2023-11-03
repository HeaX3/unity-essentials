using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PrefabSwitcher
{
    // UnityEditor gives us editor-specific access.

    /// <summary>Performs manual iteration to swap out one game object for another.</summary>
    public class PrefabSwitcher : MonoBehaviour
    {
        /// <summary>The new object to instantiate in place of the old object.</summary>
        public GameObject newPrefab;

        /// <summary>The old objects, intended to be swapped out for iterations of 
        /// the new object.</summary>
        public GameObject[] oldGameObjects;

        /// <summary>The string tag to use when replacing objects by tag.</summary>
        public string searchByTag;

        /// <summary>The mesh to use when replacing objects by mesh.</summary>
        public Mesh searchByMesh;

        /// <summary>Swaps all the game objects in oldGameObjects for 
        /// a new newPrefab.</summary>
        public void SwapAllByArray(GameObject[] gameObjects = null)
        {
            if (gameObjects == null) gameObjects = oldGameObjects;
            // Store a boolean to detect if we intend to swap this game object.
            var swappingSelf = false;

            // For each game object in the oldGameObjects array, 
            for (var i = 0; i < gameObjects.Length; i++)
            {
                // If the current game object is this game object, 
                if (gameObjects[i] == gameObject)
                {
                    // Enable the flag to swap this game object at the end, so we
                    // do not destroy it before the script an complete its task.
                    swappingSelf = true;
                }
                else
                {
                    // Else, we are not dealing with the game object local to this 
                    // script, so we can swap the prefabs, immediately. 
                    SwapPrefabs(gameObjects[i]);
                }
            }

            // If we have flagged the local game object to be swapped, 
            if (swappingSelf)
            {
                // Swap the local game object.
                SwapPrefabs(gameObject);
            }
        }

        /// <summary>Swaps all the game objects that use the tag <code>searchByTag</code>.
        /// If empty, we will use the tag of the local game object.</summary>
        public void SwapAllByTag()
        {
            // If searchByTag is null, 
            if (searchByTag == "")
            {
                // Set searchByTag to the tag of the local game object.
                searchByTag = gameObject.tag;
            }

            // Find all the game objects using the tag searchByTag, 
            // store them in our array, and proceed to swapping them.
            var oldGameObjects = GameObject.FindGameObjectsWithTag(searchByTag);
            SwapAllByArray(oldGameObjects);
        }

        /// <summary>Swaps all the game objects that use the mesh <code>searchByMesh</code>.</summary>
        public void SwapAllByMesh()
        {
            // If searchByTag is null, 
            if (searchByMesh == null)
            {
                Debug.LogWarning("Assign a mesh first");
                return;
            }

            // Find all the game objects using the tag searchByTag, 
            // store them in our array, and proceed to swapping them.
            var oldGameObjects = FindObjectsOfType<MeshFilter>().Where(f => f.sharedMesh == searchByMesh)
                .Select(f => f.gameObject).ToArray();
            SwapAllByArray(oldGameObjects);
        }

        /// <summary>Swaps the desired oldGameObject for a newPrefab.</summary>
        /// <param name="oldGameObject">The old game object.</param>
        void SwapPrefabs(GameObject oldGameObject)
        {
            // Determine the rotation and position values of the old game object.
            // Replace rotation with Quaternion.identity if you do not wish to keep rotation.
            var rotation = oldGameObject.transform.rotation;
            var position = oldGameObject.transform.position;

            // Instantiate the new game object at the old game objects position and rotation.
            var newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab, oldGameObject.scene);
            var newTransform = newGameObject.transform;
            newTransform.position = position;
            newTransform.rotation = rotation;

            // If the old game object has a valid parent transform,
            // (You can remove this entire if statement if you do not wish to ensure your
            // new game object does not keep the parent of the old game object.
            if (oldGameObject.transform.parent != null)
            {
                // Set the new game object parent as the old game objects parent.
                newTransform.SetParent(oldGameObject.transform.parent);
            }

            // Destroy the old game object, immediately, so it takes effect in the editor.
            DestroyImmediate(oldGameObject);
        }
    }

    /// <summary>Custom Editor for our PrefabSwitch script, to allow us to perform actions
    /// from the editor.</summary>
    [CustomEditor(typeof(PrefabSwitcher))]
    public class PrefabSwitcherEditor : Editor
    {
        /// <summary>Calls on drawing the GUI for the inspector.</summary>
        public override void OnInspectorGUI()
        {
            // Draw the default inspector.
            DrawDefaultInspector();

            // Grab a reference to the target script, so we can identify it as a 
            // PrefabSwitch, instead of a simple Object.
            var prefabSwitch = (PrefabSwitcher)target;

            // Create a Button for "Swap By Tag",
            if (GUILayout.Button("Swap By Tag"))
            {
                // if it is clicked, call the SwapAllByTag method from prefabSwitch.
                prefabSwitch.SwapAllByTag();
            }

            // Create a Button for "Swap By Array", 
            if (GUILayout.Button("Swap By Array"))
            {
                // if it is clicked, call the SwapAllByArray method from prefabSwitch.
                prefabSwitch.SwapAllByArray();
            }

            // Create a Button for "Swap By Array", 
            if (GUILayout.Button("Swap By Mesh"))
            {
                // if it is clicked, call the SwapAllByArray method from prefabSwitch.
                prefabSwitch.SwapAllByMesh();
            }
        }
    }
}