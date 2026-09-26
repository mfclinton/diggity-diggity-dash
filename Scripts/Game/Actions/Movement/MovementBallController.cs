using Game.Input.Data;
using Game.Input.Data.Interfaces;
using Game.Actions.Base;
using UnityEngine;

namespace Game.Actions.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementBallController : AController
    {
        [Header("Movement Speed Settings")]
        [SerializeField] private float moveForce = 10f;
        [SerializeField] private float maxHorizontalSpeed = 8f;

        [Header("Movement Boost Settings")]
        [SerializeField] private float initialBoostMultiplier = 2f;
        [SerializeField] private float directionChangeMultiplier = 2.5f;
        [SerializeField] private float minSpeedThreshold = 0.5f;

        [Header("Movement Damping Settings")]
        [SerializeField] private float groundDamping = 0.2f;
        [SerializeField] private float airDamping = 0.05f;
        [SerializeField] private float stopDamping = 0.5f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private int maxAirJumps = 1;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private float jumpHoldForce = 5f;
        [SerializeField] private float jumpHoldDuration = 0.25f;
        [SerializeField] private bool useJumpButtonHold = false;
        
        [Header("Ground Detection")]
        [SerializeField] private float groundCheckRadiusMultiplier = 0.8f;
        [SerializeField] private float groundCheckOffset = -.1f;
        [SerializeField] private LayerMask groundLayer;

        // References
        private Rigidbody2D rb;

        // Internal State
        private Vector2 movementInput;

        private bool isGrounded;
        private bool wasGrounded;
        private float lastGroundedTime;

        private int airJumpsLeft;
        
        private bool isJumping;
        private float lastJumpPressedTime;
        private float jumpHoldTimer;
        
        private void Awake()
        {
            base.Awake();
            
            rb = GetComponent<Rigidbody2D>();
            
            // Initialize
            UpdateDamping();
        }

        private void Update()
        {
            CheckGrounded();
            UpdateDamping();
            UpdateJumpVariables();
            ReadInput();
            ProcessJumpBuffer();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyJumpHoldForce();
        }

        private void CheckGrounded()
        {
            wasGrounded = isGrounded;
            
            Vector2 groundCheck = new Vector2(transform.position.x, transform.position.y + groundCheckOffset);
            isGrounded = Physics2D.OverlapCircle(groundCheck, transform.localScale.y / 2 * groundCheckRadiusMultiplier, groundLayer);
            
            // Reset Jump State
            if (isGrounded)
            {
                lastGroundedTime = Time.time;
                airJumpsLeft = maxAirJumps;
                
                // Reset Jumping State
                if (rb.linearVelocity.y <= 0)
                    isJumping = false;
            }
        }
        
        private void UpdateDamping()
        {
            bool isMoving = movementInput.magnitude > Mathf.Epsilon;
            
            if (!isGrounded)
                rb.linearDamping = airDamping;
            else if (!isMoving)
                rb.linearDamping = stopDamping;
            else if (isGrounded)
                rb.linearDamping = groundDamping;
        }

        private void UpdateJumpVariables()
        {
            if (!isJumping)
                return;

            // Update Jump Hold Timer
            jumpHoldTimer -= Time.deltaTime;
            if (jumpHoldTimer <= 0)
                jumpHoldTimer = 0;
        }

        private void ReadInput()
        {
            if (inputProvider == null)
                return;

            // Input
            InputContext input = inputProvider.GetCurrentInput();

            // Movement
            movementInput = Vector2.zero;
            if (Mathf.Abs(input.Movement.x) > 0.01f)
                movementInput = new Vector2(input.Movement.x, 0).normalized;
            
            // Jump Pressed
            if (useJumpButtonHold ? input.IsJumpButtonHeld : input.JumpButtonPressed)
            {
                lastJumpPressedTime = Time.time;                
                TryJump();
            }
            
            // Jump Released
            if (input.JumpButtonReleased && isJumping)
            {
                jumpHoldTimer = 0;
            }
        }

        private void ProcessJumpBuffer()
        {
            float timeSinceJumpPressed = Time.time - lastJumpPressedTime;
            float timeSinceGrounded = Time.time - lastGroundedTime;
            
            bool canCoyoteJump = timeSinceGrounded <= coyoteTime;
            bool hasJumpBuffer = timeSinceJumpPressed <= jumpBufferTime;
            
            if (hasJumpBuffer && canCoyoteJump && !isJumping)
            {
                TryJump();
            }
        }

        private void TryJump()
        {
            // Jump
            float timeSinceGrounded = Time.time - lastGroundedTime;
            if (timeSinceGrounded <= coyoteTime)
            {
                Jump();
                return;
            }
            
            // Air Jump
            if (airJumpsLeft > 0)
            {
                airJumpsLeft--;
                Jump();
                return;
            }
        }

        private void Jump()
        {
            isJumping = true;
            jumpHoldTimer = jumpHoldDuration;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void ApplyJumpHoldForce()
        {
            if (isJumping && jumpHoldTimer > 0 && rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            }
        }

        private void ApplyMovement()
        {
            if (movementInput.magnitude < Mathf.Epsilon)
                return;

            float movementForce = moveForce;

            bool isMinSpeed = Mathf.Abs(rb.linearVelocity.x) > minSpeedThreshold;
            bool changingDirection = Mathf.Sign(movementInput.x) != Mathf.Sign(rb.linearVelocity.x);
            
            // Direction Change
            if (isMinSpeed && changingDirection)
            {
                movementForce *= directionChangeMultiplier;
            }
            
            // Initial Boost
            if (!isMinSpeed)
                movementForce *= initialBoostMultiplier;

            // Main Movement
            rb.AddForce(movementInput * movementForce, ForceMode2D.Force);
            
            // Limit Horizontal Speed
            rb.linearVelocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
                rb.linearVelocity.y
            );
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 groundCheck = new Vector2(transform.position.x, transform.position.y + groundCheckOffset);
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck, transform.localScale.y / 2 * groundCheckRadiusMultiplier);
        }
    }
}