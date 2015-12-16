using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar]
    public bool alive = true;
	[SyncVar(hook="PlayerIDChanged")]
	public int playerID;

    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;

    private float rot = 0;

	private float invuln = 0;

    private bool wasJumpPressedLastFrame = false;

    private bool isGrounded {
        get {
            return Physics.Raycast(transform.position, Vector3.up * -1f, 1.5f, whatIsGround);
        }
    }
    private bool foundObstacle {
        get {
            return Physics.CheckSphere(transform.position + (Vector3.forward * 1f), 0.25f, whatIsObstacle);
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
            var horizAxis = Input.GetAxis("Horizontal");

            if(!alive) { //Ghost controls
                rb.velocity = (transform.parent.right * horizAxis * PlayerMoveSpeed) + (Vector3.up * rb.velocity.y);
            } else {
                rb.velocity = (transform.parent.right * horizAxis * PlayerMoveSpeed * (isGrounded ? 1.0f : 0.5f)) + (Vector3.up * rb.velocity.y);
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);


            if (Input.GetButtonDown("Jump") && !wasJumpPressedLastFrame)
            {
                PowerupBase[] powerups = this.GetComponents<PowerupBase>();
                bool grounded = isGrounded;
                float multiplier = 1.0f;
                foreach (PowerupBase p in powerups)
                {
                    Debug.Log(p.name + " " + p.Active);
                    if (p.Active)
                        multiplier *= p.OnJumpPressed(grounded);
                }
                if (!grounded && multiplier > 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Stop vertical 
                rb.AddRelativeForce(0f, 16f*multiplier, 0f, ForceMode.Impulse);
            }
            wasJumpPressedLastFrame = Input.GetButtonDown("Jump");

            if(horizAxis > 0.05f) {
                rot--;
            }

            if(horizAxis < -0.05f) {
                rot++;
            }
            rot = Mathf.Clamp(rot, -40f, 40f);


            if(horizAxis > -0.05f && horizAxis < 0.05f) {
                if(rot < 0) {
                    rot++;
                } else if(rot > 0) {
                    rot--;
                }
            }

            this.transform.localEulerAngles = new Vector3(0, 0, rot);

            if(alive && invuln <= 0 && foundObstacle) {

				PowerupBase[] powerups = this.GetComponents<PowerupBase>();
				bool kill = true;
				foreach(PowerupBase p in powerups){
					if(p.Active)
						kill &= p.OnObstacleCollision(null);
				}

				if(kill){
	                alive = false;
                    transform.SetParent(GameObject.Find("Dead Area").transform, false);
                    transform.localPosition = Vector3.up * 2f;
                    rb.velocity = Vector3.zero;
	                rb.drag = 0.85f;
	                Debug.Log("HIT");
				}else{
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
    public void RpcOnPowerupCollected(string powerupType)
    {
        Debug.Log("Get Power " + this.isClient);
        PowerupBase powerup = (PowerupBase)this.GetComponent(powerupType);
        powerup.OnCollected();
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
    public void RpcSetPlayerID(int value)
    {
        this.playerID = value;
        SetColors(value + 1);
    }

    [Command]
    public void CmdSetPlayerID(int value)
    {
        this.playerID = value;
        RpcSetPlayerID(value);
        SetColors(value + 1);
    }

    private void SetColors(int colID)
    {
        Renderer rend = this.transform.FindChild("pPrism4").GetComponent<Renderer>();
        Renderer sigrend = this.transform.FindChild("sigcolor").GetComponent<Renderer>();
        PlayerColor c = colors[colID % colors.Length];
        Debug.Log("BITCHES " + c.color + " FUCKER " + Color.red);
        rend.material.SetColor("_Color",c.color);
        rend.material.SetFloat("_Metallic", c.metal);
        rend.material.SetFloat("_Glossiness", c.smooth);
        sigrend.material.SetColor("_Color", c.color);
        sigrend.material.SetColor("_EmissionColor", c.sigcolor);
        this.GetComponent<TrailRenderer>().material.SetColor("_Color", rend.material.color);
    }
}