using UnityEngine;
using System.Collections;

public class DetectCollisionToParent : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        this.SendMessageUpwards("OnTriggerEnterChild", col, SendMessageOptions.DontRequireReceiver);
    }
    void OnTriggerExit(Collider col)
    {
        this.SendMessageUpwards("OnTriggerExitChild", col, SendMessageOptions.DontRequireReceiver);
    }
}
