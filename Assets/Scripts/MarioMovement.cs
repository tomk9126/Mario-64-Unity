using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObject;
    public Rigidbody rb;

    public float rotationSpeed;

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;
    float horizontalInput;
    float verticalInput;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    Vector3 moveDirection;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    bool grounded;

    private void Start()
    {
        rb.freezeRotation = true;
    }
    private void Update()
    {   
        SpeedControl();
        //gruond check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        //rotate player orientation
        
        Vector3 viewDir = player.position - transform.position;
        if (viewDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //rotate the player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        

        //calculate movement direction
        moveDirection = inputDir.normalized;

        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * airMultiplier, ForceMode.Acceleration);
        }
        
        if (inputDir != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        //ground drag
        if (grounded)
        {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }

        //when to jump
        if(Input.GetButton("Jump") && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        //limit Mario's speed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Debug.Log("Jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }

}
