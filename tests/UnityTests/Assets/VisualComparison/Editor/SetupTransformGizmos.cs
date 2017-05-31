using UnityEditor;
using UnityEngine;

static class VisualCompareSetup
{
    [MenuItem("FbxSharp/Add Comparison Gizmos")]
    public static void AddComparisonGizmos()
    {
        // Add comparison gizmos to the entire hierarchy of anything that's
        // selected -- unless there's already a gizmo.
        var stack = new System.Collections.Generic.List<Transform>(Selection.transforms);
        while(stack.Count > 0) {
            var top = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            if (!top.GetComponent<ShowTransformGizmos>()) {
                top.gameObject.AddComponent<ShowTransformGizmos>();
            }
            foreach(UnityEngine.Transform child in top) {
                stack.Add(child);
            }
        }
    }

    [MenuItem("FbxSharp/Remove Comparison Gizmos")]
    public static void RemoveComparisonGizmos()
    {
        // Remove the gizmos. This removes the first one only; if there happen
        // to be several for some reason, just run this several times.
        var stack = new System.Collections.Generic.List<Transform>(Selection.transforms);
        while(stack.Count > 0) {
            var top = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            var topGizmo = top.GetComponent<ShowTransformGizmos>();
            if (topGizmo) {
                topGizmo.enabled = false;
                UnityEngine.GameObject.DestroyImmediate(topGizmo);
            }
            foreach(UnityEngine.Transform child in top) {
                stack.Add(child);
            }
        }
    }
}
