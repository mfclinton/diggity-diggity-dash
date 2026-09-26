using Game.Input.Data;
using Game.Input.Data.Interfaces;
using Game.Input.PlayerInput.InputActions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input.PlayerInput
{
    public class PlayerInputProvider : MonoBehaviour, IInputProvider
    {
        // References
        private InputSystem_Actions _inputActions;

        // State
        public InputContext CurrentInputContext { get; } = new();

        // Singleton
        private static PlayerInputProvider _instance;
        public static PlayerInputProvider Instance
        {
            get => _instance ??= FindAnyObjectByType<PlayerInputProvider>();
        }

        #region IInputProvider Implementation

        public InputContext GetCurrentInput()
        {
            return CurrentInputContext;
        }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Initialize
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            SubscribeToInputEvents();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
            UnsubscribeFromInputEvents();
        }

        private void OnDestroy()
        {
            // Clean Up
            if (_instance == this)
                _instance = null;

            _inputActions.Dispose();
        }

        private void LateUpdate()
        {
            CurrentInputContext.Reset();
        }

        #endregion

        #region Initialization

        private void SubscribeToInputEvents()
        {
            // Movement
            _inputActions.Player.Move.performed += OnPlayerMove;
            _inputActions.Player.Move.canceled += OnPlayerMove;

            // Jump
            _inputActions.Player.Jump.performed += OnPlayerJumpPressed;
            _inputActions.Player.Jump.canceled += OnPlayerJumpReleased;

            // Dig
            _inputActions.Player.Dig.performed += OnPlayerDigPressed;
            _inputActions.Player.Dig.canceled += OnPlayerDigReleased;
        }

        private void UnsubscribeFromInputEvents()
        {
            // Movement
            _inputActions.Player.Move.performed -= OnPlayerMove;
            _inputActions.Player.Move.canceled -= OnPlayerMove;

            // Jump
            _inputActions.Player.Jump.performed -= OnPlayerJumpPressed;
            _inputActions.Player.Jump.canceled -= OnPlayerJumpReleased;

            // Dig
            _inputActions.Player.Dig.performed -= OnPlayerDigPressed;
            _inputActions.Player.Dig.canceled -= OnPlayerDigReleased;
        }

        #endregion

        #region Input Event Handlers

        private void OnPlayerMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            CurrentInputContext.Movement = movement;
        }

        private void OnPlayerJumpPressed(InputAction.CallbackContext context)
        {
            CurrentInputContext.JumpButtonPressed = true;
            CurrentInputContext.IsJumpButtonHeld = true;
        }

        private void OnPlayerJumpReleased(InputAction.CallbackContext context)
        {
            CurrentInputContext.JumpButtonReleased = true;
            CurrentInputContext.IsJumpButtonHeld = false;
        }

        private void OnPlayerDigPressed(InputAction.CallbackContext context)
        {
            CurrentInputContext.IsDigButtonPressed = true;
            CurrentInputContext.IsDigButtonHeld = true;
        }

        private void OnPlayerDigReleased(InputAction.CallbackContext context)
        {
            CurrentInputContext.IsDigButtonReleased = true;
            CurrentInputContext.IsDigButtonHeld = false;
        }

        #endregion
    }
}