using System;
using System.Collections.Generic;
using Game.Actions.Digging.Stencils.Interfaces;
using Game.Terrain;
using UnityEngine;

namespace Game.Actions.Digging.Stencils
{
    [Serializable]
    public class CircularDiggingStencil : IDiggingStencil
    {
        [Header("Digging Offsets")]
        [SerializeField] private float lookAheadDistance = 0.3f;
        [SerializeField] private float verticalOffset = 0.25f;

        [Header("Digging Area")]
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float minStrength = 0.05f;
        [SerializeField] private float maxStrength = 1.0f;
        
        public List<DigCell> GetAffectedCells(
            TerrainGrid terrainGrid, 
            Vector2 origin, 
            Vector2 direction)
        {
            List<DigCell> affectedCells = new List<DigCell>();
            
            // Calculate Dig Position
            Vector2 digWorldPosition = origin + (direction * lookAheadDistance);
            digWorldPosition.y += verticalOffset;

            Vector2Int digCellPosition = terrainGrid.WorldToGrid(digWorldPosition);
            
            // Calculate Affected Cells
            int cellRadius = Mathf.CeilToInt(radius / terrainGrid.CellSize);
            for (int xOffset = -cellRadius; xOffset <= cellRadius; xOffset++)
            {
                for (int yOffset = -cellRadius; yOffset <= cellRadius; yOffset++)
                {
                    Vector2Int cellPos = new Vector2Int(digCellPosition.x + xOffset, digCellPosition.y + yOffset);
                    
                    // Out of Bounds
                    if (cellPos.x < 0 || cellPos.x >= terrainGrid.Width ||
                        cellPos.y < 0 || cellPos.y >= terrainGrid.Height)
                        continue;
                        
                    // Already Digged
                    if (terrainGrid.GetDensity(cellPos.x, cellPos.y) <= 0)
                        continue;
                    
                    // Calculate Strength
                    Vector2 cellWorldPos = terrainGrid.GridToWorld(cellPos);

                    float distance = Vector2.Distance(digWorldPosition, cellWorldPos);
                    float normalizedDistance = distance / radius;

                    float strength = Mathf.Lerp(maxStrength, minStrength, normalizedDistance);
                    
                    // Add Affected Cell
                    if (strength > 0)
                        affectedCells.Add(new DigCell(cellPos, strength));
                }
            }
            
            return affectedCells;
        }

        public void DrawPreview(Vector2 origin, Vector2 direction)
        {
            Vector2 digPosition = origin + (direction * lookAheadDistance);
            digPosition.y += verticalOffset;
            
            // Draw Dig Area
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(digPosition, radius);
            
            // Draw Direction Line
            Gizmos.DrawLine(origin, digPosition);
        }
    }
}