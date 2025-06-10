using Runtime.Enums;
using Runtime.Events;
using Runtime.Objects;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class GridObject
    {
        private readonly GridSystem<GridObject> _gridSystem;
        private readonly Vector2Int _coordinates;
        
        private CarObject carObject;
        private GridTypes _type = GridTypes.None;
        private bool _isWalkable;

        public GridObject(GridSystem<GridObject> gridSystem, Vector2Int coordinates)
        {
            _gridSystem = gridSystem;
            _coordinates = coordinates;
            
            SetIsWalkable(false);
        }
        
        public void OnNewFreeSpace(Vector2Int pos)
        {
            if (!IsNearBy(pos) || _isWalkable) return;

            SetIsWalkable(true);
            
            if (_type == GridTypes.Space)
            {
                LevelGridEvents.Instance.onNewFreeSpace?.Invoke(_coordinates);
            }
        }
        
        private bool IsNearBy(Vector2Int other)
        {
            return (other.x == _coordinates.x - 1 && other.y == _coordinates.y) ||
                   (other.x == _coordinates.x + 1 && other.y == _coordinates.y) ||
                   (other.y == _coordinates.y - 1 && other.x == _coordinates.x) ||
                   (other.y == _coordinates.y + 1 && other.x == _coordinates.x);
        }

        public override string ToString() => _coordinates.x + ", " + _coordinates.y;
        
        public void SetCar(CarObject carObject)
        {
            this.carObject = carObject;
            this.carObject.SetCoordinates(_coordinates);
        }

        public void SetNullCar() => carObject = null;
        public bool HasCar() => carObject;
        public CarObject GetCar() => carObject;
        
        public GridSystem<GridObject> GetGridSystem => _gridSystem;
        public Vector2Int GetCoordinates() => _coordinates;
        public GridTypes GetGridType() => _type;
        public bool GetIsWalkable() => _isWalkable;
        
        public void SetGridType(GridTypes type)
        {
            _type = type;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            _isWalkable = (_type != GridTypes.Obstacle) && isWalkable;

            if (HasCar())
            {
                carObject.SetIsAvailableCar(_isWalkable);
            }
        }

    }
}