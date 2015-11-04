using UnityEngine;
using System.Collections;

public class PathBuilder : MonoBehaviour {
    private PathChunk lastPipe;

    [System.Serializable]
    public struct ChunkOption {
        public GameObject option;
        public float weight;
        public float vertRotDelta;
        public bool horizRot;
    }

    private float totalWeight = 0.0f;

    private float totalRot = 0.0f;

    public ChunkOption Straight;
    public ChunkOption[] SpawnOptions;

    // Update is called once per frame
    void Start() {
        totalWeight = Straight.weight;
        foreach (ChunkOption alpha in SpawnOptions) {
            totalWeight += alpha.weight;
        }

        StartCoroutine(CreatePath());
    }

    public IEnumerator CreatePath() {
        for (byte i = 0; i < 255; i++) {
            GameObject newGO = null;
            if (i < 5) {
                newGO = GameObject.Instantiate<GameObject>(Straight.option); // Straight Option
            } else {
                while (newGO == null) {
                    float selection = Random.Range(0, totalWeight);
                    for (byte j = 0; j < SpawnOptions.Length + 1; j++) {
                        if(j == SpawnOptions.Length) {
                            newGO = GameObject.Instantiate<GameObject>(Straight.option);
                            break;
                        }
                        if (selection < SpawnOptions[j].weight) {
                            if ((totalRot < -10 && SpawnOptions[j].vertRotDelta < -10) || (totalRot > 10 && SpawnOptions[j].vertRotDelta > 10)) break;
                            if ((totalRot != 0) && SpawnOptions[j].horizRot) break;
                            totalRot += SpawnOptions[j].vertRotDelta;
                            newGO = GameObject.Instantiate<GameObject>(SpawnOptions[j].option);
                            break;
                        } else {
                            selection -= SpawnOptions[j].weight;
                        }
                    }
                }
            }

            newGO.transform.SetParent(transform, true);
            var newPipe = newGO.GetComponent<PathChunk>();
            if (lastPipe) {
                newGO.transform.position = lastPipe.EndPoint.transform.position;
                newGO.transform.rotation = lastPipe.EndPoint.transform.rotation;
                lastPipe.EndPoint.next = newPipe.StartPoint;
            } else {
                newGO.transform.position = transform.position;
                newGO.transform.rotation = Quaternion.identity;
            }
            lastPipe = newGO.GetComponent<PathChunk>();

            yield return null;
        }

    }
}
