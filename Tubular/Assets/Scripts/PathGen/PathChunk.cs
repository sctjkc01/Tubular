using UnityEngine;
using System.Collections;

public class PathChunk : MonoBehaviour {
    public bool passed = false, destroyStarted = false;

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

    void Update() {
        if(passed ^ destroyStarted) {
            Destroy(gameObject, 2f);
            destroyStarted = true;
        }
    }

}
