using System;
using System.Collections.Generic;
using System.Linq;
using Game.Terrain.DensityFunctions;
using Game.Terrain.DensityFunctions.Interfaces;
using Game.Terrain.MarchingSquares;
using Game.Terrain.MarchingSquares.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Terrain.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Render Settings")]
        [SerializeField] private int gridWidth = 128;
        [SerializeField] private int gridHeight = 128;
        [SerializeField] private int gridChunkSize = 16;
        [SerializeField] private float gridCellSize = 1f;

        [Header("Density Function Settings")]
        [SerializeReference] private IDensityFunction densityFunction;

        [Header("Collider Settings")]
        [SerializeField] private bool generateColliders = true;
        [SerializeField] private TerrainColliderGenerator.ColliderType colliderType = TerrainColliderGenerator.ColliderType.PolygonCollider;
        [SerializeField] private float pathSimplificationTolerance = 1f;

        [Header("Wall Settings")]
        [SerializeField] private bool generateWalls = true;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private float wallHeight = 3f;
        [SerializeField] private float wallOffset = 0.5f;

        [Header("Performance Settings")]
        [SerializeField] private float updateInterval = 0.2f;

        // References
        private TerrainMeshGenerator meshGenerator;
        private TerrainColliderGenerator colliderGenerator;

        // Internal References
        private List<GameObject> wallObjects = new List<GameObject>();

        public TerrainGrid TerrainGrid { get; private set; }
        private MarchingSquaresGenerator marchingSquaresGenerator;

        // Internal State
        private float lastUpdateTime;
        
        private void LateUpdate()
        {
            if (TerrainGrid.IsDirty && Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateDirtyChunks();
            }
        }

        private void OnDestroy()
        {
            colliderGenerator?.CleanupAllColliders();
        }

        [Button("Regenerate Density Map")]
        public void Initialize()
        {
            Cleanup();
            
            // Texture Density Function
            if (densityFunction is TextureDensityFunction textureDensityFunction)
            {
                gridWidth = textureDensityFunction.Texture.width;
                gridHeight = textureDensityFunction.Texture.height;
            }

            // Create Classes
            TerrainGrid = new TerrainGrid(gridWidth, gridHeight, gridChunkSize, gridCellSize, densityFunction);
            marchingSquaresGenerator = new MarchingSquaresGenerator();

            meshGenerator = new TerrainMeshGenerator(gameObject);
            colliderGenerator = new TerrainColliderGenerator(gameObject, pathSimplificationTolerance);
            
            // Walls
            if (generateWalls)
                GenerateWalls();
        }
        
        public void Cleanup()
        {
            // Clear Mesh
            meshGenerator?.Cleanup();
            
            // Clear Colliders
            colliderGenerator?.CleanupAllColliders();
            
            // Clear Walls
            ClearWalls();
        }

        private void UpdateDirtyChunks()
        {
            // Process Dirty Chunks
            foreach (Vector2Int chunkCoord in TerrainGrid.DirtyChunks)
            {
                GenerateChunk(chunkCoord);
            }
            
            // Combine Chunk Meshes
            meshGenerator.CombineChunkMeshes();
            
            // Clear Dirty Chunks
            Debug.Log($"Updated {TerrainGrid.DirtyChunks.Count} terrain chunks.");
            TerrainGrid.ClearDirtyChunks();

            lastUpdateTime = Time.time;
        }
        
        private void GenerateChunk(Vector2Int chunkCoord)
        {
            // Bounds
            (Vector2Int startGridCoord, Vector2Int endGridCoord) = TerrainGrid.GetChunkBounds(chunkCoord);
            
            // Triangles and Edges
            (List<Triangle> triangles, HashSet<Edge> edges) = marchingSquaresGenerator.Execute(
                startGridCoord, endGridCoord,
                TerrainGrid.CellSize,
                TerrainGrid.GetDensity
            );
            
            // Empty Mesh
            if (triangles.Count == 0)
            {
                // Clear Colliders
                if (generateColliders)
                    colliderGenerator.ClearChunkColliders(chunkCoord);
                    
                return;
            }
            
            // Generate Colliders
            if (generateColliders && edges.Count > 0)
            {
                colliderGenerator.GenerateColliders(chunkCoord, edges, colliderType);
            }
            
            // Generate Mesh
            meshGenerator.GenerateChunkMesh(chunkCoord, triangles);
        }
        
        public void SetDensityFunction(IDensityFunction newDensityFunction)
        {
            densityFunction = newDensityFunction;
        }

        #region Wall Functions

        private void GenerateWalls()
        {
            ClearWalls();
            
            if (!generateWalls || wallPrefab == null)
                return;

            float terrainWidth = TerrainGrid.Width * TerrainGrid.CellSize;
            float terrainHeight = TerrainGrid.Height * TerrainGrid.CellSize;

            void CreateWall(Vector2 position, Vector2 scale, float rotationZ)
            {
                GameObject wall = Instantiate(wallPrefab, transform);
                wall.transform.localPosition = position;
                wall.transform.localScale = scale;
                wall.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
                wallObjects.Add(wall);
            }

            // Bottom wall
            CreateWall(new Vector2(terrainWidth / 2, -wallOffset), 
                       new Vector2(terrainWidth, wallHeight), 0);
            
            // Top wall
            CreateWall(new Vector2(terrainWidth / 2, terrainHeight + wallOffset), 
                       new Vector2(terrainWidth, wallHeight), 0);
            
            // Left wall
            CreateWall(new Vector2(-wallOffset, terrainHeight / 2), 
                       new Vector2(wallHeight, terrainHeight), 0);
            
            // Right wall
            CreateWall(new Vector2(terrainWidth + wallOffset, terrainHeight / 2), 
                       new Vector2(wallHeight, terrainHeight), 0);                       
        }
                
        private void ClearWalls()
        {
            foreach (var wall in wallObjects)
                Destroy(wall);

            wallObjects.Clear();
        }

        #endregion
    }
}