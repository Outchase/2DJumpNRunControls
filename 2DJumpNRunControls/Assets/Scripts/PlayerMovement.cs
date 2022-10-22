using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 400f;
    public float jumpForce = 10f;
    private Vector2 movementDirection;
    private Rigidbody2D rb;
    private bool isGround;

    public Transform groundCenter;
    public LayerMask groundLayer;


    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
        isGround = Physics2D.OverlapCircle(groundCenter.position, .2f, groundLayer);
    }

    public void Awake()
    {
        //groundCenter = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Jump(InputAction.CallbackContext context) {

        Debug.Log(isGround);
        if (isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
    }


}
