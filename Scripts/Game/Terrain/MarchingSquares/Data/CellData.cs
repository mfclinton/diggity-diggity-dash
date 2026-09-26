using UnityEngine;

namespace Game.Terrain.MarchingSquares.Data
{
    public struct CellData
    {
        public Vector2 BottomLeft, BottomRight, TopRight, TopLeft;
        public float BottomLeftValue, BottomRightValue, TopRightValue, TopLeftValue;
        public int CaseIndex;
            
        public Vector2 MidBottom, MidRight, MidTop, MidLeft;
    }
}