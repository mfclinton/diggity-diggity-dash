using System;
using UnityEngine;

namespace Game.Terrain.MarchingSquares.Data
{
    [Serializable]
    public struct Triangle
    {
        public Vector2 VertexA;
        public Vector2 VertexB;
        public Vector2 VertexC;
        
        public int CaseIndex;

        public Triangle(Vector2 a, Vector2 b, Vector2 c, int caseIndex = -1)
        {
            VertexA = a;
            VertexB = b;
            VertexC = c;
            CaseIndex = caseIndex;
        }
    }
}