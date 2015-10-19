using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    private GameManager gm;
    [SyncVar]
    public bool alive = true;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;

    private float rot = 0;
    private bool left = false;
    private bool right = false;

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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    void Update() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        if(isLocalPlayer && gm.live) {

            if(!alive) { //Ghost controls
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * PlayerMoveSpeed, rb.velocity.y, 0f);
            } else {
                rb.velocity = new Vector3(Input.GetAxis("Horizontal") * PlayerMoveSpeed * (isGrounded ? 1.0f : 0.5f), rb.velocity.y, 0f);
            }


            if(isGrounded && Input.GetButtonDown("Jump")) {
                rb.AddRelativeForce(0f, 20f, 0f, ForceMode.Impulse);
            }

			float horiz = Input.GetAxis ("Horizontal");
            if(horiz > 0) {
                rot--;
            }

            if(horiz < 0) {
                rot++;
            }
            rot = Mathf.Clamp(rot, -40f, 40f);


            if(horiz ==0 ) {
                if(rot < 0) {
                    rot++;
                } else if(rot > 0) {
                    rot--;
                }
            }

            this.transform.localEulerAngles = new Vector3(0, 0, rot);

            if(alive && foundObstacle) {
                alive = false;
                transform.position = new Vector3(transform.position.x, 15f, 0f);
                rb.useGravity = false;
                rb.drag = 0.85f;
                Debug.Log("HIT");
            }
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
}