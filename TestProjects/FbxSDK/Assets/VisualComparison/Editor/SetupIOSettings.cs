using UnityEditor;
using UnityEngine;

namespace FbxSharp
{
    static class FbxIOSettingsSetup
    {
        [MenuItem ("FbxSharp/Add IOSettings")]
        public static void AddIOSettings ()
        {
            // Add comparison gizmos to the entire hierarchy of anything that's
            // selected -- unless there's already a gizmo.
            var stack = new System.Collections.Generic.List<Transform> (Selection.transforms);
            while (stack.Count > 0) {
                var top = stack [stack.Count - 1];
                stack.RemoveAt (stack.Count - 1);
                if (!top.GetComponent<FbxSharp.IOSettings> ()) {
                    top.gameObject.AddComponent<FbxSharp.IOSettings> ();
                }
            }
        }

        [MenuItem ("FbxSharp/Remove IOSettings")]
        public static void RemoveIOSettings ()
        {
            // Remove the gizmos. This removes the first one only; if there happen
            // to be several for some reason, just run this several times.
            var stack = new System.Collections.Generic.List<Transform> (Selection.transforms);
            while (stack.Count > 0) {
                var top = stack [stack.Count - 1];
                stack.RemoveAt (stack.Count - 1);
                var topIOSettings = top.GetComponent<FbxSharp.IOSettings> ();
                if (topIOSettings) {
                    topIOSettings.enabled = false;
                    UnityEngine.GameObject.DestroyImmediate (topIOSettings);
                }
            }
        }
    }
}