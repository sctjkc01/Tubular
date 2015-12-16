using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerupShield : PowerupBase {

	[SerializeField]
	private GameObject shieldVisual;

	// Use this for initialization
	void Start () {
		shieldVisual.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnCollected(){
		this.isActive = true;
		//shieldVisual.SetActive(true);
		CmdShowVisual(true);
	}

	public override bool OnObstacleCollision(GameObject obstacle){
		//NetworkServer.Destroy(obstacle);
		this.isActive = false;
		//shieldVisual.SetActive(false);
		CmdShowVisual(false);
		return false;
	}

	public override void OnActivated(bool newValue){
		if(newValue != this.isActive){
			//shieldVisual.SetActive(newValue);
			CmdShowVisual(newValue);
		}
		base.OnActivated(newValue);
	}
	
	[Command]
	public void CmdShowVisual(bool val){
		RpcShowVisual(val);
	}
	[ClientRpc]
	public void RpcShowVisual(bool val){
		if(shieldVisual) shieldVisual.SetActive(val);
	}
}
