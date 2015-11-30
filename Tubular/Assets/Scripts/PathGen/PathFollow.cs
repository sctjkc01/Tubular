using UnityEngine;
using System.Collections;

public class PathFollow : MonoBehaviour {
    public bool DestroysChunks = false;
    private static float Speed {
        get {
            return GameManager.inst.live ? GameManager.inst.GameTravelSpeed : 0f;
        }
    }

    public bool RotateX = false;
    public PathNode currNode;

    void Update() {
        Travel(Speed * Time.deltaTime);
    }

    public void Travel(float amt) {
        while(currNode.next && amt > 0f) {
            float distToNextNode = Vector3.Distance(transform.position, currNode.next.transform.position);
            if(amt > distToNextNode) {
                amt -= distToNextNode;
                if(DestroysChunks && currNode is PathChunkExit) {
                    currNode.GetComponentInParent<PathChunk>().passed = true;
                }
                currNode = currNode.next;
                transform.position = currNode.transform.position;
                transform.eulerAngles = RotateX ? currNode.transform.eulerAngles : new Vector3(0f, currNode.transform.eulerAngles.y, 0f);
            } else {
                distToNextNode -= amt;
                amt = 0f;
                float pct = distToNextNode / Vector3.Distance(currNode.transform.position, currNode.next.transform.position);
                transform.position = Vector3.Lerp(currNode.next.transform.position, currNode.transform.position, pct);
                Vector3 rotEuler = Quaternion.Lerp(currNode.next.transform.rotation, currNode.transform.rotation, pct).eulerAngles;
                if(!RotateX) {
                    rotEuler.x = 0;
                }
                Debug.Log(gameObject.name + " rot: " + rotEuler.ToString("2f"));
                transform.eulerAngles = rotEuler;
            }

        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 5.0f));
        Gizmos.color = Color.white;
    }

}
