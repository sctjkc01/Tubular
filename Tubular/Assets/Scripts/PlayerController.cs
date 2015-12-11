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
    }


    void Update() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        if(isLocalPlayer && GameManager.inst != null && GameManager.inst.IsLive()) {
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
                if(multiplier > 0) rb.AddRelativeForce(0f, 16f*multiplier, 0f, ForceMode.Impulse);
                if(grounded)
                    rb.AddRelativeForce(0f, 16f * multiplier, 0f, ForceMode.Impulse);
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