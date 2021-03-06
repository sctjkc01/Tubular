﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour {
    /// <summary>
    /// List of all object bundles that will be chosen from to spawn.
    /// </summary>
    public SpawnBundle[] bundles;

    /// <summary>
    /// Chance per second that something will spawn.
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
    void Update()
    {
        if (!isLocalPlayer && GameManager.inst != null && GameManager.inst.live) {
            TimeSinceLastCheck += Time.deltaTime;
            
            while (TimeSinceLastCheck > 1.0f) {
                if (Random.value < SpawnChance)
                {
                    SpawnBundle bundleToUse = bundles[Random.Range(0, bundles.Length)];
                    GameObject newGO = GameObject.Instantiate<GameObject>(bundleToUse.prefab);
                    newGO.transform.position = transform.position + new Vector3(Random.Range(bundleToUse.maxDistFromCenter * -1f, bundleToUse.maxDistFromCenter), 0, 0);
                    newGO.transform.rotation = transform.rotation;
                    NetworkServer.Spawn(newGO);
                }

                TimeSinceLastCheck -= 1.0f;
            }
        }
    }
}
