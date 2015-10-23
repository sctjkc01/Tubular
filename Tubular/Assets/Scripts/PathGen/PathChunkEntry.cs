using UnityEngine;
using System.Collections;

public class PathChunkEntry : PathNode {
    public override void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.color = Color.white;

        base.OnDrawGizmos();
    }
}
