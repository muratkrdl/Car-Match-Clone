using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Keys;
using Runtime.Objects;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class CarPlaceGrid
    {
        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;
        private Transform _parent;

        private List<GridObject> _carPlaceGrid = new List<GridObject>();

        public CarPlaceGrid(GridSystem<GridObject> gridSystem, LevelSO currentLevel, Transform parent)
        {
            _gridSystem = gridSystem;
            _currentLevel = currentLevel;
            _parent = parent;
            
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                _carPlaceGrid.Add(_gridSystem.GetGridObject(new GridPosition(i,0)));
            }
            
            CoreGameEvents.Instance.onCarClicked += OnCarClicked;
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
            var carsToSlide = _carPlaceGrid
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
        
        private void SetCarToGridPosition(Car car, GridPosition position)
        {
            SetCarAtGridPosition(position, car);

            var sameTypeCars = GetSameTypeCars(car);
            if (sameTypeCars.Count == 3)
            {
                var blastCars = BlastSameTypeCars(sameTypeCars);
                var movedCars = SlideCarsToLeft();
                
                car.MoveToGridPosition(_gridSystem.GetGridObject(position), () => HandleBlast(blastCars, movedCars));
            }
            else
            {
                car.MoveToGridPosition(_gridSystem.GetGridObject(position));
            }
        }

        private List<Car> BlastSameTypeCars(List<GridObject> sameTypeCars)
        {
            List<Car> blastCars = new List<Car>();

            foreach (var item in sameTypeCars)
            {
                blastCars.Add(item.GetCar());
                SetNullCarAtGridPosition(item.GetGridPosition());
            }

            return blastCars;
        }

        private void HandleBlast(List<Car> blastCars, List<SlideCarLeftParams> slideCarsLeft)
        {
            // TODO : Blast Car Anim
            foreach (var car in blastCars)
            {
                car.gameObject.SetActive(false);
            }
            
            foreach (var move in slideCarsLeft)
            {
                if (move.To.GetCar() != move.Car) continue;
                move.Car.MoveToGridPosition(move.To);
            }
        }

        private List<SlideCarLeftParams> SlideCarsToLeft()
        {
            List<SlideCarLeftParams> movedCars = new List<SlideCarLeftParams>();

            foreach (var gridObj in _carPlaceGrid)
            {
                if (!gridObj.HasCar()) continue;

                var from = gridObj.GetGridPosition();
                var to = GetLeftGridPosition(from);
                var car = gridObj.GetCar();

                SetNullCarAtGridPosition(from);
                SetCarAtGridPosition(to, car);

                movedCars.Add(new SlideCarLeftParams()
                {
                    Car = car,
                    From = _gridSystem.GetGridObject(from),
                    To = _gridSystem.GetGridObject(to)
                });
            }

            return movedCars;
        }
        // Recursion 
        private GridPosition GetLeftGridPosition(GridPosition gridPosition)
        {
            GridPosition newPos = new GridPosition(gridPosition.x - 1, gridPosition.y);
            if (IsValidGridPosition(newPos) && !GetCarAtGridObject(newPos).HasCar())
            {
                return GetLeftGridPosition(newPos);
            }

            return gridPosition;
        }

        private void MoveCar(Car car, GridPosition from, GridPosition to)
        {
            SetNullCarAtGridPosition(from);
            SetCarAtGridPosition(to, car);
            
            car.MoveToGridPosition(_gridSystem.GetGridObject(to));
        }

        private List<GridObject> GetSameTypeCars(Car car)
        {
            return _carPlaceGrid
                .Where(g => g.HasCar() && g.GetCar().GetCarSo() == car.GetCarSo())
                .OrderBy(g => g.GetGridPosition().x)
                .ToList();
        }
        private GridObject GetFirstEmptySlot()
        {
            return _carPlaceGrid
                .FirstOrDefault(g => !g.HasCar());
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => _parent.GetChild(gridPosition.x).position;

        private bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition) && _carPlaceGrid.Contains(_gridSystem.GetGridObject(gridPosition));
        private GridObject GetCarAtGridObject(GridPosition position) => _gridSystem.GetGridObject(position);
        private void SetCarAtGridPosition(GridPosition position, Car car) => _gridSystem.GetGridObject(position).SetCar(car);
        private void SetNullCarAtGridPosition(GridPosition position) => _gridSystem.GetGridObject(position).SetNullCar();
        private bool HasCarAt(GridPosition position) => _gridSystem.GetGridObject(position).HasCar();
        public bool HasAvailableSlot() => GetFirstEmptySlot() != null;
        
    }
}
