using UnityEngine;

namespace Game.Actions.Digging.Stencils
{
    public struct DigCell
    {
        public Vector2Int Position;
        public float Strength;

        public DigCell(Vector2Int position, float strength)
        {
            Position = position;
            Strength = strength;
        }
    }
}