using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data.UnityObject;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Objects;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class CarPlaceGrid : MonoSingleton<CarPlaceGrid>
    {
        [SerializeField] private CarPlaceObject carPlaceObjectPrefab;
    
        private int _width;
        private int _height;
        private Vector2 _cellSize;

        private GridSystem<GridObject> _gridSystem;
        
        private LevelSO _currentLevel;

        private Vector2Int[] _checkPoses = 
        {
            Vector2Int.left,
            Vector2Int.right,
        };
    
        protected override void Awake()
        {
            base.Awake();
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level1");
            
            _width = _currentLevel.CarPlaceWidth;
            _height = _currentLevel.CarPlaceHeight;
            _cellSize = _currentLevel.CarPlaceCellSize;
            
            _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize, (g, gridPosition) => new GridObject(g, gridPosition));

            CreateLevel();
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onCarClicked += OnCarClicked;
        }

        private void OnCarClicked(Car car)
        {
            if (!HasFreeSpace()) return;
            
            SetNewCar(car);
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onCarClicked -= OnCarClicked;
        }

        private void CreateLevel()
        {
            InitializePlaces();
        }
        
        private void InitializePlaces()
        {
            foreach (GridObject item in _gridSystem.GetFlatGridObjectArray())
            {
                Instantiate(carPlaceObjectPrefab, transform);
                item.SetGridType(GridTypes.Space);
            }
        }
        
        private void SetNewCar(Car car)
        {
            var gridArray = _gridSystem.GetFlatGridObjectArray();

            var sameTypeCars = gridArray
                .Where(g => g.HasCar() && g.GetCar().GetCarSo() == car.GetCarSo())
                .OrderBy(g => g.GetGridPosition().x)
                .ToList();

            if (sameTypeCars.Count > 0)
            {
                GridPosition newPos = new GridPosition(sameTypeCars.Last().GetGridPosition().x + 1, 0);

                if (HasCarOnGridPosition(newPos))
                {
                    SlideObjectToWay(newPos, 1);
                }

                SetCarAtGridPosition(newPos, car);
                car.SetIsAvailable(false);
                car.DoMoveToPos(GetWorldPosition(newPos));
            }
            else
            {
                var emptySlot = gridArray
                    .Where(g => !g.HasCar())
                    .OrderBy(g => g.GetGridPosition().x)
                    .FirstOrDefault();

                if (emptySlot == null) return;
                
                var pos = emptySlot.GetGridPosition();
                SetCarAtGridPosition(pos, car);
                car.SetIsAvailable(false);
                car.DoMoveToPos(GetWorldPosition(pos));
            }
        }

        private void SlideObjectToWay(GridPosition pos, int amount)
        {
            List<GridObject> slideToRightList = _gridSystem.GetFlatGridObjectArray()
                .Where(g => g.HasCar() && g.GetGridPosition().x >= pos.x)
                .OrderByDescending(g => g.GetGridPosition().x)
                .ToList();

            foreach (var item in slideToRightList)
            {
                GridPosition toObject = new GridPosition(item.GetGridPosition().x + amount, 0);
                CarMovedGridPosition(item.GetCar(), item.GetGridPosition(), toObject);
            }
        }

        private void CarMovedGridPosition(Car car, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            SetNullCarAtGridPosition(fromGridPosition);
    
            SetCarAtGridPosition(toGridPosition, car);
            
            car.DoMoveToPos(GetWorldPosition(toGridPosition));
        }
        
        private void SetCarAtGridPosition(GridPosition gridPosition, Car car)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.SetCar(car);
        }
    
        private void SetNullCarAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.SetNullCar();
        }
    
        private bool HasCarOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.HasCar();
        }
    
        private Car GetCarAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.GetCar();
        }

        private bool HasFreeSpace()
        {
            return _gridSystem.GetFlatGridObjectArray().Any(item => item.GetIsInteractable());
        }
        
        private Vector3 GetWorldPosition(GridPosition gridPosition) => transform.GetChild(gridPosition.x).transform.position;
        private bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);

        public bool GetAvailableSlot()
        {
            var emptySlot = _gridSystem.GetFlatGridObjectArray()
                .Where(g => !g.HasCar())
                .OrderBy(g => g.GetGridPosition().x)
                .FirstOrDefault();

            return emptySlot != null;
        }
        
    }
}