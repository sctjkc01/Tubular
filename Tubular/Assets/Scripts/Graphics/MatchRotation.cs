using UnityEngine;
using System.Collections;

public class MatchRotation : MonoBehaviour {
    public Transform target;

    // Update is called once per frame
    void Update() {
        transform.rotation = target.rotation;
    }
}
