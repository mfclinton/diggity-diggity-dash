using UnityEngine;

namespace Game.Visuals
{
    public class VisualFollower : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 positionOffset;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSmoothTime = 0.2f;
        [SerializeField] private float maxRotationDeltaPerFrame = 10f;
        
        [Header("Raycast Settings")]
        [SerializeField] private float raycastDistance = 2f;
        [SerializeField] private LayerMask layerMask;
        
        private float currentAngle = 0f;
        private float currentAngularVelocity = 0f;
        
        private void LateUpdate()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            transform.position = target.position + positionOffset;
        }

        private void UpdateRotation()
        {
            float targetAngle = 0f;
            
            RaycastHit2D hit = Physics2D.Raycast(target.position, Vector2.down, raycastDistance, layerMask);
            if (hit.collider)
            {
                Vector2 surfaceNormal = hit.normal;
                targetAngle = -Mathf.Atan2(surfaceNormal.x, surfaceNormal.y) * Mathf.Rad2Deg;
            }
            
            currentAngle = Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref currentAngularVelocity,
                rotationSmoothTime,
                maxRotationDeltaPerFrame,
                Time.deltaTime
            );
                
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        }
    }
}