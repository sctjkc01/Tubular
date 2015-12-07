using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar(hook="OnKill")]
    public bool alive = true;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;

	public Material ghostMaterial;

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
            return Physics.CheckSphere(transform.position + (Vector3.forward * 1f) + (Vector3.up * 0.005f), 0.25f, whatIsObstacle);
        }
    }

    void Start() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        rb.isKinematic = !isLocalPlayer;

        transform.SetParent(GameObject.Find("Play Area").transform);
        transform.localPosition = Vector3.up * 2f;
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
                    if (p.Active)
                        multiplier *= p.OnJumpPressed(grounded);
                }
                if (!grounded && multiplier > 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Stop vertical 
                if(multiplier > 0) rb.AddRelativeForce(0f, 16f*multiplier, 0f, ForceMode.Impulse);
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

                if (kill)
                {
                    this.GetComponent<Collider>().isTrigger = true;
					CmdKill();//alive = false;
                    transform.SetParent(GameObject.Find("Dead Area").transform, false);
                    transform.localPosition = Vector3.up * 10f; //Fly up
                    rb.velocity = Vector3.zero;
	                rb.drag = 0.85f;
					rb.useGravity = false;
					this.GetComponent<DeadPowerup>().Activate ();
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

	[Command]
	public void CmdKill(){
		this.alive = false;
	}

	private void OnKill(bool alive){
		if(alive == this.alive) return;
		if(!alive && !this.isLocalPlayer){
			this.transform.FindChild("Model").gameObject.SetActive(false);
		}else if(this.isLocalPlayer){
			Transform model = this.transform.FindChild("Model");
			model.GetComponent<Renderer>().material = this.ghostMaterial;
			model.localScale = new Vector3(model.localScale.x, model.localScale.y, -model.localScale.z);
		}
		this.alive = alive;
	}
}