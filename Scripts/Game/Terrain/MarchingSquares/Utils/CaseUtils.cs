using System.Collections.Generic;
using Game.Terrain.MarchingSquares.Data;

namespace Game.Terrain.MarchingSquares.Utils
{
    public static class CaseUtils
    {
        public static int GetCaseIndex(float isoLevel, float bottomLeft, float bottomRight, float topRight, float topLeft)
        {
            int caseIndex = 0;

            if (bottomLeft > isoLevel) caseIndex |= 1;
            if (bottomRight > isoLevel) caseIndex |= 2;
            if (topRight > isoLevel) caseIndex |= 4;
            if (topLeft > isoLevel) caseIndex |= 8;
                
            return caseIndex;
        }
        
        public static void GenerateTrianglesForCell(List<Triangle> triangles, ref CellData cell)
        {
            switch (cell.CaseIndex)
            {
                case 0: // All points are outside
                    break;
                case 1: // Only Bottom-Left inside
                    triangles.Add(new Triangle(cell.BottomLeft, cell.MidBottom, cell.MidLeft, cell.CaseIndex));
                    break;
                case 2: // Only Bottom-Right inside
                    triangles.Add(new Triangle(cell.BottomRight, cell.MidRight, cell.MidBottom, cell.CaseIndex));
                    break;
                case 3: // Bottom Edge inside
                    triangles.Add(new Triangle(cell.BottomLeft, cell.BottomRight, cell.MidRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomLeft, cell.MidRight, cell.MidLeft, cell.CaseIndex));
                    break;
                case 4: // Only Top-Right inside
                    triangles.Add(new Triangle(cell.TopRight, cell.MidTop, cell.MidRight, cell.CaseIndex));
                    break;
                case 5: // Diagonal split (Bottom-Left and Top-Right inside)
                    triangles.Add(new Triangle(cell.BottomLeft, cell.MidBottom, cell.MidLeft, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopRight, cell.MidTop, cell.MidRight, cell.CaseIndex));
                    break;
                case 6: // Right Edge inside
                    triangles.Add(new Triangle(cell.BottomRight, cell.TopRight, cell.MidTop, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomRight, cell.MidTop, cell.MidBottom, cell.CaseIndex));
                    break;
                case 7: // All except Top-Left inside
                    triangles.Add(new Triangle(cell.BottomLeft, cell.BottomRight, cell.TopRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomLeft, cell.TopRight, cell.MidTop, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomLeft, cell.MidTop, cell.MidLeft, cell.CaseIndex));
                    break;
                case 8: // Only Top-Left inside
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidLeft, cell.MidTop, cell.CaseIndex));
                    break;
                case 9: // Left Edge inside
                    triangles.Add(new Triangle(cell.BottomLeft, cell.TopLeft, cell.MidTop, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomLeft, cell.MidTop, cell.MidBottom, cell.CaseIndex));
                    break;
                case 10: // Diagonal split (Bottom-Right and Top-Left inside)
                    triangles.Add(new Triangle(cell.BottomRight, cell.MidRight, cell.MidBottom, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidTop, cell.MidLeft, cell.CaseIndex));
                    break;
                case 11: // All except Top-Right inside
                    triangles.Add(new Triangle(cell.TopLeft, cell.BottomLeft, cell.BottomRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.BottomRight, cell.MidRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidRight, cell.MidTop, cell.CaseIndex));
                    break;
                case 12: // Top Edge inside
                    triangles.Add(new Triangle(cell.TopLeft, cell.TopRight, cell.MidRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidRight, cell.MidLeft, cell.CaseIndex));
                    break;
                case 13: // All except Bottom-Right inside
                    triangles.Add(new Triangle(cell.TopLeft, cell.TopRight, cell.MidRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidRight, cell.MidBottom, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopLeft, cell.MidBottom, cell.BottomLeft, cell.CaseIndex));
                    break;
                case 14: // All except Bottom-Left inside
                    triangles.Add(new Triangle(cell.TopRight, cell.TopLeft, cell.MidLeft, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopRight, cell.MidLeft, cell.MidBottom, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.TopRight, cell.MidBottom, cell.BottomRight, cell.CaseIndex));
                    break;
                case 15: // All points inside
                    triangles.Add(new Triangle(cell.BottomLeft, cell.BottomRight, cell.TopRight, cell.CaseIndex));
                    triangles.Add(new Triangle(cell.BottomLeft, cell.TopRight, cell.TopLeft, cell.CaseIndex));
                    break;
            }
        }

        public static void GenerateEdgesForCell(HashSet<Edge> edges, ref CellData cell)
        {
            switch (cell.CaseIndex)
            {
                case 0: // All points are outside
                    break;
                case 1: // Only Bottom-Left inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    break;
                case 2: // Only Bottom-Right inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    break;
                case 3: // Bottom Edge inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidRight));
                    break;
                case 4: // Only Top-Right inside
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 5: // Diagonal split (Bottom-Left and Top-Right inside)
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 6: // Right Edge inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidTop));
                    break;
                case 7: // All except Top-Left inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 8: // Only Top-Left inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 9: // Left Edge inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidTop));
                    break;
                case 10: // Diagonal split (Bottom-Right and Top-Left inside)
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 11: // All except Top-Right inside
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 12: // Top Edge inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidRight));
                    break;
                case 13: // All except Bottom-Right inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    break;
                case 14: // All except Bottom-Left inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    break;
                case 15: // All points inside
                    break;
            }
        }

        public static void GenerateEdgesForBorder(HashSet<Edge> edges, ref CellData cell, bool isLeftBorder = false, bool isBottomBorder = false, bool isRightBorder = false, bool isTopBorder = false)
        {
            // Internal cell edges (existing logic)
            switch (cell.CaseIndex)
            {
                case 0: // All points are outside
                    break;
                case 1: // Only Bottom-Left inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    break;
                case 2: // Only Bottom-Right inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    break;
                case 3: // Bottom Edge inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidRight));
                    break;
                case 4: // Only Top-Right inside
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 5: // Diagonal split (Bottom-Left and Top-Right inside)
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 6: // Right Edge inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidTop));
                    break;
                case 7: // All except Top-Left inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 8: // Only Top-Left inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 9: // Left Edge inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidTop));
                    break;
                case 10: // Diagonal split (Bottom-Right and Top-Left inside)
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    edges.Add(new Edge(cell.MidLeft, cell.MidTop));
                    break;
                case 11: // All except Top-Right inside
                    edges.Add(new Edge(cell.MidRight, cell.MidTop));
                    break;
                case 12: // Top Edge inside
                    edges.Add(new Edge(cell.MidLeft, cell.MidRight));
                    break;
                case 13: // All except Bottom-Right inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidRight));
                    break;
                case 14: // All except Bottom-Left inside
                    edges.Add(new Edge(cell.MidBottom, cell.MidLeft));
                    break;
                case 15: // All points inside
                    break;
            }
            
            // Handle world border edges
            // Left border
            if (isLeftBorder)
            {
                if ((cell.CaseIndex & 1) != 0 && (cell.CaseIndex & 8) != 0) // Bottom-left and top-left are inside
                {
                    edges.Add(new Edge(cell.BottomLeft, cell.TopLeft));
                }
                else if ((cell.CaseIndex & 1) != 0 && (cell.CaseIndex & 8) == 0) // Only bottom-left is inside
                {
                    edges.Add(new Edge(cell.BottomLeft, cell.MidLeft));
                }
                else if ((cell.CaseIndex & 1) == 0 && (cell.CaseIndex & 8) != 0) // Only top-left is inside
                {
                    edges.Add(new Edge(cell.MidLeft, cell.TopLeft));
                }
            }
            
            // Bottom border
            if (isBottomBorder)
            {
                if ((cell.CaseIndex & 1) != 0 && (cell.CaseIndex & 2) != 0) // Bottom-left and bottom-right are inside
                {
                    edges.Add(new Edge(cell.BottomLeft, cell.BottomRight));
                }
                else if ((cell.CaseIndex & 1) != 0 && (cell.CaseIndex & 2) == 0) // Only bottom-left is inside
                {
                    edges.Add(new Edge(cell.BottomLeft, cell.MidBottom));
                }
                else if ((cell.CaseIndex & 1) == 0 && (cell.CaseIndex & 2) != 0) // Only bottom-right is inside
                {
                    edges.Add(new Edge(cell.MidBottom, cell.BottomRight));
                }
            }
            
            // Right border
            if (isRightBorder)
            {
                if ((cell.CaseIndex & 2) != 0 && (cell.CaseIndex & 4) != 0) // Bottom-right and top-right are inside
                {
                    edges.Add(new Edge(cell.BottomRight, cell.TopRight));
                }
                else if ((cell.CaseIndex & 2) != 0 && (cell.CaseIndex & 4) == 0) // Only bottom-right is inside
                {
                    edges.Add(new Edge(cell.BottomRight, cell.MidRight));
                }
                else if ((cell.CaseIndex & 2) == 0 && (cell.CaseIndex & 4) != 0) // Only top-right is inside
                {
                    edges.Add(new Edge(cell.MidRight, cell.TopRight));
                }
            }
            
            // Top border
            if (isTopBorder)
            {
                if ((cell.CaseIndex & 8) != 0 && (cell.CaseIndex & 4) != 0) // Top-left and top-right are inside
                {
                    edges.Add(new Edge(cell.TopLeft, cell.TopRight));
                }
                else if ((cell.CaseIndex & 8) != 0 && (cell.CaseIndex & 4) == 0) // Only top-left is inside
                {
                    edges.Add(new Edge(cell.TopLeft, cell.MidTop));
                }
                else if ((cell.CaseIndex & 8) == 0 && (cell.CaseIndex & 4) != 0) // Only top-right is inside
                {
                    edges.Add(new Edge(cell.MidTop, cell.TopRight));
                }
            }
        }
    }
}