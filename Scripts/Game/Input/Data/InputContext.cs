using UnityEngine;

namespace Game.Input.Data
{
    public class InputContext
    {
        // Movement Inputs
        public Vector2 Movement { get; set; }
        
        // Button States
        public bool JumpButtonPressed { get; set; }
        public bool JumpButtonReleased { get; set; }
        public bool IsJumpButtonHeld { get; set; }
        
        public bool IsDigButtonPressed { get; set; }
        public bool IsDigButtonReleased { get; set; }
        public bool IsDigButtonHeld { get; set; }
        
        public void Reset()
        {
            // Reset Button States
            JumpButtonPressed = false;
            JumpButtonReleased = false;
            
            IsDigButtonPressed = false;
            IsDigButtonReleased = false;
        }
    }
}