using UnityEngine;

public class ShowTransformGizmos : MonoBehaviour
{
    [Tooltip("How big this gizmo shows up is in relation to its parent.")]
    public float GizmoSizeScale = 0.5f;

    [Tooltip("Modify the color of the axes. White: make them brighter than the parent; black: make them darker; grey: leave them the same. Colors affect certain axes more than others.")]
    public Color GizmoColorScale = new Color(0.5f, 0.5f, 0.5f);

    float Size {
        get {
            var parent = transform.parent;
            if (!transform.parent) { return GizmoSizeScale; }

            var parentGizmo = parent.GetComponent<ShowTransformGizmos>();
            if (!parentGizmo) { return GizmoSizeScale; }

            return parentGizmo.Size * GizmoSizeScale;
        }
    }

    Color ColorScale {
        get {
            var parent = transform.parent;
            if (!transform.parent) { return GizmoColorScale; }

            var parentGizmo = parent.GetComponent<ShowTransformGizmos>();
            if (!parentGizmo) { return GizmoColorScale; }

            return parentGizmo.ColorScale * 2 * GizmoColorScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Compute the color scale, so we can keep track which set of lines is which.
        var colorScale = this.ColorScale;
        var size = this.Size;

        // Draw a black line to the parent.
        var parent = transform.parent;
        if (parent) {
            Gizmos.color = colorScale * Color.white;
            Gizmos.DrawLine(transform.position, transform.parent.position);
        }

        // Red line forward.
        Gizmos.color = colorScale * Color.red;
        var direction = transform.TransformDirection(Vector3.forward) * size;
        Gizmos.DrawRay(transform.position, direction);

        // Green line up.
        Gizmos.color = colorScale * Color.green;
        direction = transform.TransformDirection(Vector3.up) * size;
        Gizmos.DrawRay(transform.position, direction);

        // Blue line to the right.
        Gizmos.color = colorScale * Color.blue;
        direction = transform.TransformDirection(Vector3.right) * size;
        Gizmos.DrawRay(transform.position, direction);
    }
}
