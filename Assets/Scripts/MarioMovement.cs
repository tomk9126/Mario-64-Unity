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
    public bool readyToLongJump;
    public bool longJumping;
    public float longJumpForceMultiplier;
    public float longJumpSpeedMultiplier;

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
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        
        //rotate player's orientation
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


        //when to long jump
        if (Input.GetButton("Jump") && Input.GetButton("Fire3") && readyToLongJump && grounded)
        {   
            Debug.Log("Long Jump");
            longJumping = true;
            readyToJump = false;
            readyToLongJump = false;
            Jump(longJumpForceMultiplier);
            Invoke(nameof(ResetJump), jumpCooldown);
            rb.AddForce(moveDirection * moveSpeed * longJumpSpeedMultiplier, ForceMode.Acceleration);
        }

        //when to jump
        else if (Input.GetButton("Jump") && readyToJump && grounded)
        {
            Debug.Log("Jump");
            readyToJump = false;
            readyToLongJump = false;
            Jump(1f);
            Invoke(nameof(ResetJump), jumpCooldown);
            
        }
        

        //calculate movement direction
        moveDirection = inputDir.normalized;

        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * airMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        if (inputDir != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        //ground check
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SpeedControl()
    {
        float maxSpeed = moveSpeed;

        if (!grounded && longJumping)
        {
            maxSpeed *= longJumpSpeedMultiplier;
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit player speed
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }

    private void Jump(float jumpMultiplier)
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);
    }

    private void ResetJump() 
    {
        readyToJump = true;
        readyToLongJump = true;
        longJumping = false;
           
    }

}

