using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 150f;
    public float maxSpeed = 5f;

    public float idleFriction = 0.9f;
    Rigidbody2D rb;
/*    Animator animator;
    SpriteRenderer spriteRenderer;*/
    Vector2 moveInput = Vector2.zero;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        /*animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();*/
    }

    void FixedUpdate()
    {

        if (moveInput != Vector2.zero)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (moveInput * moveSpeed * Time.deltaTime), maxSpeed);

            /*if (moveInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            UpdateAnimatorParameters();*/
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, idleFriction);
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    /*void UpdateAnimatorParameters()
    {
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
    }*/
}
