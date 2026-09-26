using System.Collections.Generic;
using Game.Terrain;
using UnityEngine;

namespace Game.Actions.Digging.Stencils.Interfaces
{
    public interface IDiggingStencil
    {
        List<DigCell> GetAffectedCells(
            TerrainGrid terrainGrid, 
            Vector2 origin, 
            Vector2 direction);

        void DrawPreview(Vector2 origin, Vector2 direction);
    }
}