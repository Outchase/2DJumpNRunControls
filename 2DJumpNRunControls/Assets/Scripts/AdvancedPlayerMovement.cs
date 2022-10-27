using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedPlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private bool isGround;
    private bool isWallRight;
    private bool isWallLeft;
    private bool isGrabbing = false;
    private bool didWallJump = false;
    private Rigidbody2D rb;
    private float tempGrounded = 0;
    private float tempJumpPress = 0;
    private float horizontalVelocity = 0;
    private int extraAmountOfJumps;
    private float wallGrabCounter = 0;
    private float tempGravity = 0;


    [SerializeField] bool enableDash = false;
    [SerializeField] float acceleration = 1;
    [SerializeField] float speedMultipier = 2;
    [SerializeField][Range(0, 1)] float dampingMovingForward = 0.5f;
    [SerializeField][Range(0, 1)] float dampingWhenStopping = 0.5f;
    [SerializeField][Range(0, 1)] float dampingWhenTurning = 0.8f;
    [SerializeField] bool enableWallJump = false;
    [SerializeField] bool enableDoubleJump = false;
    [SerializeField] float shortJump = 9f;
    [SerializeField] float jumpForce = 18f;
    [SerializeField] float horizontalWallJumpForce = 10f;
    [SerializeField] float verticalWallJumpForce = 12f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCenter;
    [SerializeField] Transform wallCenterRight;
    [SerializeField] Transform wallCenterLeft;
    [SerializeField] float coyoteTimer = 0.05f;
    [SerializeField] float JumpBeforeGroundTimer = 0.2f;
    [SerializeField] float wallGrabTimer = 0.2f;
    [SerializeField] float wallJumpTimer = 0.2f;

    public void OnMove(InputAction.CallbackContext context)
    {
        
            movementDirection = context.ReadValue<Vector2>();
        
    }

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        tempGravity = rb.gravityScale;
    }

    private void Update()
    {
        //verify if wall or ground is touched
        isGround = Physics2D.OverlapCircle(groundCenter.position, 0.2f, groundLayer);
        isWallRight = Physics2D.OverlapCircle(wallCenterRight.position, 0.2f, groundLayer);
        isWallLeft = Physics2D.OverlapCircle(wallCenterLeft.position, 0.2f, groundLayer);

        //coyote Time set whenever player is on ground
        tempGrounded -= Time.deltaTime;
        if (isGround)
        {
            tempGrounded = coyoteTimer;
        }

        //let player jump before hiting the ground
        tempJumpPress -= Time.deltaTime;
        if (tempJumpPress > 0 && tempGrounded > 0)
        {
            tempJumpPress = 0;
            tempGrounded = 0;
            extraAmountOfJumps = 1;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (enableWallJump)
        {
            WallJumpControler();
        }

        Movement();

        //apply speed to the player movement 
        if (!didWallJump)
        {
            rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {


        if (context.started)
        {
            tempJumpPress = JumpBeforeGroundTimer;

            if (enableDoubleJump && extraAmountOfJumps > 0 && !isGrabbing)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                extraAmountOfJumps--;
            }
        }

        if (context.started && isGrabbing)
        {
            didWallJump = true;
        }



        if (context.canceled)
        {
            if (rb.velocity.y > 0f && isGround)
            {
                rb.velocity = new Vector2(rb.velocity.x, shortJump);
            }

        }
    }

    private void WallJumpControler()
    {
        if (!isGround)
        {
            if (isWallLeft && wallGrabCounter >= 0 || isWallRight && wallGrabCounter >= 0)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                wallGrabCounter -= Time.deltaTime;
                isGrabbing = true;

                //listens to jump event
                if (didWallJump)
                {
                    if (isWallLeft)
                    {
                        rb.velocity = new Vector2(horizontalWallJumpForce, verticalWallJumpForce);
                    }
                    else
                    {
                        rb.velocity = new Vector2(-horizontalWallJumpForce, verticalWallJumpForce);
                    }
                    //reset grabtime and set recovery walljump after sucessful jump

                    Invoke(nameof(SetWallJumpToFalse), wallJumpTimer);
                }

            }
            else
            {
                rb.gravityScale = tempGravity;
                isGrabbing = false;

            }
        }
        else
        {
            //reset grabtime after touching the ground
            SetWallJumpToFalse();
        }


    }

    public void OnDash(InputAction.CallbackContext context)
    {
        //cut acceleration in half to multiply speed
        if (enableDash)
        {
            if (context.performed)
            {
                dampingMovingForward /= speedMultipier;
            }

            if (context.canceled)
            {
                dampingMovingForward *= speedMultipier;
            }
        }
    }

    public void SetWallJumpToFalse()
    {
        didWallJump = false;
        wallGrabCounter = wallGrabTimer;

    }

    public void Movement()
    {
        //add basic velocity each frame
        horizontalVelocity = rb.velocity.x;
        horizontalVelocity += movementDirection.x;

        //flip negaive number into positive, verify movement direction and on 0 damp speed
        //damp speed when direction is switched
        //damp while moving forward
        if (Mathf.Abs(movementDirection.x) < 0.01f) //Âbs flips negative to positive
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingWhenStopping, Time.deltaTime * 10f);
        }
        else if (Mathf.Sign(movementDirection.x) != Mathf.Sign(horizontalVelocity))// Sign turns positive and 0 to 1 and negative to -1
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingWhenTurning, Time.deltaTime * 10f);
        }
        else
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingMovingForward, Time.deltaTime * 10f);//x^y
        }

    }
}
