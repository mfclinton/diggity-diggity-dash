using System;
using Game.Terrain.DensityFunctions.Interfaces;
using UnityEngine;

namespace Game.Terrain.DensityFunctions
{
    [Serializable]
    public class FlatDensityFunction : IDensityFunction
    {
        [SerializeField] private float densityValue;
        
        public float Sample(int x, int y)
        {
            return densityValue;
        }
    }
}