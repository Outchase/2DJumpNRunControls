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

    [SerializeField] float speed;
    [SerializeField] float speedMultipier;
    [SerializeField] float jumpForce;
    [SerializeField] float shortJump;
    [SerializeField] Transform groundCenter;
    [SerializeField] Transform wallCenter;
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
        isWall = Physics2D.OverlapCircle(wallCenter.position, .2f, groundLayer);

        if (wallJump)
        {
            WallJumpControler();
        }

        //Debug.Log(rb.velocity.y);

    }

    private void FixedUpdate()
    {
        if (antiMidAirControl)
        {
            AntiMidAir();
        }
        else
        {
            rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
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

        if (isGrabbing)
        {
            if (context.started)
            {
                rb.velocity = new Vector2(movementDirection.x * speed, rb.velocity.y * jumpForce);
            }

        }
        
        if (extraAmountOfJumps >= 0)
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * shortJump);
                extraAmountOfJumps--;
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
