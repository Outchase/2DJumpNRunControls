using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private bool isGround;
    private int extraAmountOfJumps;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] Transform groundCenter;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speedMultipier;
    [SerializeField] float shortJump;
    [SerializeField] bool antiMidAirControl;
    [SerializeField] bool doubleJump;



    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void Update()
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

    private void AntiGravity()
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        

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
