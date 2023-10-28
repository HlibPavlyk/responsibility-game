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

    private Controls controls; // for Olya inputManager
    private InputManager inputManager;
    /*    Animator animator;
        SpriteRenderer spriteRenderer;*/
    Vector2 moveInput = Vector2.zero;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        inputManager = InputManager.GetInstance();

        /*animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();*/
    }

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Interact.started += context => inputManager.InteractButtonPressed(context);
        controls.Player.Interact.performed += context => inputManager.InteractButtonPressed(context);
        controls.Player.Interact.canceled += context => inputManager.InteractButtonPressed(context);
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Interact.started -= context => inputManager.InteractButtonPressed(context);
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

        if (DialogueManager.GetInstance().isDialoguePlaying)
        {
            return; //stop character movement while he talk
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