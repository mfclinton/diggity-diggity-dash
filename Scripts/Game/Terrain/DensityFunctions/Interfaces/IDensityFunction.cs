using UnityEngine;

namespace Game.Terrain.DensityFunctions.Interfaces
{
    public interface IDensityFunction
    {
        float Sample(int x, int y);
    }
}