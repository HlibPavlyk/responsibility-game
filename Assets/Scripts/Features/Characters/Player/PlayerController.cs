using System;
using System.Collections;
using Core.Abstractions;
using Core.DI;
using Systems.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Features.Characters.Player
{
    public class PlayerController : InjectableDynamicMonoBehaviour
    {
        // serialized fields
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 150f;
        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float idleFriction = 0.9f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private float invincibilityDuration = 2f;

        // injected dependencies
        [Inject] private IInputManager _inputManager;
        [Inject] private GameState _gameState;

        // unity components
        private Rigidbody2D _rb;
        private Controls _controls;
        private Animator _animator;
        private Vector2 _moveInput = Vector2.zero;
    
        // fields for monster interaction
        private int _currentHealth;
        private bool _isInvincible;
        private float _invincibilityTimer;
    
        // actions for input controls
        private Action<InputAction.CallbackContext> _onInteract;
        private Action<InputAction.CallbackContext> _onSubmit;
        
        // constants for animator
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int Y = Animator.StringToHash("Y");


        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _currentHealth = maxHealth;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (_isInvincible)
            {
                _invincibilityTimer -= Time.deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    _isInvincible = false;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _controls = new Controls();
        }

        private void OnEnable()
        {
            _onInteract = context => _inputManager?.InteractButtonPressed(context);
            _onSubmit = context => _inputManager?.SubmitButtonPressed(context);

            _controls.Enable();

            // connect interaction button
            _controls.Player.Interact.started += _onInteract;
            _controls.Player.Interact.performed += _onInteract;
            _controls.Player.Interact.canceled += _onInteract;

            // connect submit button
            _controls.Player.Submit.started += _onSubmit;
            _controls.Player.Submit.performed += _onSubmit;
            _controls.Player.Submit.canceled += _onSubmit;
        }

        private void OnDisable()
        {
            _controls.Disable();

            // disconnect interaction button
            _controls.Player.Interact.started -= _onInteract;
            _controls.Player.Interact.performed -= _onInteract;
            _controls.Player.Interact.canceled -= _onInteract;

            // disconnect submit button
            _controls.Player.Submit.started -= _onSubmit;
            _controls.Player.Submit.performed -= _onSubmit;
            _controls.Player.Submit.canceled -= _onSubmit;
        }

        void FixedUpdate()
        {
            if (IsInputBlocked())
            {
                _moveInput = Vector2.zero; // Зупиняємо рух під час діалогу або переходу
                _animator.SetBool(IsWalking, false);
            }

            if (_moveInput != Vector2.zero)
            {
                _rb.linearVelocity = Vector2.ClampMagnitude(
                    _rb.linearVelocity + _moveInput * (moveSpeed * Time.deltaTime), 
                    maxSpeed);
            }
            else
            {
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, idleFriction);
                _animator.SetBool(IsWalking, false);
            }
        }

        void OnMove(InputValue value)
        {
            if (_animator == null ||  IsInputBlocked())
            {
                _moveInput = Vector2.zero;
                return;
            }
                
            _moveInput = value.Get<Vector2>();

            if (_moveInput.x != 0 || _moveInput.y != 0)
            {
                _animator.SetFloat(X, _moveInput.x);
                _animator.SetFloat(Y, _moveInput.y);
                _animator.SetBool(IsWalking, true);
            }
            else
            {
                _animator.SetBool(IsWalking, false);
            }
        }

        private bool IsInputBlocked()
        {
            return (_gameState?.isTransitionAnimationPlaying ?? false) ||
                   (_gameState?.isDialoguePlaying ?? false) ||
                   (_gameState?.isPaused ?? false);
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
            if (_isInvincible) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

            if (_currentHealth <= 0)
            {
                Debug.Log("Player is dead!");
                // Handle player death here
            }

            _isInvincible = true;
            _invincibilityTimer = invincibilityDuration;
            Debug.Log("Player took damage. Current health: " + _currentHealth);

            StartCoroutine(BlinkEffect());
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            Debug.Log("Player healed. Current health: " + _currentHealth);
        }
    }
}
