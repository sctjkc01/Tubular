using UnityEngine;
using System.Collections;

public class PathChunkExit : PathNode {
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);

        if(next != null) {
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
        Gizmos.color = Color.white;
    }
}
