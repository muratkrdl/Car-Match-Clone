using Runtime.Objects;

namespace Runtime.Systems.GridSystem
{
    public class GridObject
    {
        private GridSystem<GridObject> _gridSystem;
        private GridPosition _gridPosition;
        private Car _car;

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridSystem = gridSystem;
            _gridPosition = gridPosition;
        }

        public override string ToString()
        {
            return _gridPosition.ToString();
        }

        public void SetCar(Car car)
        {
            _car = car;
        }

        public void SetNullCar()
        {
            _car = null;
        }

        public bool HasCar()
        {
            return _car;
        }

        public Car GetCar()
        {
            return _car;
        }
        
    }
}