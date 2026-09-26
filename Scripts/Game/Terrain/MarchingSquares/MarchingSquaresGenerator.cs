using System;
using System.Collections.Generic;
using Game.Terrain.MarchingSquares.Data;
using Game.Terrain.MarchingSquares.Utils;
using UnityEngine;

namespace Game.Terrain.MarchingSquares
{
    public class MarchingSquaresGenerator
    {
        private float isoLevel = 0.5f;
        
        public (List<Triangle>, HashSet<Edge>) Execute(Vector2Int startGridCoord, Vector2Int endGridCoord, float cellSize, Func<int, int, float> getDensity)
        {
            List<Triangle> triangles = new List<Triangle>();
            HashSet<Edge> edges = new HashSet<Edge>();
            
            CellData cellData = new CellData();
            for (int x = startGridCoord.x; x <= endGridCoord.x; x++)
            {
                for (int y = startGridCoord.y; y <= endGridCoord.y; y++)
                {
                    UpdateCellData(x, y, cellSize, getDensity, ref cellData);
                    
                    // Outside Case
                    if (cellData.CaseIndex == 0)
                        continue;
                    
                    // Triangle Case
                    CaseUtils.GenerateTrianglesForCell(triangles, ref cellData);
                    
                    // Edge Case
                    CaseUtils.GenerateEdgesForCell(edges, ref cellData);

                    // Edge Border Cases
                    bool isLeftBorder = x == startGridCoord.x;
                    bool isBottomBorder = y == startGridCoord.y;
                    bool isRightBorder = x == endGridCoord.x;
                    bool isTopBorder = y == endGridCoord.y;
                    
                    CaseUtils.GenerateEdgesForBorder(edges, ref cellData, isLeftBorder, isBottomBorder, isRightBorder, isTopBorder);
                }
            }
                
            return (triangles, edges);
        }
        
        private void UpdateCellData(int x, int y, float cellSize, Func<int, int, float> getDensity, ref CellData cell)
        {
            // Sample Density Values
            cell.BottomLeftValue = getDensity(x, y);
            cell.BottomRightValue = getDensity(x + 1, y);
            cell.TopRightValue = getDensity(x + 1, y + 1);
            cell.TopLeftValue = getDensity(x, y + 1);
            
            // Calculate Case Index
            cell.CaseIndex = CaseUtils.GetCaseIndex(isoLevel, cell.BottomLeftValue, cell.BottomRightValue, cell.TopRightValue, cell.TopLeftValue);

            // Outside Case
            if (cell.CaseIndex == 0)
                return;
            
            // Calculate Cell Corners
            cell.BottomLeft = new Vector2(x * cellSize, y * cellSize);
            cell.BottomRight = new Vector2((x + 1) * cellSize, y * cellSize);
            cell.TopRight = new Vector2((x + 1) * cellSize, (y + 1) * cellSize);
            cell.TopLeft = new Vector2(x * cellSize, (y + 1) * cellSize);
            
            // Inside Case
            if (cell.CaseIndex == 15)
                return;
            
            // Calculate Midpoints
            cell.MidBottom = InterpolateVerts_Smooth(cell.BottomLeft, cell.BottomRight, cell.BottomLeftValue, cell.BottomRightValue);
            cell.MidRight = InterpolateVerts_Smooth(cell.BottomRight, cell.TopRight, cell.BottomRightValue, cell.TopRightValue);
            cell.MidTop = InterpolateVerts_Smooth(cell.TopLeft, cell.TopRight, cell.TopLeftValue, cell.TopRightValue);
            cell.MidLeft = InterpolateVerts_Smooth(cell.BottomLeft, cell.TopLeft, cell.BottomLeftValue, cell.TopLeftValue);
        }

        #region Helpers
        
        private Vector2 InterpolateVerts_Smooth(Vector2 vertA, Vector2 vertB, float valueA, float valueB)
        {
            if (Mathf.Abs(valueA - valueB) < Mathf.Epsilon)
                return vertA;
            
            float t = (isoLevel - valueA) / (valueB - valueA);
            return Vector2.Lerp(vertA, vertB, t);
        }

        #endregion
    }
}