using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CollectablePowerup : NetworkBehaviour
{
    [SerializeField]
    private string powerupClassName;

    private bool collected = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider c)
    {
        Debug.Log("Powerup " + c.tag + " " + this.isServer);
        if (!this.collected && this.isServer && c.tag == "Player")
        {
            this.collected = true;
            c.GetComponent<PlayerController>().RpcOnPowerupCollected(powerupClassName);
            Destroy(this.gameObject);
        }
    }
}
