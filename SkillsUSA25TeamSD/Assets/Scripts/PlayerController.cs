using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 50f;
    public float jumpForce = 10f;

    Rigidbody playerRb;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    public float groundDrag;

    [Header("Wall Interactions")]
    public bool onWall;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.drag = groundDrag;
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.9f, whatIsGround);
        if (grounded)
        {
            playerRb.drag = groundDrag;
        }
        if (Input.GetKeyDown(KeyCode.Space) && grounded || (Input.GetKeyDown(KeyCode.Space) && onWall))
        {
            Jump();
        }
        if (onWall)
        {
            playerRb.drag = groundDrag / 2;
        }

    }

    private void FixedUpdate()
    {
        PlayerMovements();
    }

    private void PlayerMovements()
    {
        // Checks if the keys for the axis "Horizontal" are being inputted, and gives it a value between -1 and 1.
        float horizontal = Input.GetAxis("Horizontal");

        playerRb.AddForce(Vector3.right * movementSpeed * horizontal, ForceMode.Force);
    }

    private void Jump()
    {
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        playerRb.velocity = new Vector3(playerRb.velocity.x, 4, playerRb.velocity.z);
        playerRb.drag /= 2;
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
