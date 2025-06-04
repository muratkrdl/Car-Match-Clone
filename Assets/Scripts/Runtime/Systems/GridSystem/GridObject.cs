using System;
using Runtime.Enums;
using Runtime.Objects;

namespace Runtime.Systems.GridSystem
{
    public class GridObject
    {
        private GridSystem<GridObject> _gridSystem;
        private GridPosition _gridPosition;
        private Car _car;

        private GridTypes _type;

        private bool _isInteractable;

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridSystem = gridSystem;
            _gridPosition = gridPosition;
        }

        public override string ToString() => _gridPosition.ToString();
        public void SetCar(Car car) => _car = car;
        public void SetNullCar() => _car = null;
        public bool HasCar() => _car;
        public Car GetCar() => _car;
        
        public GridPosition GetGridPosition() => _gridPosition;

        public void SetGridType(GridTypes type)
        {
            _isInteractable = type switch
            {
                GridTypes.Space => true,
                _ => false
            };
            _type = type;
        }
        public GridTypes GetGridType() => _type;
        public bool GetIsInteractable() => _isInteractable;

    }
}