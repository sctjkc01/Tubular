using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour {
    // Update is called once per frame
    void Update() {
        // Move object
        transform.position += new Vector3(0f, 0f, -1f) * GameManager.inst.GameTravelSpeed * Time.deltaTime;
        // Destroy object if it's past the players
        if (transform.position.z < -10f) Destroy(this.gameObject);
    }
}
