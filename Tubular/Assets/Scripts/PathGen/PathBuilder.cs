using UnityEngine;
using System.Collections;


public class PathBuilder : MonoBehaviour {
    [Tooltip("How many chunks to keep in play at a time.")]
    public byte chunkCnt;
    private byte chunkCntTemp; //chunkCnt changes to start with smaller amount
    public static byte currentChunkCount; // How many chunks are in play right now
    private PathChunk lastPipe;

    private static System.Random rand;
    public static void SetSeed(int seed) { rand = new System.Random(seed); }

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
        chunkCntTemp = 30;
        StartCoroutine(CreatePath());

        SetSeed(0);
    }

    public IEnumerator CreatePath() {
        yield return new WaitForEndOfFrame();
        PathFollow[] followers = GameObject.FindObjectsOfType<PathFollow>();
        for (byte i = 0; i < chunkCntTemp; i++)
        {
            GameObject newGO = null;
            if (i < 5) {
                newGO = GameObject.Instantiate<GameObject>(Straight.option); // Straight Option
            } else {
                while (newGO == null)
                {
                    float selection = (float)rand.NextDouble()*totalWeight;//Random.Range(0, totalWeight);
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

            currentChunkCount++;

            if(i == 0) {
                for(byte j = 0; j < followers.Length; j++) {
                    followers[j].currNode = lastPipe.StartPoint; // Place the path followers on the start point.
                }
            }
            if(i == 20) {
                for(byte j = 0; j < followers.Length; j++) {
                    if(followers[j].gameObject.GetComponent<ObjectSpawner>() != null) {
                        followers[j].Travel(200f); // Push the obstacle spawner 200 units ahead instead
                    } else if(followers[j].gameObject.name == "Dead Area") {
                        followers[j].Travel(70f); // Push the "dead player" area 30 units ahead instead
                    } else {
                        followers[j].Travel(5f); // Push everything 5 units ahead, prevent the starting clipping
                    }
                }
            }
            if(i > 20)
                yield return null;
        }
        chunkCntTemp = chunkCnt; //After first runthrough 

        while(true) {
            while (currentChunkCount >= chunkCntTemp)
            {
                yield return null;
            }

            GameObject newGO = null;
            while (newGO == null)
            {
                float selection = (float)rand.NextDouble() * totalWeight;//Random.Range(0, totalWeight);
                for(byte j = 0; j < SpawnOptions.Length + 1; j++) {
                    if(j == SpawnOptions.Length) {
                        newGO = GameObject.Instantiate<GameObject>(Straight.option);
                        break;
                    }
                    if(selection < SpawnOptions[j].weight) {
                        if((totalRot < -10 && SpawnOptions[j].vertRotDelta < -10) || (totalRot > 10 && SpawnOptions[j].vertRotDelta > 10)) break;
                        if((totalRot != 0) && SpawnOptions[j].horizRot) break;
                        totalRot += SpawnOptions[j].vertRotDelta;
                        newGO = GameObject.Instantiate<GameObject>(SpawnOptions[j].option);
                        break;
                    } else {
                        selection -= SpawnOptions[j].weight;
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

            currentChunkCount++;
        }

    }
}
