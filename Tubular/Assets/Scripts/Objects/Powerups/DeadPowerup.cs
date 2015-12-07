using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DeadPowerup : PowerupBase
{
	
	[SerializeField]
	private GameObject deathVisual;
	
	[SerializeField]
	private GameObject bombPrefab;

    [SerializeField]
    private float timeBetweenBombs;
    private float timer;

	// Use this for initialization
	void Start()
	{
		if(deathVisual) deathVisual.SetActive(false);
	}
	
	// Update is called once per frame
	void Update()
	{
        if (timer > 0) timer -= Time.deltaTime;
	}
	
	
	public void Activate(){
		CmdShowDeath();
		this.isActive = true;
	}
	
	public override float OnJumpPressed(bool grounded) 
	{
		//Drop bomb
        if (timer <= 0)
        {
            CmdSpawnBomb(this.transform.position);
            timer = timeBetweenBombs;
        }
		
		return 0.0f;
	}
	
	[Command]
	public void CmdSpawnBomb(Vector3 pos){
		GameObject spawn = (GameObject)Instantiate(bombPrefab,pos, Quaternion.identity);
		//spawn.transform.SetParent(GameObject.Find("Obstacle"));
		Rigidbody rb = spawn.GetComponent<Rigidbody>();
		rb.AddForce(Vector3.up * 5, ForceMode.Impulse);

		NetworkServer.Spawn(spawn);
	}

	[Command]
	public void CmdShowDeath(){
		RpcShowDeath();
	}
	[ClientRpc]
	public void RpcShowDeath(){
		if(deathVisual) deathVisual.SetActive(true);
	}

}
