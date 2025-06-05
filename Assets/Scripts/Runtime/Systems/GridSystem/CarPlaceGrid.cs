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
            _gridSystem = new GridSystem<GridObject>(
                _currentLevel.CarPlaceWidth,
                _currentLevel.CarPlaceHeight,
                _currentLevel.CarPlaceCellSize,
                (g, pos) => new GridObject(g, pos));
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
            if (!HasFreeSpace()) return;
            PlaceCarOnGrid(car);
        }

        private void PlaceCarOnGrid(Car car)
        {
            var sameTypeCars = GetSameTypeCars(car);

            if (sameTypeCars.Any())
            {
                var lastCarPos = sameTypeCars.Last().GetGridPosition();
                var newPos = new GridPosition(lastCarPos.x + 1, 0);

                if (HasCarAt(newPos))
                {
                    SlideCarsToWay(newPos, 1);
                }

                SetCarToGridPosition(car, newPos);
            }
            else
            {
                var emptySlot = GetFirstEmptySlot();
                if (emptySlot == null) return;

                SetCarToGridPosition(car, emptySlot.GetGridPosition());
            }
        }

        private void SlideCarsToWay(GridPosition fromPosition, int offset)
        {
            var carsToSlide = _gridSystem.GetFlatGridObjectArray()
                .Where(g => g.HasCar() && g.GetGridPosition().x >= fromPosition.x)
                .ToList();

            carsToSlide = offset > 0
                ? carsToSlide.OrderByDescending(g => g.GetGridPosition().x).ToList()
                : carsToSlide.OrderBy(g => g.GetGridPosition().x).ToList();
            
            foreach (var gridObject in carsToSlide)
            {
                var from = gridObject.GetGridPosition();
                var to = new GridPosition(from.x + offset, 0);
                MoveCar(gridObject.GetCar(), from, to);
            }
        }

        private void MoveCar(Car car, GridPosition from, GridPosition to)
        {
            car.MoveToGridPosition(_gridSystem.GetGridObject(to));
            
            SetNullCarAtGridPosition(from);
            SetCarAtGridPosition(to, car);
        }

        private void SetCarToGridPosition(Car car, GridPosition position)
        {
            SetCarAtGridPosition(position, car);
            car.MoveToGridPosition(_gridSystem.GetGridObject(position), CheckBlastableCars(car));
        }

        private TweenCallback CheckBlastableCars(Car car)
        {
            var sameTypeCars = GetSameTypeCars(car);
            if (sameTypeCars.Count != 3) return null;
            return () =>
            {
                GridPosition lastGridPosition = sameTypeCars.Last().GetGridPosition();
                
                foreach (var item in sameTypeCars)
                {
                    item.GetCar().gameObject.SetActive(false);
                    SetNullCarAtGridPosition(item.GetGridPosition());
                }
                
                SlideCarsToWay(lastGridPosition, -3);
            };
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

        private void SetCarAtGridPosition(GridPosition position, Car car) => _gridSystem.GetGridObject(position).SetCar(car);
        private void SetNullCarAtGridPosition(GridPosition position) => _gridSystem.GetGridObject(position).SetNullCar();
        private bool HasCarAt(GridPosition position) => _gridSystem.GetGridObject(position).HasCar();
        private bool HasFreeSpace() => _gridSystem.GetFlatGridObjectArray().Any(g => g.GetIsInteractable());
        public Vector3 GetWorldPosition(GridPosition position) => transform.GetChild(position.x).position;
        public bool HasAvailableSlot() => GetFirstEmptySlot() != null;
        
    }
}
