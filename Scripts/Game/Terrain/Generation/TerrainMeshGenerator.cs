using System.Collections.Generic;
using System.Linq;
using Game.Terrain.MarchingSquares.Data;
using UnityEngine;

namespace Game.Terrain.Generation
{
    public class TerrainMeshGenerator
    {
        private readonly MeshFilter meshFilter;
        private readonly Mesh mainMesh;
        private readonly Dictionary<Vector2Int, Mesh> chunkMeshes = new Dictionary<Vector2Int, Mesh>();

        public TerrainMeshGenerator(GameObject terrainObject)
        {
            meshFilter = terrainObject.GetComponent<MeshFilter>();
            
            // Create main mesh
            mainMesh = new Mesh();
            meshFilter.sharedMesh = mainMesh;
        }
        
        public void Cleanup()
        {
            // Clear Mesh
            Object.Destroy(mainMesh);
            
            // Clear Chunk Meshes
            foreach (var chunkMesh in chunkMeshes.Values)
                Object.Destroy(chunkMesh);
        }

        public Mesh GenerateChunkMesh(Vector2Int chunkCoord, List<Triangle> triangles)
        {
            // Empty Mesh
            if (triangles.Count == 0)
            {
                // Clear Mesh
                if (chunkMeshes.ContainsKey(chunkCoord))
                    chunkMeshes[chunkCoord].Clear();
                    
                return null;
            }
                            
            // Get or Create Chunk Mesh
            Mesh chunkMesh;
            if (!chunkMeshes.TryGetValue(chunkCoord, out chunkMesh))
            {
                chunkMesh = new Mesh();
                chunkMeshes[chunkCoord] = chunkMesh;
            }
            
            // Update Mesh Data
            CreateMeshFromTriangles(chunkMesh, triangles);
            
            return chunkMesh;
        }
        
        private void CreateMeshFromTriangles(Mesh targetMesh, List<Triangle> triangles)
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();
            List<Color> vertexColorsList = new List<Color>();

            // Helper Function to Add Vertex
            Dictionary<Vector3, int> vertexCache = new Dictionary<Vector3, int>();
            void AddVertex(Vector3 vertex, int caseIndex)
            {
                if (!vertexCache.TryGetValue(vertex, out int index))
                {
                    // Add Vertex
                    index = verticesList.Count;
                    vertexCache[vertex] = index;
                    verticesList.Add(vertex);

                    // Add Vertex Color
                    Color vertexColor = Color.black;
                    if (caseIndex == 15)
                        vertexColor = Color.white;
                
                    vertexColorsList.Add(vertexColor);
                }

                trianglesList.Add(index);
            }

            // Add Triangle Vertices
            foreach (var triangle in triangles)
            {
                AddVertex(triangle.VertexA, triangle.CaseIndex);
                AddVertex(triangle.VertexB, triangle.CaseIndex);
                AddVertex(triangle.VertexC, triangle.CaseIndex);
            }

            // Assign to Mesh
            targetMesh.Clear();
            targetMesh.vertices = verticesList.ToArray();
            targetMesh.triangles = trianglesList.ToArray();
            targetMesh.colors = vertexColorsList.ToArray();
            targetMesh.RecalculateNormals();
        }
        
        public void CombineChunkMeshes()
        {
            // No Chunks
            if (chunkMeshes.Count == 0)
                return;
            
            // Valid Meshes
            var validMeshes = chunkMeshes.Values.Where(m => m != null && m.vertexCount > 0).ToArray();
            if (validMeshes.Length == 0)
                return;
            
            // Create Combine Instances
            var combineInstances = validMeshes.Select(m => new CombineInstance 
            { 
                mesh = m, 
                transform = Matrix4x4.identity 
            }).ToArray();
            
            // Merge Meshes
            mainMesh.Clear();
            mainMesh.CombineMeshes(combineInstances, true, true);
            mainMesh.RecalculateNormals();
        }
    }
}