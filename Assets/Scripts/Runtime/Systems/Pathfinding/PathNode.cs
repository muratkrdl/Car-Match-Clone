using Runtime.Systems.GridSystem;
using UnityEngine;

namespace Runtime.Systems.Pathfinding
{
    public class PathNode
    {
        private GridSystem<PathNode> grid;
        public Vector2Int Position;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;

        public PathNode(GridSystem<PathNode> grid, Vector2Int position)
        {
            this.grid = grid;
            Position.x = position.x;
            Position.y = position.y;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
            
        public override string ToString()
        {
            return Position.x + "," + Position.y;
        }
        
    }
}