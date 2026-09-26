using System;
using Game.Terrain.DensityFunctions.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Terrain.DensityFunctions
{
    [Serializable]
    public class PerlinNoiseDensityFunction : IDensityFunction
    {
        [SerializeField] float scale;
        [SerializeField] Vector2 offset;
        
        public float Sample(int x, int y)
        {
            float perlinValue = Mathf.PerlinNoise((x * scale) + offset.x, (y * scale) + offset.y);
            return Mathf.Clamp01(perlinValue);
        }
    }
}