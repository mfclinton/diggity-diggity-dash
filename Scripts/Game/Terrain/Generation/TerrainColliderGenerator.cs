using System.Collections.Generic;
using System.Linq;
using Game.Terrain.MarchingSquares.Data;
using UnityEngine;

namespace Game.Terrain.Generation
{
    public class TerrainColliderGenerator
    {
        public enum ColliderType
        {
            EdgeCollider,
            PolygonCollider
        }

        // Internal References        
        private readonly GameObject targetObject;

        // Collider Tracking
        private Dictionary<Vector2Int, List<Collider2D>> chunkColliders = new Dictionary<Vector2Int, List<Collider2D>>();
        
        // Settings
        private readonly float pathSimplificationTolerance;

        // Constants
        private const int MIN_POINTS_FOR_COLLIDER = 3;

        public TerrainColliderGenerator(GameObject targetObject, float pathSimplificationTolerance = 1f)
        {
            this.targetObject = targetObject;
            this.pathSimplificationTolerance = pathSimplificationTolerance;
        }

        public void GenerateColliders(Vector2Int chunkCoord, HashSet<Edge> edges, ColliderType colliderType)
        {
            // Clear Existing Colliders
            ClearChunkColliders(chunkCoord);
            
            // No Edges
            if (edges == null || edges.Count == 0)
                return;

            // Get Path
            List<List<Vector2>> paths = ProcessEdgePaths(edges);
            if (paths.Count == 0)
                return;
            
            // Create Colliders
            List<Collider2D> newColliders = new List<Collider2D>();
            if (colliderType == ColliderType.EdgeCollider)
                newColliders.AddRange(CreateEdgeColliders(paths));
            else
                newColliders.Add(CreatePolygonCollider(paths));
            
            // Store Colliders
            if (newColliders.Count > 0)
                chunkColliders[chunkCoord] = newColliders;
        }
        
        public void ClearChunkColliders(Vector2Int chunkCoord)
        {
            if (chunkColliders.TryGetValue(chunkCoord, out List<Collider2D> existingColliders))
            {
                foreach (var collider in existingColliders)
                {
                    if (collider != null)
                    {
                        Object.Destroy(collider);
                    }
                }
                
                chunkColliders.Remove(chunkCoord);
            }
        }
        
        public void CleanupAllColliders()
        {
            foreach (var colliderList in chunkColliders.Values)
            {
                foreach (var collider in colliderList)
                {
                    if (collider != null)
                        Object.Destroy(collider);
                }
            }
            
            chunkColliders.Clear();
        }
        
        #region Collider Functions

        private List<EdgeCollider2D> CreateEdgeColliders(List<List<Vector2>> paths)
        {
            List<EdgeCollider2D> colliders = new List<EdgeCollider2D>();
            
            for (int i = 0; i < paths.Count; i++)
            {
                EdgeCollider2D collider = targetObject.AddComponent<EdgeCollider2D>();
                collider.points = paths[i].ToArray();
                colliders.Add(collider);
            }
            
            return colliders;
        }
                
        private PolygonCollider2D CreatePolygonCollider(List<List<Vector2>> paths)
        {
            PolygonCollider2D collider = targetObject.AddComponent<PolygonCollider2D>();
                
            collider.pathCount = paths.Count;
            for (int i = 0; i < paths.Count; i++)
                collider.SetPath(i, paths[i].ToArray());
            
            return collider;
        }

        #endregion

        #region Path and Edge Functions

        private List<List<Vector2>> ProcessEdgePaths(HashSet<Edge> edges)
        {
            List<List<Vector2>> paths = ConnectEdgesIntoPaths(edges);
            
            paths.Sort((a, b) => b.Count.CompareTo(a.Count));
            
            for (int i = 0; i < paths.Count; i++)
                paths[i] = SimplifyPath(paths[i]);
            
            return paths.Where(p => p.Count >= MIN_POINTS_FOR_COLLIDER).ToList();
        }

        private List<List<Vector2>> ConnectEdgesIntoPaths(HashSet<Edge> edges)
        {
            List<List<Vector2>> paths = new List<List<Vector2>>();
            HashSet<Edge> remainingEdges = new HashSet<Edge>(edges);
            
            while (remainingEdges.Count > 0)
            {
                var path = CreatePathFromEdges(remainingEdges);
                paths.Add(path);
            }
            
            return paths;
        }

        private List<Vector2> CreatePathFromEdges(HashSet<Edge> remainingEdges)
        {
            List<Vector2> path = new List<Vector2>();
            
            Edge firstEdge = remainingEdges.First();
            remainingEdges.Remove(firstEdge);
            
            path.Add(firstEdge.Start);
            path.Add(firstEdge.End);
            
            Vector2 pathStart = firstEdge.Start;
            Vector2 pathEnd = firstEdge.End;
            
            bool extendedPath;
            do
            {
                extendedPath = TryExtendPath(remainingEdges, ref path, ref pathStart, ref pathEnd);
            }
            while (extendedPath);
            
            if (ArePointsEqual(pathStart, pathEnd))
                path.RemoveAt(path.Count - 1);
            
            return path;
        }

        private bool TryExtendPath(HashSet<Edge> remainingEdges, ref List<Vector2> path, 
                                  ref Vector2 pathStart, ref Vector2 pathEnd)
        {
            foreach (Edge edge in remainingEdges)
            {
                if (TryConnectEdgeToPath(edge, ref path, ref pathStart, ref pathEnd))
                {
                    remainingEdges.Remove(edge);
                    return true;
                }
            }
            
            return false;
        }

        private bool TryConnectEdgeToPath(Edge edge, ref List<Vector2> path, 
                                         ref Vector2 pathStart, ref Vector2 pathEnd)
        {
            if (ArePointsEqual(pathEnd, edge.Start))
            {
                path.Add(edge.End);
                pathEnd = edge.End;
                return true;
            }
            
            if (ArePointsEqual(pathEnd, edge.End))
            {
                path.Add(edge.Start);
                pathEnd = edge.Start;
                return true;
            }
            
            if (ArePointsEqual(pathStart, edge.Start))
            {
                path.Insert(0, edge.End);
                pathStart = edge.End;
                return true;
            }
            
            if (ArePointsEqual(pathStart, edge.End))
            {
                path.Insert(0, edge.Start);
                pathStart = edge.Start;
                return true;
            }
            
            return false;
        }
        
        private List<Vector2> SimplifyPath(List<Vector2> path)
        {
            if (path.Count <= 2)
                return path;
            
            List<Vector2> result = new List<Vector2>();
            result.Add(path[0]);
            
            for (int i = 1; i < path.Count - 1; i++)
            {
                // Surrounding Points
                Vector2 prev = path[i - 1];
                Vector2 current = path[i];
                Vector2 next = path[i + 1];
                
                // Tangents
                Vector2 dirA = (current - prev).normalized;
                Vector2 dirB = (next - current).normalized;
                
                // Similarity Check
                if (Vector2.Dot(dirA, dirB) < pathSimplificationTolerance)
                {
                    result.Add(current);
                }
            }
            
            result.Add(path[path.Count - 1]);
            return result;
        }

        private bool ArePointsEqual(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b) < Mathf.Epsilon;
        }

        #endregion
    }
}