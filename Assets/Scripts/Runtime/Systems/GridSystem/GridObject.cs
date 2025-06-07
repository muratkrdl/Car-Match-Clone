using Runtime.Enums;
using Runtime.Events;
using Runtime.Objects;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class GridObject
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly GridPosition _gridPosition;
        
        private Car _car;
        private GridTypes _type;
        private bool _isInteractable;

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridSystem = gridSystem;
            _gridPosition = gridPosition;
            
            SetIsInteractable(false);
            
            CoreGameEvents.Instance.onNewFreeSpace += OnNewFreeSpace;
        }
        
        private void OnNewFreeSpace(GridPosition pos)
        {
            if (!_gridPosition.IsNearBy(pos) || _isInteractable)
                return;

            SetIsInteractable(true);
            
            if (_type == GridTypes.Space)
            {
                CoreGameEvents.Instance.onNewFreeSpace?.Invoke(_gridPosition);
            }
        }

        public override string ToString() => _gridPosition.ToString();
        
        public void SetCar(Car car) => _car = car;
        public void SetNullCar() => _car = null;
        public bool HasCar() => _car;
        public Car GetCar() => _car;
        
        public GridSystem<GridObject> GetGridSystem => _gridSystem;
        public GridPosition GetGridPosition() => _gridPosition;
        public GridTypes GetGridType() => _type;
        public bool GetIsInteractable() => _isInteractable;

        public void SetIsInteractable(bool isInteractable)
        {
            _isInteractable = (_type != GridTypes.Obstacle) && isInteractable;

            if (HasCar())
            {
                _car.SetIsAvailableCar(_isInteractable);
            }
        }

        public void SetGridType(GridTypes type)
        {
            _type = type;

            SetIsInteractable(_type == GridTypes.Space);
        }

    }
}