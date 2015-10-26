using UnityEngine;
using System.Collections;

public class PathChunk : MonoBehaviour {
    public PathNode StartPoint {
        get {
            return transform.GetComponentInChildren<PathChunkEntry>();
        }
    }

    public PathNode EndPoint {
        get {
            return transform.GetComponentInChildren<PathChunkExit>();
        }
    }
}
