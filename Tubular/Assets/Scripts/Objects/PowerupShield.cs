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
		shieldVisual.SetActive(true);
	}

	public override bool OnObstacleCollision(GameObject obstacle){
		//NetworkServer.Destroy(obstacle);
		this.isActive = false;
		shieldVisual.SetActive(false);
		return false;
	}

	public override void OnActivated(bool newValue){
		if(newValue != this.isActive){
			shieldVisual.SetActive(newValue);
		}
		base.OnActivated(newValue);
	}
}
