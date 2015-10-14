using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    /// <summary>
    /// Value read by everything that travels.  Value is in Unity Units per Second.
    /// </summary>
    [Tooltip("In Unity Units per Second.  Read by everything that travels.")]
    public float GameTravelSpeed = 5.0f;

    /// <summary>
    /// Are we playing a game right now?
    /// </summary>
    public bool live = false;

    public static GameManager inst;

    void Awake() {
        if (inst == null) inst = this;
        else Destroy(this.gameObject);
    }
}
