using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private bool isGround;

    public float speed = 400f;
    public float jumpForce = 17f;
    public int amountOfJumps;
    public Transform groundCenter;
    public LayerMask groundLayer;


    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    public void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCenter.position, .2f, groundLayer);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGround)
        {
            amountOfJumps = 3;

        }

        if (amountOfJumps >= 0)
        {
            
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
            }
            amountOfJumps--;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (isGround)
        {
            if (context.performed)
            {
                speed = speed *2;
            }
        }

        if (context.canceled)
        {
            speed = 400f;

        }

    }
}
