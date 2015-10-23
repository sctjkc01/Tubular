using UnityEngine;
using System.Collections;

public class PathChunkEntry : PathNode {
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.25f);

        if(next != null) {
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
        Gizmos.color = Color.white;
    }
}
