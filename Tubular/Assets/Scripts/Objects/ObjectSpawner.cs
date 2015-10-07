using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour {
    /// <summary>
    /// List of all object bundles that will be chosen from to spawn.
    /// </summary>
    public SpawnBundle[] bundles;

    /// <summary>
    /// A bundle of things that can be spawned.
    /// </summary>
    [System.Serializable]
    public struct SpawnBundle {
        /// <summary>
        /// The bundle to spawn.
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// How far from the center can this bundle be?
        /// </summary>
        public float maxDistFromCenter;
    }

    // Update is called once per frame
    void Update() {

    }
}
