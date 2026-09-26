using System;
using Game.Terrain.DensityFunctions.Interfaces;
using UnityEngine;

namespace Game.Terrain.DensityFunctions
{
    [Serializable]
    public class TextureDensityFunction : IDensityFunction
    {
        [SerializeField] Texture2D texture;
        public Texture2D Texture => texture;

        public float Sample(int x, int y)
        {
            if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
                return 0f;

            // Sample Color
            Color color = texture.GetPixel(x, y);
            return color.grayscale;
        }
        
        public void SetTexture(Texture2D newTexture)
        {
            texture = newTexture;
        }
    }
}