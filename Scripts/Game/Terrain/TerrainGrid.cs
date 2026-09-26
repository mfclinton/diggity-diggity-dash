using System.Collections.Generic;
using Game.Terrain.DensityFunctions.Interfaces;
using UnityEngine;

namespace Game.Terrain
{
    public class TerrainGrid
    {
        // Configuration
        public int Width { get; }
        public int Height { get; }
        public int ChunkSize { get; }
        public float CellSize { get; }
        
        // Internal State
        private float[,] densityMap;
        public float[,] DensityMap => densityMap;

        private IDensityFunction densityFunction;
        
        // Chunking System
        private HashSet<Vector2Int> dirtyChunks = new HashSet<Vector2Int>();
        public IReadOnlyCollection<Vector2Int> DirtyChunks => dirtyChunks;

        // Public State
        public bool IsDirty => dirtyChunks.Count > 0;
        
        #region Initialization

        public TerrainGrid(int width, int height, int chunkSize, float cellSize, IDensityFunction initialDensityFunction)
        {
            this.Width = width;
            this.Height = height;
            this.ChunkSize = chunkSize;
            this.CellSize = cellSize;
            this.densityFunction = initialDensityFunction;
            
            densityMap = new float[Width, Height];
            RegenerateDensityMap();
        }
        
        #endregion
        
        #region Density Functions
        
        public void RegenerateDensityMap()
        {           
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    densityMap[x, y] = densityFunction.Sample(x, y);
                    
            MarkAllChunksDirty();
        }

        public float GetDensity(int x, int y)
        {
            int clampedX = Mathf.Clamp(x, 0, Width - 1);
            int clampedY = Mathf.Clamp(y, 0, Height - 1);
            
            return densityMap[clampedX, clampedY];
        }
        
        public void SetDensity(int x, int y, float value)
        {
            // Nothing Updated
            if ((x < 0 || y < 0) || (x >= Width || y >= Height))
                return;
            
            float oldValue = densityMap[x, y];
            densityMap[x, y] = Mathf.Clamp01(value);

            // Value Changed
            if (oldValue != densityMap[x, y])
                MarkAffectedChunksAsDirty(x, y);
        }

        #endregion

        #region Chunk Functions

        private void MarkAllChunksDirty()
        {
            dirtyChunks.Clear();
            for (int x = 0; x < Width; x += ChunkSize)
                for (int y = 0; y < Height; y += ChunkSize)
                    dirtyChunks.Add(GridToChunk(x, y));
        }

        private void MarkAffectedChunksAsDirty(int x, int y)
        {
            // Main Chunk
            Vector2Int mainChunk = GridToChunk(x, y);
            dirtyChunks.Add(mainChunk);
            
            // Neighboring Chunks
            bool isLeftBorder = x % ChunkSize == 0 && x > 0;
            bool isBottomBorder = y % ChunkSize == 0 && y > 0;

            if (isLeftBorder)
                dirtyChunks.Add(new Vector2Int(mainChunk.x - 1, mainChunk.y)); // Left chunk

            if (isBottomBorder)
                dirtyChunks.Add(new Vector2Int(mainChunk.x, mainChunk.y - 1)); // Bottom chunk

            if (isLeftBorder && isBottomBorder)
                dirtyChunks.Add(new Vector2Int(mainChunk.x - 1, mainChunk.y - 1)); // Diagonal chunk
        }

        public void ClearDirtyChunks()
        {
            dirtyChunks.Clear();
        }

        #endregion

        #region Coord Helpers

        public Vector2 GridToWorld(Vector2Int gridPosition)
        {
            float x = gridPosition.x * CellSize;
            float y = gridPosition.y * CellSize;
            return new Vector2(x, y);
        }

        public Vector2Int WorldToGrid(Vector2 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x / CellSize);
            int y = Mathf.RoundToInt(worldPosition.y / CellSize);
            return new Vector2Int(x, y);
        }
        
        private Vector2Int GridToChunk(int x, int y)
        {
            return new Vector2Int(x / ChunkSize, y / ChunkSize);
        }

        public Vector2Int WorldToChunk(Vector2 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            return GridToChunk(gridPos.x, gridPos.y);
        }
        
        public (Vector2Int startGridCoord, Vector2Int endGridCoord) GetChunkBounds(Vector2Int chunkCoord)
        {
            int startX = chunkCoord.x * ChunkSize;
            int startY = chunkCoord.y * ChunkSize;
            int endX = Mathf.Min(startX + ChunkSize - 1, Width - 1);
            int endY = Mathf.Min(startY + ChunkSize - 1, Height - 1);
            
            return (new Vector2Int(startX, startY), new Vector2Int(endX, endY));
        }

        #endregion
    }
}