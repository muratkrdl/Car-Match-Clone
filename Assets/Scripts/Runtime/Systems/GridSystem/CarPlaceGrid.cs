using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;

        protected override void Awake()
        {
            base.Awake();

            LoadLevelData();
            InitializeGridSystem();
            CreateLevel();
        }
        
        private void LoadLevelData()
        {
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level1");
        }

        private void InitializeGridSystem()
        {
            _gridSystem = new GridSystem<GridObject>
            (
                _currentLevel.CarPlaceWidth,
                _currentLevel.CarPlaceHeight,
                _currentLevel.CarPlaceCellSize,
                (g, pos) => new GridObject(g, pos)
            );
        }

        private void CreateLevel()
        {
            foreach (var gridObject in _gridSystem.GetFlatGridObjectArray())
            {
                Instantiate(carPlaceObjectPrefab, transform);
                gridObject.SetGridType(GridTypes.Space);
            }
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onCarClicked += OnCarClicked;
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onCarClicked -= OnCarClicked;
        }

        private void OnCarClicked(Car car)
        {
            PlaceCarOnGrid(car);
        }

        private void PlaceCarOnGrid(Car car)
        {
            var sameTypeCars = GetSameTypeCars(car);
            var newPosition = sameTypeCars.Any()
                ? new GridPosition(sameTypeCars.Last().GetGridPosition().x + 1, 0)
                : GetFirstEmptySlot().GetGridPosition();

            if (HasCarAt(newPosition))
            {
                SlideCarsToRight(newPosition);
            }

            SetCarToGridPosition(car, newPosition);
        }

        private void SlideCarsToRight(GridPosition fromPosition)
        {
            var carsToSlide = _gridSystem.GetFlatGridObjectArray()
                .Where(g => g.HasCar() && g.GetGridPosition().x >= fromPosition.x)
                .OrderByDescending(g => g.GetGridPosition().x)
                .ToList();
            
            foreach (var gridObject in carsToSlide)
            {
                var from = gridObject.GetGridPosition();
                var to = new GridPosition(from.x + 1, 0);
                MoveCar(gridObject.GetCar(), from, to);
            }
        }

        private void MoveCar(Car car, GridPosition from, GridPosition to)
        {
            SetNullCarAtGridPosition(from);
            SetCarAtGridPosition(to, car);
            car.MoveToGridPosition(_gridSystem.GetGridObject(to));
        }

        private void SetCarToGridPosition(Car car, GridPosition position)
        {
            SetCarAtGridPosition(position, car);
            var sameTypeCars = GetSameTypeCars(car);
            
            List<Car> blastCars = new List<Car>();
            if (sameTypeCars.Count == 3)
            {
                foreach (var item in sameTypeCars)
                {
                    blastCars.Add(item.GetCar());
                    SetNullCarAtGridPosition(item.GetGridPosition());
                }
            };
            
            car.MoveToGridPosition(_gridSystem.GetGridObject(position), CheckBlastableCars(blastCars));
        }

        private TweenCallback CheckBlastableCars(List<Car> blastCars)
        {
            if (blastCars.Count < 3) return null;
            return () =>
            {
                foreach (var item in blastCars)
                {
                    item.gameObject.SetActive(false);
                }

                SlideCarsToLeft();
            };
        }

        private void SlideCarsToLeft()
        {
            foreach (var item in _gridSystem.GetFlatGridObjectArray())
            {
                if (!item.HasCar()) continue;
                
                var from = item.GetGridPosition();
                var to = GetLeftGridPosition(item.GetGridPosition());
                MoveCar(item.GetCar(), from, to);
            }
        }
        private GridPosition GetLeftGridPosition(GridPosition gridPosition)
        {
            GridPosition newPos = new GridPosition(gridPosition.x - 1, gridPosition.y);
            if (IsValidGridPosition(newPos) && !GetCarAtGridObject(newPos).HasCar())
            {
                return GetLeftGridPosition(newPos);
            }

            return gridPosition;
        }

        private List<GridObject> GetSameTypeCars(Car car)
        {
            return _gridSystem.GetFlatGridObjectArray()
                .Where(g => g.HasCar() && g.GetCar().GetCarSo() == car.GetCarSo())
                .OrderBy(g => g.GetGridPosition().x)
                .ToList();
        }

        private GridObject GetFirstEmptySlot()
        {
            return _gridSystem.GetFlatGridObjectArray()
                .FirstOrDefault(g => !g.HasCar());
        }

        private bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);
        private GridObject GetCarAtGridObject(GridPosition position) => _gridSystem.GetGridObject(position);
        private void SetCarAtGridPosition(GridPosition position, Car car) => _gridSystem.GetGridObject(position).SetCar(car);
        private void SetNullCarAtGridPosition(GridPosition position) => _gridSystem.GetGridObject(position).SetNullCar();
        private bool HasCarAt(GridPosition position) => _gridSystem.GetGridObject(position).HasCar();
        private bool HasFreeSpace() => _gridSystem.GetFlatGridObjectArray().Any(g => g.GetIsInteractable());
        public Vector3 GetWorldPosition(GridPosition position) => transform.GetChild(position.x).position;
        public bool HasAvailableSlot() => GetFirstEmptySlot() != null;
        
    }
}
