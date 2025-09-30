using System.Collections;
using Core.Abstractions;
using Core.DI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Systems.Game;

public class PlayerController : InjectableDynamicMonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float idleFriction = 0.9f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invincibilityDuration = 2f;

    // DI залежності - замість GameManager.Instance
    [Inject] private IInputManager inputManager;
    [Inject] private GameState gameState;

    // Компоненти Unity
    private Rigidbody2D rb;
    private Controls controls;
    private Animator animator;
    private Vector2 moveInput = Vector2.zero;
    
    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
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

    protected override void Awake()
    {
        base.Awake();
        controls = new Controls();
    }
    
    /*private void Awake()
    {
        //base.Awake();
        controls = new Controls();
    }*/

    private void OnEnable()
    {
        controls.Enable();
        
        // Використовуємо injected inputManager замість GameManager.Instance.InputManager
        controls.Player.Interact.started += context => inputManager?.InteractButtonPressed(context);
        controls.Player.Interact.performed += context => inputManager?.InteractButtonPressed(context);
        controls.Player.Interact.canceled += context => inputManager?.InteractButtonPressed(context);

        controls.Player.Submit.started += context => inputManager?.SubmitButtonPressed(context);
        controls.Player.Submit.performed += context => inputManager?.SubmitButtonPressed(context);
        controls.Player.Submit.canceled += context => inputManager?.SubmitButtonPressed(context);
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Interact.started -= context => inputManager?.InteractButtonPressed(context);
        controls.Player.Submit.started -= context => inputManager?.SubmitButtonPressed(context);
    }

    void FixedUpdate()
    {
        // Використовуємо injected залежності замість GameManager.Instance
        var shouldStopMovement = (gameState?.isDialoguePlaying ?? false) || 
                                 (gameState?.isTransitionAnimationPlaying ?? false);

        if (shouldStopMovement)
        {
            moveInput = Vector2.zero; // Зупиняємо рух під час діалогу або переходу
            animator.SetBool("IsWalking", false);
        }

        if (moveInput != Vector2.zero)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(
                rb.linearVelocity + moveInput * (moveSpeed * Time.deltaTime), 
                maxSpeed);
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
            // Використовуємо injected залежності
            bool shouldBlockInput = (gameState?.isTransitionAnimationPlaying ?? false) || 
                                   (gameState?.isDialoguePlaying ?? false);

            if (shouldBlockInput)
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

    private IEnumerator BlinkEffect()
    {
        float blinkTime = invincibilityDuration / 10f;

        for (float i = 0; i < invincibilityDuration; i += blinkTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkTime);
        }

        spriteRenderer.enabled = true;
    }

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

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed. Current health: " + currentHealth);
    }
}
