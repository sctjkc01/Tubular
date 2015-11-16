using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar]
    public bool alive = true;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;

    private float rot = 0;
    private bool left = false;
    private bool right = false;

	private float invuln = 0;

    private bool wasJumpPressedLastFrame = false;

    private bool isGrounded {
        get {
            return Physics.CheckSphere(transform.position + (Vector3.up * -1f), 0.25f, whatIsGround);
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
    }


    void Update() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        if(isLocalPlayer && GameManager.inst != null && GameManager.inst.IsLive()) {

            //left
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                left = true;
            }
            if(Input.GetKeyUp(KeyCode.LeftArrow)) {
                left = false;
            }
            //right
            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                right = true;
            }
            if(Input.GetKeyUp(KeyCode.RightArrow)) {
                right = false;
            }

            if(!alive) { //Ghost controls
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * PlayerMoveSpeed, rb.velocity.y, 0f);
            } else {
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * PlayerMoveSpeed * (isGrounded ? 1.0f : 0.5f), rb.velocity.y, 0f);
            }


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

            if(right) {
                rot--;
            }

            if(left) {
                rot++;
            }
            rot = Mathf.Clamp(rot, -40f, 40f);


            if(right == false && left == false) {
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
	                transform.position = transform.position + (transform.up * 15f);
                    rb.velocity = Vector3.zero;
	                rb.useGravity = false;
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
}