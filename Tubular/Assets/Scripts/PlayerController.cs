using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar(hook="OnKill")]
    public bool alive = true;
	[SyncVar(hook="PlayerIDChanged")]
	public int playerID = -2;
	[SyncVar]
	public float points;

    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;
    public float PlayerAgainstPipeForce = 3f;

	public Material ghostMaterial;
	
	private float rot = 0;
	private float invuln = 0;

    private bool wasJumpPressedLastFrame = false;

    private bool isGrounded {
        get {
            return Physics.Raycast(transform.position, transform.up * -1f, 1.5f, whatIsGround);
        }
    }
    private bool foundObstacle {
        get {
            return Physics.CheckSphere(transform.position + (transform.parent.forward * 1f), 0.25f, whatIsObstacle);
        }
    }

    void Start() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        rb.isKinematic = !isLocalPlayer;

        transform.SetParent(GameObject.Find("Play Area").transform);
        transform.localPosition = Vector3.up * 2f;

        if (this.isLocalPlayer)
        {
            HandshakeMsg msg = new HandshakeMsg();
            msg.username = ((GameNetworkManager)NetworkManager.singleton).Username;
            NetworkManager.singleton.client.Send(HandshakeMsg.msgType, msg);
        }
    }


    void Update() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        if(isLocalPlayer && GameManager.inst != null && GameManager.inst.IsLive()) {

            if(this.transform.localPosition.magnitude > (alive ? 80:20)) {
              this.transform.localPosition = this.alive ? Vector3.zero : new Vector3(0,12,0);
              rb.velocity = Vector3.zero;
            }
            
            var horizAxis = Input.GetAxis("Horizontal");
            if(isGrounded) {
                // Rotate player around central circle axis
                transform.RotateAround(transform.parent.position + (Vector3.up * 10f), transform.forward, horizAxis * PlayerMoveSpeed);
                rb.AddRelativeForce(0f, PlayerAgainstPipeForce * -1f, 0f, ForceMode.Force); // Force player against pipe
                Debug.DrawLine(transform.position, transform.position + (transform.up * PlayerAgainstPipeForce * -1f), Color.red);
            } else {
                rb.AddForce(horizAxis * PlayerMoveSpeed * (!alive ? 1.0f : 0.5f), 0f, 0f, ForceMode.Impulse);
            }

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f); // Force player to be on plane

            Vector3 towardCenter = (transform.localPosition - (Vector3.up * 12f))* -1f;
            transform.up = towardCenter;
            transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z);
            Debug.DrawLine(transform.position, transform.parent.position + (Vector3.up * 12f), Color.blue);


            if(Input.GetButtonDown("Jump") && !wasJumpPressedLastFrame) {
                PowerupBase[] powerups = this.GetComponents<PowerupBase>();
                bool grounded = isGrounded;
                float multiplier = 1.0f;
                foreach(PowerupBase p in powerups) {
                    Debug.Log(p.name + " " + p.Active);
                    if(p.Active)
                        multiplier *= p.OnJumpPressed(grounded);
                }
                if(!grounded && multiplier > 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Stop vertical 
                if(multiplier > 0) rb.AddRelativeForce(0f, 8f*multiplier, 0f, ForceMode.Impulse);
                if(grounded)
                    rb.AddRelativeForce(0f, 8f * multiplier, 0f, ForceMode.Impulse);
                else
                    rb.AddForce(0, 8f * multiplier, 0f, ForceMode.Impulse);
            }
            wasJumpPressedLastFrame = Input.GetButtonDown("Jump");

            if(alive && invuln <= 0 && foundObstacle) {

                PowerupBase[] powerups = this.GetComponents<PowerupBase>();
                bool kill = true;
                foreach(PowerupBase p in powerups) {
                    if(p.Active)
                        kill &= p.OnObstacleCollision(null);
                }
                
                if(kill) {
                    alive = false;
					this.GetComponent<Collider>().isTrigger = true;
					this.GetComponent<TrailRenderer>().enabled = false;
                    CmdKill();//alive = false;
                    
                    transform.SetParent(GameObject.Find("Dead Area").transform, false);
                    transform.localPosition = Vector3.up * 10f; //Fly up
                    rb.velocity = Vector3.zero;
                    rb.useGravity = false;
                    this.GetComponent<DeadPowerup>().Activate ();
                    rb.drag = 0.85f;
                    Debug.Log("HIT");
                } else {
                    invuln = 2;
                }
            }

            if(invuln > 0) invuln -= Time.deltaTime;
        }
    }

    public void Collision(GameObject go, bool hostile) {
        if(hostile) {
            alive = false;
            rb.useGravity = false;
            rb.drag = 0.85f;
            //Kill player, activate ghost mode
        } else {
            //Add points and multiplier
            //TODO change hostile to enum representing type e.g. gate, obstacle, powerup
        }
    }

    [ClientRpc]
    public void RpcOnPowerupCollected(string powerupType) {
        Debug.Log("Get Power " + this.isClient);
        PowerupBase powerup = (PowerupBase)this.GetComponent(powerupType);
        powerup.OnCollected();
    }

	[Command]
	public void CmdKill(){
		this.alive = false;
		this.GetComponent<TrailRenderer>().enabled = false;
	}

	private void OnKill(bool alive){
		if(alive == this.alive) return;
		this.GetComponent<TrailRenderer>().enabled = false;
		if(!alive && !this.isLocalPlayer){
			this.transform.FindChild("Model").gameObject.SetActive(false);
		}else if(this.isLocalPlayer){
			Transform model = this.transform.FindChild("Model");
			model.GetComponent<Renderer>().material = this.ghostMaterial;
			model.localScale = new Vector3(model.localScale.x, model.localScale.y, -model.localScale.z);
		}
		this.alive = alive;
	}
  
    struct PlayerColor
    {
        public Color color;
        public Color sigcolor;
        public float metal;
        public float smooth;
        public PlayerColor(Color c, Color sc, float metal, float smooth)
        {
            color = c;
            sigcolor = sc;
            this.metal = metal;
            this.smooth = smooth;
        }
    }
	private static PlayerColor[] colors = { new PlayerColor(new Color(48/255f,227/255f,234/255f), new Color(0,73/255f,216/255f), 0.42f, 0.79f),
                                            new PlayerColor(new Color(240/255f,90/255f,90/255f), new Color(255/255f,0,0), 0.8f, 0.84f),
                                            new PlayerColor(new Color(155/255f,183/255f,113/255f), new Color(79/255f,216/255f,79/255f), 0.56f, 0.79f),
                                            new PlayerColor(new Color(212/255f,77/255f,251/255f), new Color(183/255f,66/255f,216/255f), 0.56f, 0.88f)};
	public void PlayerIDChanged(int value){
		this.playerID = value;
        SetColors(value+1);
    }

    [ClientRpc]
    public void RpcSetPlayerID(int value, string username)
    {
        this.playerID = value;
		SetColors(value + 1);
		Debug.Log("PLAYER " + value);
		GameObject.Find ("P" + Mathf.Max(1,this.playerID+1) + "ScoreArea").GetComponent<Image>().enabled = true;
		GameObject.Find ("P" + Mathf.Max(1,this.playerID+1) + "username").GetComponent<Text>().enabled = true;
		GameObject.Find ("p" + Mathf.Max(1,this.playerID+1) + "score").GetComponent<Text>().enabled = true;
		GameObject.Find ("p" + Mathf.Max(1,this.playerID+1) + "score").GetComponent<Text>().text = "0000";
		GameObject.Find ("P" + Mathf.Max(1,this.playerID+1) + "username").GetComponent<Text>().text = username;
    }

    [Command]
    public void CmdSetPlayerID(int value, string username)
    {
        this.playerID = value;
		RpcSetPlayerID(value, username);
        SetColors(value + 1);
    }

    private void SetColors(int colID)
	{

        Renderer rend = this.transform.FindChild("pPrism4").GetComponent<Renderer>();
        Renderer sigrend = this.transform.FindChild("sigcolor").GetComponent<Renderer>();
        PlayerColor c = colors[colID % colors.Length];
//        Debug.Log("BITCHES " + c.color + " FUCKER " + Color.red);
		rend.material.SetColor("_EmissionColor",c.color);
        rend.material.SetFloat("_Metallic", c.metal);
        rend.material.SetFloat("_Glossiness", c.smooth);
        sigrend.material.SetColor("_Color", c.color);
        sigrend.material.SetColor("_EmissionColor", c.sigcolor);
        this.GetComponent<TrailRenderer>().material.SetColor("_Color", rend.material.color);
    }

	[Command]
	public void CmdAddPoint(float points){
		this.points += points;
		PowerupBase[] powerups = this.GetComponents<PowerupBase>();
		foreach(PowerupBase p in powerups) {
			//Debug.Log(p.name + " " + p.Active);
			float nscore = p.OnScoreGained(this.points, points);
			if(p.Active && nscore != -1) 
			{
				this.points = nscore;
				//powerup = true;
				break;
			}
		}
		GameObject.Find ("p" + Mathf.Max(1,this.playerID+1) + "score").GetComponent<Text>().text = ((int)Mathf.Round(this.points)).ToString().PadLeft(4,'0');
		RpcSetScore (this.points);
	}

	[ClientRpc]
	private void RpcSetScore(float score){
		bool powerup = false;
		PowerupBase[] powerups = this.GetComponents<PowerupBase>();
		foreach(PowerupBase p in powerups) {
			//Debug.Log(p.name + " " + p.Active);
			float nscore = p.OnScoreGained(this.points, score - this.points);
			if(p.Active && nscore != -1) 
			{
				this.points = nscore;
				powerup = true;
				break;
			}
		}
		if(!powerup) this.points = score;

		GameObject.Find ("p" + Mathf.Max(1,this.playerID+1) + "score").GetComponent<Text>().text = ((int)Mathf.Round(this.points)).ToString().PadLeft(4,'0');
	}
}