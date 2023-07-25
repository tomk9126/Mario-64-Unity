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
    public float horizontalInput;
    public float verticalInput;
    private float moveMagnitude;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float longJumpForceMultiplier;
    public float longJumpSpeedMultiplier;
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
        //checks directional inputs
        HandleMovementInput();

        //checks if the player can, and should, jump
        CheckJumpInput();
    }

    private void CheckJumpInput()
    {
        //chekcs for spacebar on keyboard, 'A' button on controller
        if (Input.GetButtonDown("Jump"))
        {
            if (readyToJump && grounded)
            {
                PerformJump();
            }
            //shift key on keyboard, Left bumper on controllers
            else if (Input.GetButton("Fire3") && readyToLongJump && grounded)
            {
                PerformLongJump(playerObject.forward);
            }
        }
    }
    private void FixedUpdate()
    {
        //checks unity rigidbody 'isGrounded'
        GroundCheck();

        MovePlayer();

        //cap player's speed
        SpeedControl();
    }

    private void HandleMovementInput()
    {
        //left directional stick, or WASD
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //the further the directional stick is pushed, the faster the player speeds up
        moveMagnitude = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        
    }

    private void MovePlayer()
    {
        //take into account camera rotation and adjust player's direction when moving to ensure forward movement is accurate.
        Vector3 viewDir = player.position - transform.position;
        if (viewDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            orientation.rotation = Quaternion.Slerp(orientation.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        

        if ((Input.GetButton("Jump") && Input.GetButton("Fire3") && readyToLongJump && grounded) ||
            (Input.GetButtonDown("Jump") && readyToJump && grounded))
        {
            if (Input.GetButton("Fire3"))
            {
                PerformLongJump(inputDir);
            }
            else
            {
                PerformJump();
            }
        }

        //push player in direction
        MoveInDirection(inputDir);
    }

    private void MoveInDirection(Vector3 inputDir)
    {
        Vector3 moveDirection = inputDir.normalized;

        //take into account air drag
        float moveSpeedMultiplier = grounded ? 1f : airMultiplier;

        //accelerate
        rb.AddForce(moveDirection * moveMagnitude * moveSpeed * moveSpeedMultiplier, ForceMode.Acceleration);

        //if there is no input, don't push the player forward at all
        if (inputDir != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        rb.drag = grounded ? groundDrag : 0;
    }

    private void PerformLongJump(Vector3 inputDir)
    {
        //jolt forward and slightly up
        Debug.Log("Long Jump");
        longJumping = true;
        readyToJump = false;
        readyToLongJump = false;
        Jump(longJumpForceMultiplier);

        //prevents player from jumping too soon or in air
        Invoke(nameof(ResetJump), jumpCooldown);

        //push forward
        rb.AddForce(inputDir * moveSpeed * longJumpSpeedMultiplier, ForceMode.Acceleration);
    }

    private void PerformJump()
    {
        //impulse upward
        Debug.Log("Jump");
        readyToJump = false;
        readyToLongJump = false;
        Jump(1f);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void Jump(float jumpMultiplier)
    {
        //does the upward push
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

        float maxSpeed = moveSpeed;

        //player may only bypass speed cap if they're long jumping
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
