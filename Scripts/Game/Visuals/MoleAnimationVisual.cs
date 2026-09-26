using UnityEngine;
using Game.Actions.Digging;

namespace Game.Visuals
{
    [RequireComponent(typeof(Animator))]
    public class MoleAnimationVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rigidBody2D;
        [SerializeField] private DiggingController diggingController;

        [Header("Animation Settings")]
        [SerializeField] private float flipThreshold = 0.1f;

        // Constants
        private const string speedParameterName = "Speed";
        private const string diggingParameterName = "Digging";

        // References
        private Animator animator;
                
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            UpdateAnimationParameters();
            UpdateSpriteFlipping();
        }
        
        private void UpdateAnimationParameters()
        {
            animator.SetFloat(speedParameterName, rigidBody2D.linearVelocity.magnitude);
            animator.SetBool(diggingParameterName, diggingController.IsDigging);
        }

        private void UpdateSpriteFlipping()
        {
            if (Mathf.Abs(rigidBody2D.linearVelocity.x) < flipThreshold)
                return;
            
            // Flip
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(rigidBody2D.linearVelocity.x) * Mathf.Abs(scale.x);

            transform.localScale = scale;
        }
    }
}