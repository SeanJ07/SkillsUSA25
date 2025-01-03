using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 50f;
    public float jumpForce = 20f;
    private float gravMultiplier = 1;
    public bool locked = false;

    Rigidbody playerRb;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    public float groundDrag;

    [Header("Wall Interactions")]
    public bool onWall;

    public bool topdownView;

    public bool playerWalking;

    

    [Header("Player Stats")]
    public float health = 100;

    [Header("Enemy Interactions")]
    public float damage;
    public float hitCooldown;
    private bool alreadyHit;

    [Header("Sound Settings")]
    public AudioClip attackingSFX;
    private AudioSource audioSource;
    public AudioClip hurt;
    public AudioClip collect;
    public AudioClip walking; //not sure how to code the walking since i dont want it to play in the air

    float walkSoundCd = .5f;

    private void Awake()
    {
        locked = false;
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.drag = groundDrag;

        audioSource = GetComponent<AudioSource>();
        locked = false;
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.9f, whatIsGround);
        if (grounded)
        {
            playerRb.drag = groundDrag;
            gravMultiplier = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && grounded || (Input.GetKeyDown(KeyCode.Space) && onWall))
        {
            Jump();
        }
        if (onWall)
        {
            playerRb.drag = groundDrag / 2;
        }
        if (!grounded || !onWall)
        {
            gravMultiplier += Time.deltaTime;
            Physics.gravity = new Vector3(0, (-25 - gravMultiplier), 0);
        }
        else
        {
            Physics.gravity = new Vector3(0, -25, 0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!alreadyHit)
            {
                StartCoroutine(AttackSequence());
            }
        }


    }

    private void FixedUpdate()
    {
        if (!locked)
        { 
            PlayerMovements();

        }
        
        
    }

    private void PlayerMovements()
    {
        // Checks if the keys for the axis "Horizontal" are being inputted, and gives it a value between -1 and 1.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        playerRb.AddForce(Vector3.right * movementSpeed * horizontal, ForceMode.Force);
        if(horizontal != 0)
        {
            playerWalking = true;
        }
        else
        {
            playerWalking = false;
        }
        if (topdownView)
        {
            playerRb.AddForce(Vector3.forward * movementSpeed * vertical, ForceMode.Force);
            if(vertical != 0)
            {
                playerWalking = true;
            }
            else
            {
                playerWalking = false;
            }
        }

        if (playerWalking && grounded)
        {
            walkSoundCd -= Time.deltaTime;

            if (walkSoundCd <= 0)
            {

                audioSource.PlayOneShot(walking);
                walkSoundCd = 0.5f;
            }

        }
        else
        {
            audioSource.Stop();
        } 


    }


    private void Jump()
    {
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
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

    public void ChangeLockedStatus()
    {
        playerRb.velocity = new Vector3(0, 0, 0);
        locked = !locked;
    }

    public IEnumerator AttackSequence()
    {
        // ATTACK CODE

        audioSource.PlayOneShot(attackingSFX);
        alreadyHit = true;
        yield return new WaitForSeconds(hitCooldown);

        alreadyHit = false;
    }
}
