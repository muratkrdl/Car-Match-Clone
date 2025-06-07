using Runtime.Systems.GridSystem;

namespace Runtime.Systems.Pathfinding
{
    public class PathNode
    {
        private GridSystem<PathNode> grid;
        public int x;
        public int y;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;

        public PathNode(GridSystem<PathNode> grid, GridPosition position)
        {
            this.grid = grid;
            x = position.x;
            y = position.y;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
            
        public override string ToString()
        {
            return x + "," + y;
        }
        
    }
}