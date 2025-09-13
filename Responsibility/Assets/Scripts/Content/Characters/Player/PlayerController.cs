using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 150f;
    public float maxSpeed = 5f;
    public float idleFriction = 0.9f;
    public SpriteRenderer spriteRenderer;

    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    public float invincibilityDuration = 2f;
    private float invincibilityTimer = 0f;
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

        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        /*spriteRenderer = GetComponent<SpriteRenderer>();*/
    }

    void Update()
    {

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    private IEnumerator BlinkEffect()
    {
        float blinkTime = invincibilityDuration / 10f; // Duration of each blink

        for (float i = 0; i < invincibilityDuration; i += blinkTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle sprite visibility
            yield return new WaitForSeconds(blinkTime);
        }

        spriteRenderer.enabled = true; // Ensure sprite is visible after invincibility
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
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + (moveInput * moveSpeed * Time.deltaTime), maxSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, idleFriction);
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

    // Method to handle taking damage
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
            // Handle player death here
        }

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        Debug.Log("Player took damage. Current health: " + currentHealth);

        StartCoroutine(BlinkEffect());
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    /*void UpdateAnimatorParameters()
    {
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
    }*/


}