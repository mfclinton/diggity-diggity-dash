using System.IO;
using UnityEngine;

namespace Game.Core.Utils
{
    public static class TextureUtils
    {
        public static Texture2D LoadTextureFromPNG(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"PNG file not found: {filePath}");
                return new Texture2D(1, 1);
            }

            // Load Texture
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(filePath));
            
            return texture;
        }

        public static Color[,] TextureToColorMap(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null");
                return new Color[1, 1];
            }
            
            // Get Pixels
            Color[] pixels = texture.GetPixels();

            // Copy to 2D Array
            int width = texture.width;
            int height = texture.height;
            
            Color[,] colorMap = new Color[width, height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[x, y] = pixels[y * width + x];
            
            return colorMap;
        }
    }
}