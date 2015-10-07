using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour {
    /// <summary>
    /// List of all object bundles that will be chosen from to spawn.
    /// </summary>
    public SpawnBundle[] bundles;

    /// <summary>
    /// Chance per second that somsthing will spawn.
    /// </summary>
    [Tooltip("Chance per second that something will spawn, out of 1")]
    [Range(0f, 1f)]
    public float SpawnChance = 0.01f;

    private float TimeSinceLastCheck = 0f;

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
        TimeSinceLastCheck += Time.deltaTime;

        if(TimeSinceLastCheck > 1.0f) {
            while (TimeSinceLastCheck > 1.0f) {
                if (Random.value < SpawnChance) {
                    SpawnBundle bundleToUse = bundles[Random.Range(0, bundles.Length)];
                    GameObject newGO = GameObject.Instantiate<GameObject>(bundleToUse.prefab);
                    newGO.transform.SetParent(transform);
                    newGO.transform.localPosition = new Vector3(Random.Range(bundleToUse.maxDistFromCenter * -1f, bundleToUse.maxDistFromCenter), 0, 0);
                }

                TimeSinceLastCheck -= 1.0f;
            }
        }
    }
}
