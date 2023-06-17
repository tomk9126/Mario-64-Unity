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

    [Header("Physics")]
    public float groundDrag;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    private float moveMagnitude;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float longJumpForceMultiplier;
    public float longJumpSpeedMultiplier;
    public int ticksAfterJump;
    public float rotationSpeed;

    [Header("Score")]
    public int coins;
    public int redCoins;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    public bool grounded;
    public bool readyToJump;
    public bool readyToLongJump;
    private bool longJumping;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleInput();
        GroundCheck();
        RotatePlayerOrientation();
        MovePlayer();
        SpeedControl();
    }


    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveMagnitude = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
    }

    private void RotatePlayerOrientation()
    {
        Vector3 viewDir = player.position - transform.position;
        if (viewDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (ticksAfterJump < 99)
        {
            ticksAfterJump++;
        }

        //check jump inputs
        if (Input.GetButton("Jump") && Input.GetButton("Fire3") && readyToLongJump && grounded)
        {
            PerformLongJump(inputDir);
        }
        else if (Input.GetButton("Jump") && readyToJump && grounded)
        {
            PerformJump();
        }

        //jump and long jump
        MoveDirection(inputDir);

        if (Input.GetButtonDown("Jump") && readyToJump && grounded)
        {
            PerformJump();
        }
        else if (Input.GetButtonDown("Jump") && Input.GetButton("Fire3") && readyToLongJump && grounded)
        {
            PerformLongJump(inputDir);
        }
    }

    private void MoveDirection(Vector3 inputDir)
    {
        //move player in direction
        Vector3 moveDirection = inputDir.normalized;

        if (grounded)
        {
            rb.AddForce(moveDirection * moveMagnitude * moveSpeed, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection * moveMagnitude * moveSpeed * airMultiplier, ForceMode.Acceleration);
        }

        if (inputDir != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        rb.drag = grounded ? groundDrag : 0;
    }

    private void PerformLongJump(Vector3 inputDir)
    {
        Debug.Log("Long Jump");
        longJumping = true;
        readyToJump = false;
        readyToLongJump = false;
        Jump(longJumpForceMultiplier);
        Invoke(nameof(ResetJump), jumpCooldown);
        rb.AddForce(inputDir * moveSpeed * longJumpSpeedMultiplier, ForceMode.Acceleration);
    }

    private void PerformJump()
    {
        Debug.Log("Jump");
        readyToJump = false;
        readyToLongJump = false;
        Jump(1f);
        Invoke(nameof(ResetJump), jumpCooldown);
        ticksAfterJump = 0;
    }

    private void Jump(float jumpMultiplier)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        readyToLongJump = true;
        longJumping = false;
    }

    private void SpeedControl()
    {
        //cap player's speed unless long jumping
        float maxSpeed = moveSpeed;

        if (!grounded && longJumping)
        {
            maxSpeed *= longJumpSpeedMultiplier;
            rb.AddForce(rb.velocity.normalized * moveMagnitude * moveSpeed, ForceMode.Acceleration);
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
