using Game.Input.Data;
using Game.Input.Data.Interfaces;
using UnityEngine;

namespace Game.Input.AIInput
{
    public class AIInputProvider : MonoBehaviour, IInputProvider
    {
        // Input Context
        private InputContext currentInput = new InputContext();
        public InputContext GetCurrentInput()
        {
            return currentInput;
        }

        private void LateUpdate()
        {
            currentInput.Reset();
        }
        
        #region Actions

        public void SetMovementDirection(Vector2 direction)
        {
            currentInput.Movement = direction;
        }
        
        public void TriggerJump()
        {
            currentInput.JumpButtonPressed = true;
            currentInput.IsJumpButtonHeld = true;
        }
        
        public void ReleaseJump()
        {
            currentInput.JumpButtonReleased = true;
            currentInput.IsJumpButtonHeld = false;
        }
        
        public void TriggerDig()
        {
            currentInput.IsDigButtonPressed = true;
            currentInput.IsDigButtonHeld = true;
        }
        
        public void ReleaseDig()
        {
            currentInput.IsDigButtonReleased = true;
            currentInput.IsDigButtonHeld = false;
        }

        #endregion
    }
}