using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 200f;
    private Vector2 movementDirection;
    private Rigidbody2D rb;


    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
       
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movementDirection.x * speed * Time.deltaTime, rb.velocity.y);
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
