using Game.Terrain.Generation;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Pathing;

namespace Game.Input.AIInput
{
    [RequireComponent(typeof(AIInputProvider))]
    public class AIInputBrain : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform agentTransform;

        [Header("Decision Parameters")]
        [SerializeField] private float decisionInterval = 0.5f;
        [SerializeField] private float pathfindingInterval = 2.0f;
        [SerializeField] private float targetDistanceThreshold = 5f;
        [SerializeField] private float heightJumpThreshold = 0.5f;
        
        [Header("Pathfinding Parameters")]
        [SerializeField] private float densityCostMultiplier = 5.0f;
        
        [Header("Debug")]
        [SerializeField] private bool showPathGizmos = true;
        [SerializeField] private Color pathColor = Color.yellow;
        [SerializeField] private Color targetPointColor = Color.red;
        [SerializeField] private float gizmoSize = 0.5f;

        // Internal References
        private AIInputProvider inputProvider;
        private Coroutine decisionCoroutine;
        private TerrainGenerator terrainGenerator;
        
        // Movement State
        private bool isJumping = false;
        private bool isDigging = false;
        private Vector2 currentMoveDirection;
        
        // Pathing Variables
        private Vector2Int targetGridPosition;
        private List<Vector2> pathPoints = new List<Vector2>();
        private Vector2? currentTargetPoint = null;
        private float pathfindingTimer = 0f;

        private void Awake()
        {
            inputProvider = GetComponent<AIInputProvider>();
            terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
        }
        
        private void Start()
        {
            decisionCoroutine = StartCoroutine(MakeDecisions());
            SetTargetToRightEdge();
        }
        
        private void Update()
        {
            // Periodically update pathing
            pathfindingTimer -= Time.deltaTime;
            if (pathfindingTimer <= 0)
            {
                UpdatePath();
                pathfindingTimer = pathfindingInterval;
            }
        }

        private void SetTargetToRightEdge()
        {
            Vector2Int gridSize = new Vector2Int(
                terrainGenerator.TerrainGrid.Width,
                terrainGenerator.TerrainGrid.Height
            );
            
            targetGridPosition = new Vector2Int(gridSize.x - 1, gridSize.y / 2);
        }
        
        private void UpdatePath()
        {            
            // Get Grid Position
            Vector2 currentWorldPosition = agentTransform.position;
            Vector2Int currentGridPosition = terrainGenerator.TerrainGrid.WorldToGrid(currentWorldPosition);
            
            // Clear Path
            pathPoints.Clear();
            
            // Perform A* pathfinding
            List<Vector2Int> gridPath = AStar.FindPath(
                terrainGenerator.TerrainGrid.DensityMap,
                currentGridPosition,
                targetGridPosition,
                densityCostMultiplier
            );
            
            // Set Path
            foreach (var gridPos in gridPath)
            {
                pathPoints.Add(terrainGenerator.TerrainGrid.GridToWorld(gridPos));
            }
                        
            // Set Target Point
            if (pathPoints.Count > 0)
                currentTargetPoint = pathPoints[0];
            else
                currentTargetPoint = null;
        }
        
        private IEnumerator MakeDecisions()
        {
            while (true)
            {
                // Make AI Decisions
                DecideMovement();
                DecideJump();
                DecideDig();
                
                // Update Input Provider
                UpdateInputProvider();
                
                // Wait
                yield return new WaitForSeconds(decisionInterval);
            }
        }
        
        private void DecideMovement()
        {
            if (!currentTargetPoint.HasValue)
            {
                currentMoveDirection = Vector2.zero;
                return;
            }

            Vector2 currentPosition = agentTransform.position;
            Vector2 targetPosition = currentTargetPoint.Value;

            // Skip to Next Point if Close Enough
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
            
            bool shouldAdvanceToNextPoint = false;
            if (distanceToTarget < targetDistanceThreshold)
                shouldAdvanceToNextPoint = true;
            
            if (shouldAdvanceToNextPoint && pathPoints.Count > 0)
            {
                pathPoints.RemoveAt(0);
                if (pathPoints.Count > 0)
                {
                    currentTargetPoint = pathPoints[0];
                    targetPosition = currentTargetPoint.Value;
                }
                else
                {
                    currentTargetPoint = null;
                    currentMoveDirection = Vector2.zero;
                    return;
                }
            }
            
            // Move Towards Target
            Vector2 directionToTarget = (targetPosition - currentPosition).normalized;
            currentMoveDirection = directionToTarget;
        }
        
        private void DecideJump()
        {
            isJumping = false;
            if (!currentTargetPoint.HasValue)
                return;
            
            Vector2 currentPosition = agentTransform.position;
            Vector2 targetPosition = currentTargetPoint.Value;

            if (targetPosition.y > currentPosition.y + heightJumpThreshold)
            {
                isJumping = true;
            }
        }
        
        private void DecideDig()
        {                
            isDigging = true;
            if (Random.value < 0.5f)
                isDigging = false;
        }
        
        private float GetDensityAtWorldPosition(Vector2 worldPosition)
        {
            Vector2 gridPosition = terrainGenerator.TerrainGrid.WorldToGrid(worldPosition);

            int x = Mathf.RoundToInt(gridPosition.x);
            int y = Mathf.RoundToInt(gridPosition.y);
            
            return terrainGenerator.TerrainGrid.GetDensity(x, y);
        }
        
        private void UpdateInputProvider()
        {
            // Update Movement
            inputProvider.SetMovementDirection(currentMoveDirection);
            
            // Update Jump
            if (isJumping)
            {
                inputProvider.TriggerJump();
            }
            else
            {
                inputProvider.ReleaseJump();
            }
            
            // Update Digging
            if (isDigging)
            {
                inputProvider.TriggerDig();
            }
            else
            {
                inputProvider.ReleaseDig();
            }
        }        
        
        #region Debugging

        private void DebugPath()
        {
            if (!showPathGizmos || pathPoints == null || pathPoints.Count == 0)
                return;
                
            Gizmos.color = pathColor;
            
            // Draw Full Path
            for (int i = 0; i < pathPoints.Count - 1; i++)
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            
            // Draw Path to Current Target Point
            if (currentTargetPoint.HasValue && agentTransform != null)
                Gizmos.DrawLine(agentTransform.position, currentTargetPoint.Value);
            
            // Draw Each Path Point
            foreach (Vector2 point in pathPoints)
                Gizmos.DrawSphere(point, gizmoSize);
            
            // Highlight the Current Target Point
            if (currentTargetPoint.HasValue)
                Gizmos.color = targetPointColor;
                Gizmos.DrawSphere(currentTargetPoint.Value, gizmoSize * 1.5f);
            
            // Draw Final Destination
            if (terrainGenerator != null)
            {
                Vector2 finalDestination = terrainGenerator.TerrainGrid.GridToWorld(targetGridPosition);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(finalDestination, gizmoSize * 2f);
            }
        }

        private void OnDrawGizmos()
        {
            DebugPath();
        }

        #endregion
    }
}