namespace Runtime.Systems.GridSystem
{
    public struct GridPosition
    {
        public int x;
        public int y;
 
        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"x: {x}; y: {y}";
        }
        
        public bool IsNearBy(GridPosition point)
        {
            return (point.x == x -1 && point.y == y) 
                   || (point.x == x +1 && point.y == y) 
                   || (point.y == y -1 && point.x == x) 
                   || (point.y == y -1 && point.x == x);
        }
        
    }
}
