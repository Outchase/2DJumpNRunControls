using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedPlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private bool isGround;
    private Rigidbody2D rb;
    private float tempGrounded = 0;
    private float tempJumpPress = 0;
    private float horizontalVelocity = 0;
    private int extraAmountOfJumps;


    //[SerializeField] Transform wallCenterRight;
    //[SerializeField] Transform wallCenterLeft;
    [SerializeField] bool enableDash = false;
    [SerializeField] float acceleration = 1;
    [SerializeField] float speedMultipier = 2;
    [SerializeField][Range(0, 1)] float dampingMovingForward = 0.5f;
    [SerializeField][Range(0, 1)] float dampingWhenStopping = 0.5f;
    [SerializeField][Range(0, 1)] float dampingWhenTurning = 0.8f;
    [SerializeField] bool enableDoubleJump = false;
    [SerializeField] float shortJump = 9f;
    [SerializeField] float jumpForce = 18f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCenter;
    [SerializeField] float tempGroundTimer = 0.25f;
    [SerializeField] float tempJumpPressTimer = 0.2f;





    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCenter.position, 0.2f, groundLayer);

        tempGrounded -= Time.deltaTime;
        if (isGround)
        {
            tempGrounded = tempGroundTimer;
        }

        tempJumpPress -= Time.deltaTime;
        if (tempJumpPress > 0 && tempGrounded > 0)
        {
            tempJumpPress = 0;
            tempGrounded = 0;
            extraAmountOfJumps = 1;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        horizontalVelocity = rb.velocity.x;
        horizontalVelocity += movementDirection.x;

        if (Mathf.Abs(movementDirection.x) < 0.01f)
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingWhenStopping, Time.deltaTime * 10f);
        }
        else if (Mathf.Sign(movementDirection.x) != Mathf.Sign(horizontalVelocity))
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingWhenTurning, Time.deltaTime * 10f);
        }
        else
        {
            horizontalVelocity *= Mathf.Pow(acceleration - dampingMovingForward, Time.deltaTime * 10f);

        }

        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
       
       
            if (context.performed)
            {
                tempJumpPress = tempJumpPressTimer;

                if (enableDoubleJump && extraAmountOfJumps > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    extraAmountOfJumps--;
                }
            }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, shortJump);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
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
}
