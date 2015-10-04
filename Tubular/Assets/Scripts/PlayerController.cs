using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("Tubular Scripts/Runtime/Networked/Player Controller")]
public class PlayerController : NetworkBehaviour {
    private Rigidbody rb;
    public LayerMask whatIsGround;
    public float PlayerMoveSpeed = 10f;

    private bool isGrounded {
        get {
            return Physics.CheckSphere(transform.position + (Vector3.up * -1.25f), 0.5f, whatIsGround);
        }
    }


    void Update() {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.AddForce(Input.GetAxis("Horizontal") * PlayerMoveSpeed, 0f, 0f, ForceMode.VelocityChange);


        if (Input.GetButtonDown("Jump") && isGrounded) {
            rb.AddRelativeForce(0f, 10f, 0f, ForceMode.Impulse);
        }
    }
}
