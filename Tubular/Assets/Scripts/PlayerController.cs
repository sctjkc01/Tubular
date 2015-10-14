using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    [SyncVar]
    public bool alive;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    public float PlayerMoveSpeed = 10f;

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

        rb.useGravity = isLocalPlayer;

    }


    void Update() {
        if(rb == null) rb = GetComponent<Rigidbody>();

        if(isLocalPlayer && isGrounded) {
            rb.velocity = new Vector3(Input.GetAxis("Horizontal") * PlayerMoveSpeed, rb.velocity.y, 0f);

            if(Input.GetButtonDown("Jump")) {
                rb.AddRelativeForce(0f, 20f, 0f, ForceMode.Impulse);
            }
        }

        if(isLocalPlayer && alive && foundObstacle) {

        }
    }
}
