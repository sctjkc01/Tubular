using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerupScore : PowerupBase {

	[SerializeField]
	private GameObject shieldVisual;
	
	[SerializeField]
	private float duration;
	private float time;

	// Use this for initialization
	void Start () {
		//shieldVisual.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
	}

	public override void OnCollected(){
		this.isActive = true;
		this.time = duration;
		//shieldVisual.SetActive(true);
		CmdShowVisual(true);
	}
	public override float OnScoreGained(float curScore, float add) { 
		Debug.Log("SCORE " + curScore + " " + add + " " + time);
		if(time > 0)
			return curScore + add * 10f;
		return -1;
	}

	public override void OnActivated(bool newValue){
		if(newValue != this.isActive){
			//shieldVisual.SetActive(newValue);
			this.time = duration;
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
		//if(shieldVisual) shieldVisual.SetActive(val);
	}
}
