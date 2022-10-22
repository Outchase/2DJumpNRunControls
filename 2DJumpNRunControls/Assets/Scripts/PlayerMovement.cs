using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private bool isGround;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] int amountOfJumps;
    [SerializeField] Transform groundCenter;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speedMultipier;
    [SerializeField] float shortJump;
    [SerializeField] bool antiMidAirControl;


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
        if (antiMidAirControl)
        {
            AntiGravity();
        }
        else
        {
            rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }

    }

    public void AntiGravity()
    {
        if (isGround)
        {
            rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }
        else
        {
            rb.AddForce(movementDirection * speed * Time.deltaTime);
        }
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
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * shortJump);
            }
            amountOfJumps--;
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
