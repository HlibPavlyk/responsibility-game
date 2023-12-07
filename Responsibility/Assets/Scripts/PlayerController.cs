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
    Animator animator;
   /* SpriteRenderer spriteRenderer;*/
    Vector2 moveInput = Vector2.zero;
    /*private Vector2 previousPosition = new Vector2 (1, 1);*/


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        inputManager = GameManager.Instance.InputManager;

        animator = GetComponent<Animator>();
        /*spriteRenderer = GetComponent<SpriteRenderer>();*/
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

        controls.Player.Submit.started += context => inputManager.SubmitButtonPressed(context);
        controls.Player.Submit.performed += context => inputManager.SubmitButtonPressed(context);
        controls.Player.Submit.canceled += context => inputManager.SubmitButtonPressed(context);
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Interact.started -= context => inputManager.InteractButtonPressed(context);
        controls.Player.Submit.started -= context => inputManager.SubmitButtonPressed(context);
    }

    void FixedUpdate()
    {

        if (GameManager.Instance.DialogueManager.isDialoguePlaying || GameManager.Instance.LevelManager.isTransitionAnimationPlaying)
            /*rb.position == previousPosition)*/
        {
            moveInput = Vector2.zero; //stop character movement while he talks
            animator.SetBool("IsWalking", false);
        }


        if (moveInput != Vector2.zero)
        {
            /*previousPosition = rb.position;*/
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (moveInput * moveSpeed * Time.deltaTime), maxSpeed);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, idleFriction);
        }
    }

    void OnMove(InputValue value)
    {

        if (animator != null)
        {
            if (GameManager.Instance.LevelManager.isTransitionAnimationPlaying || GameManager.Instance.DialogueManager.isDialoguePlaying)
                moveInput = Vector2.zero;
            else
                moveInput = value.Get<Vector2>();

            if (moveInput.x != 0 || moveInput.y != 0)
            {
                animator.SetFloat("X", moveInput.x);
                animator.SetFloat("Y", moveInput.y);

                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
    }

    /*void UpdateAnimatorParameters()
    {
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
    }*/


}