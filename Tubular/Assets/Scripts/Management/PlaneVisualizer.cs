using UnityEngine;
using System.Collections;

[AddComponentMenu("Tubular Scripts/In-Editor/Plane Visualizer")]
public class PlaneVisualizer : MonoBehaviour {
    public Vector2 PlaneSize;

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, PlaneSize.y * 0.5f, 0f), new Vector3(PlaneSize.x, PlaneSize.y, 0.001f));
    }
}
