using UnityEngine;
using System.Collections;

public class PathChunkExit : PathNode {
    public override void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.color = Color.white;

        base.OnDrawGizmos();
    }
}
