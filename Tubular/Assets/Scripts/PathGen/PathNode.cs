using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {
    public PathNode next;

    public virtual void OnDrawGizmos() {
        if(next != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, next.transform.position);
            Gizmos.color = Color.white;
        }
    }
}
