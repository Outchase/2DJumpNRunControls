using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private float tempGravity;
    private bool isGround;
    private bool isWall;
    private bool isGrabbing;
    private int extraAmountOfJumps;
    private float wallJumpCounter;
    private bool didWallJump;

    [SerializeField] float speed;
    [SerializeField] float speedMultipier;
    [SerializeField] float jumpForce;
    [SerializeField] float shortJump;
    [SerializeField] Transform groundCenter;
    [SerializeField] Transform wallCenterRight;
    [SerializeField] Transform wallCenterLeft;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool antiMidAirControl;
    [SerializeField] bool doubleJump;
    [SerializeField] bool wallJump;
    [SerializeField] float wallJumpTimer;



    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();

        //flip direction
        if (movementDirection.x > 0 || movementDirection.x < 0)
        {
            transform.localScale = new Vector2(movementDirection.x, transform.localScale.y);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tempGravity = rb.gravityScale;
    }

    private void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCenter.position, .2f, groundLayer);
        isWall = Physics2D.OverlapCircle(wallCenterRight.position, .2f, groundLayer);

        if (wallJump)
        {
            WallJumpControler();
            if (didWallJump)
            {
                rb.velocity = new Vector2(movementDirection.x*speed,jumpForce);
            }
        }

    }

    private void FixedUpdate()
    {
        if (antiMidAirControl)
        {
            AntiMidAir();
        }
        else
        {

            if (!didWallJump)
            {
                rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
            }
            

        }
    }

    private void AntiMidAir()
    {
        if (isGround)
        {
            rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }
        else
        {
            rb.AddForce(speed * Time.deltaTime * movementDirection);
        }
    }

    private void WallJumpControler()
    {
        if (isWall && !isGround && wallJumpCounter >= 0)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            isGrabbing = true;
            wallJumpCounter -= Time.deltaTime;
        }
        else
        {
            if (!isWall)
            {
                wallJumpCounter = wallJumpTimer;
            }
            didWallJump = false;
            isGrabbing = false;

            rb.gravityScale = tempGravity;
        }

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGround)
        {

            if (doubleJump)
            {
                extraAmountOfJumps = 1;
            }
            else
            {
                extraAmountOfJumps = 0;
            }
        }

        if (extraAmountOfJumps >= 0)
        {
            //Debug.Log("you jump");

            if (context.started)
            {
                if (isGrabbing)
                {
                    
                    didWallJump = true;
                }
                else
                {
                    extraAmountOfJumps--;
                }
            }

            if (context.performed && !isGrabbing)
            {
                rb.velocity = new Vector2(movementDirection.x, jumpForce);
            }

            if (context.canceled && rb.velocity.y > 0f && !isGrabbing)
            {
                //Debug.Log(extraAmountOfJumps);

                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * shortJump);
            }
        }




    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speed *= speedMultipier;
        }

        if (context.canceled)
        {
            speed /= speedMultipier;
        }
    }
}
