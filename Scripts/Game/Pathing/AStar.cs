using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pathing
{
    public static class AStar
    {
        private class Node : IComparable<Node>
        {
            public Vector2Int Position { get; }
            public float GCost { get; set; } // Cost from start
            public float HCost { get; set; } // Heuristic cost to end
            public float FCost => GCost + HCost;
            public Node Parent { get; set; }

            public Node(Vector2Int position)
            {
                Position = position;
            }

            public int CompareTo(Node other)
            {
                int fCostComparison = FCost.CompareTo(other.FCost);
                if (fCostComparison != 0)
                    return fCostComparison;
                    
                return HCost.CompareTo(other.HCost);
            }
        }

        private static readonly Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // N
            new Vector2Int(1, 0),   // E
            new Vector2Int(0, -1),  // S
            new Vector2Int(-1, 0),  // W
        };

        // Cost for moving in different directions
        private static readonly float[] moveCosts = new float[]
        {
            1.0f,       // N
            1.0f,       // E
            1.0f,       // S
            1.0f,       // W
        };

        // Configuration
        private const int maxIterations = 10000;

        public static List<Vector2Int> FindPath(float[,] densityGrid, Vector2Int start, Vector2Int end, float densityCostMultiplier = 0f)
        {
            // Get Dimensions
            int width = densityGrid.GetLength(0);
            int height = densityGrid.GetLength(1);

            // Initialize Data Structures
            SortedSet<Node> openSet = new SortedSet<Node>();
            Dictionary<Vector2Int, Node> allNodes = new Dictionary<Vector2Int, Node>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            // Create Start Node
            Node startNode = new Node(start);
            startNode.GCost = 0;
            startNode.HCost = CalculateHeuristic(start, end);
            openSet.Add(startNode);
            allNodes[start] = startNode;

            // Track Best Node
            Node bestNode = startNode;

            // A* Algorithm
            int iterations = 0;
            while (openSet.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                
                // Pop Node
                Node currentNode = openSet.Min;
                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                // Best Node Check
                if (currentNode.HCost < bestNode.HCost)
                    bestNode = currentNode;

                // Goal Check
                if (currentNode.Position == end)
                    return ReconstructPath(currentNode);

                // Explore Neighbors
                for (int i = 0; i < directions.Length; i++)
                {
                    Vector2Int neighborPos = currentNode.Position + directions[i];

                    // Skip Already Checked Nodes
                    if (closedSet.Contains(neighborPos) || !IsValidPosition(neighborPos.x, neighborPos.y, width, height))
                        continue;

                    // Move Cost
                    float moveCost = moveCosts[i];
                    
                    // Density Cost
                    float density = densityGrid[neighborPos.x, neighborPos.y];
                    float densityCost = density * densityCostMultiplier;
                    
                    // Total Cost
                    float totalMoveCost = moveCost + densityCost;
                    float gCost = currentNode.GCost + totalMoveCost;

                    // Get or Create Neighbor Node
                    if (!allNodes.TryGetValue(neighborPos, out Node neighborNode))
                    {
                        neighborNode = new Node(neighborPos);
                        neighborNode.HCost = CalculateHeuristic(neighborPos, end);
                        allNodes[neighborPos] = neighborNode;
                    }
                    else if (gCost >= neighborNode.GCost) // Cheaper Path Through Neighbor Already Found
                        continue;

                    // Update Neighbor Node
                    neighborNode.Parent = currentNode;
                    neighborNode.GCost = gCost;

                    // Add to Open Set
                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Add(neighborNode);
                    }
                    else
                    {
                        openSet.Remove(neighborNode);
                        openSet.Add(neighborNode);
                    }
                }
            }

            // Partial Path Found
            return ReconstructPath(bestNode);
        }

        private static bool IsValidPosition(int x, int y, int width, int height)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            return true;
        }

        private static float CalculateHeuristic(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
                        
            return dx + dy;
        }

        private static List<Vector2Int> ReconstructPath(Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            
            Node current = endNode;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }

            return SimplifyPath(path);
        }

        private static List<Vector2Int> SimplifyPath(List<Vector2Int> path)
        {
            if (path.Count <= 2)
                return path;

            // Initialize Simplified Path
            List<Vector2Int> simplifiedPath = new List<Vector2Int>();
            simplifiedPath.Add(path[0]);

            // Simplify Original Path
            Vector2Int currentDirection = path[1] - path[0];
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector2Int newDirection = path[i] - path[i-1];
                if (newDirection != currentDirection)
                {
                    simplifiedPath.Add(path[i-1]);
                    currentDirection = newDirection;
                }
            }
            
            // Add End Point
            simplifiedPath.Add(path[path.Count - 1]);
            
            return simplifiedPath;
        }
    }
}