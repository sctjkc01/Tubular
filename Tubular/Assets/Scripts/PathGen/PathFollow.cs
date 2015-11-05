using UnityEngine;
using System.Collections;

public class PathFollow : MonoBehaviour {
    private static float Speed {
        get {
            return GameManager.inst.GameTravelSpeed;
        }
    }

    public bool RotateY = false;
    public PathNode currNode;

    void Update() {
        float distToNextNode = Vector3.Distance(transform.position, currNode.next.transform.position);
        float travelLeft = Speed * Time.deltaTime;
        if(travelLeft > distToNextNode) {
            currNode = currNode.next;
            transform.position = currNode.transform.position;
            transform.eulerAngles = RotateY ? currNode.transform.eulerAngles : new Vector3(currNode.transform.eulerAngles.x, 0f, 0f);
        } else {
            distToNextNode -= travelLeft;
            float pct = distToNextNode / Vector3.Distance(currNode.transform.position, currNode.next.transform.position);
            transform.position = Vector3.Lerp(currNode.transform.position, currNode.next.transform.position, pct);
            Vector3 rotEuler = Quaternion.Lerp(currNode.transform.rotation, currNode.next.transform.rotation, pct).eulerAngles;
            if(!RotateY) {
                rotEuler.y = 0;
            }
            transform.eulerAngles = rotEuler;
        }
    }

}
