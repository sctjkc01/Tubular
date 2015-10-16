using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Obstacle : NetworkBehaviour {

    [SerializeField]
    private bool hostile = true;

    void Start()
    {
        if (!isServer) return;
        Collider[] colliders = this.GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders)
        {
            col.isTrigger = true;
            col.gameObject.AddComponent<DetectCollisionToParent>();
        }
    }

    void OnTriggerEnterChild(Collider c)
    {
        if (!isServer) return;
        Destroy(this.gameObject);
        NetworkServer.Destroy(this.gameObject);

        PlayerController pc = c.gameObject.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.Collision(this.gameObject, hostile);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        OnTriggerEnterChild(c);
    }
}
