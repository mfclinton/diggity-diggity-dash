using System;
using UnityEngine;

namespace Game.Terrain.MarchingSquares.Data
{
    [Serializable]
    public struct Edge
    {
        public Vector2 Start;
        public Vector2 End;
        
        public Edge(Vector2 start, Vector2 end)
        {
            // Ensure consistent ordering for better comparison/deduplication
            if (start.x < end.x || (start.x == end.x && start.y < end.y))
            {
                Start = start;
                End = end;
            }
            else
            {
                Start = end;
                End = start;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge other))
                return false;

            return (Start == other.Start && End == other.End);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ (End.GetHashCode() * 397);
        }
    }
}