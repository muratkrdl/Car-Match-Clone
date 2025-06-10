using System.Collections.Generic;
using System.Linq;
using Runtime.Data.UnityObject.SO;
using Runtime.Events;
using Runtime.Keys;
using Runtime.Objects;
using Runtime.Systems.Pathfinding;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class CarPlaceGrid
    {
        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;

        private List<GridObject> _carPlaceGrid = new List<GridObject>();

        private Systems.Pathfinding.Pathfinding _pathfinding;

        private bool _gameHasFinished = false;
        
        public CarPlaceGrid(GridSystem<GridObject> gridSystem, LevelSO currentLevel)
        {
            _gridSystem = gridSystem;
            _currentLevel = currentLevel;
            
            _pathfinding = new Systems.Pathfinding.Pathfinding(_gridSystem.GetWidth(), _gridSystem.GetHeight(), _gridSystem);
            
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                _carPlaceGrid.Add(_gridSystem.GetGridObject(new Vector2Int(i,0)));
            }
        }

        public void OnReset()
        {
            _carPlaceGrid.Clear();
            _carPlaceGrid = new List<GridObject>();
            _gridSystem = null;
        }

        public void PlaceCarOnGrid(CarObject carObject)
        {
            Vector2Int fromPos = carObject.GetCoordinates();
            LevelGridEvents.Instance.onNewFreeSpace?.Invoke(fromPos);
            SetNullCarAtGridPosition(fromPos);
            
            var sameTypeCars = GetSameTypeCars(carObject);
            var toPosition = sameTypeCars.Any()
                ? new Vector2Int(sameTypeCars.Last().GetCoordinates().x + 1, 0)
                : GetFirstEmptySlot().GetCoordinates();

            if (HasCarAt(toPosition))
            {
                SlideCarsToRight(toPosition);
            }
            
            SetCarToGridPosition(new CarMoveParams()
            {
                CarObject = carObject,
                From = fromPos,
                To = toPosition
            });
        }
        
        private void SlideCarsToRight(Vector2Int fromPosition)
        {
            var carsToSlide = _carPlaceGrid
                .Where(g => g.HasCar() && g.GetCoordinates().x >= fromPosition.x)
                .OrderByDescending(g => g.GetCoordinates().x)
                .ToList();
            
            foreach (var gridObject in carsToSlide)
            {
                var car = gridObject.GetCar();
                var from = gridObject.GetCoordinates();
                var to = new Vector2Int(from.x + 1, 0);
                
                SetNullCarAtGridPosition(from);
                SetCarAtGridPosition(to, car);
                car.MoveSingleToGridPosition(from, to);
            }
        }
        
        private void SetCarToGridPosition(CarMoveParams carMoveParams)
        {
            SetCarAtGridPosition(carMoveParams.To, carMoveParams.CarObject);

            var sameTypeCars = GetSameTypeCars(carMoveParams.CarObject);
            if (sameTypeCars.Count == 3)
            {
                var blastCars = BlastSameTypeCars(sameTypeCars);
                var movedCars = SlideCarsToLeft();
                
                carMoveParams.CarObject.MoveToGridPosition(GetPath(carMoveParams.From, carMoveParams.To), () => HandleBlast(blastCars, movedCars));
            }
            else
            {
                carMoveParams.CarObject.MoveToGridPosition(GetPath(carMoveParams.From, carMoveParams.To), () =>
                {
                    if (!HasAvailableSlot() && !_gameHasFinished)
                    {
                        _gameHasFinished = true;
                        CoreGameEvents.Instance.onLevelFailed?.Invoke();
                    }
                });
            }
        }

        private List<CarObject> BlastSameTypeCars(List<GridObject> sameTypeCars)
        {
            List<CarObject> blastCars = new List<CarObject>();

            foreach (var item in sameTypeCars)
            {
                blastCars.Add(item.GetCar());
                SetNullCarAtGridPosition(item.GetCoordinates());
            }

            return blastCars;
        }

        private void HandleBlast(List<CarObject> blastCars, List<CarMoveParams> slideCarsLeft)
        {
            // TODO : Blast VFX forAnimPos
            Vector2Int to = new Vector2Int(blastCars.Sum(car => car.GetCoordinates().x)/3, 1);
            foreach (var car in blastCars)
            {
                car.BlastAnimation(to, _currentLevel.CellSize);
            }
            
            foreach (var move in slideCarsLeft)
            {
                if (move.CarObject.GetCoordinates() == move.To &&(_gridSystem.GetWorldPosition(move.To) != move.CarObject.transform.position))
                {
                    move.CarObject.MoveSingleToGridPosition(move.From, move.To);
                }
                if (_gridSystem.GetGridObject(move.To).GetCar() != move.CarObject) continue;
                move.CarObject.MoveSingleToGridPosition(move.From, move.To);
            }

            if (CheckLevelSuccess())
            {
                CoreGameEvents.Instance.onLevelSuccess?.Invoke();
            }
        }

        private List<CarMoveParams> SlideCarsToLeft()
        {
            List<CarMoveParams> movedCars = new List<CarMoveParams>();

            foreach (var gridObj in _carPlaceGrid)
            {
                if (!gridObj.HasCar()) continue;

                var from = gridObj.GetCoordinates();
                var to = GetLeftGridPosition(from);
                var car = gridObj.GetCar();

                SetNullCarAtGridPosition(from);
                SetCarAtGridPosition(to, car);

                movedCars.Add(new CarMoveParams()
                {
                    CarObject = car,
                    From = from,
                    To = to
                });
            }

            return movedCars;
        }
        // Recursion 
        private Vector2Int GetLeftGridPosition(Vector2Int gridPosition)
        {
            Vector2Int newPos = new Vector2Int(gridPosition.x - 1, gridPosition.y);
            if (IsValidGridPosition(newPos) && !GetCarAtGridObject(newPos).HasCar())
            {
                return GetLeftGridPosition(newPos);
            }

            return gridPosition;
        }

        private List<GridObject> GetSameTypeCars(CarObject carObject)
        {
            return _carPlaceGrid
                .Where(g => g.HasCar() && g.GetCar().GetCarSo() == carObject.GetCarSo())
                .OrderBy(g => g.GetCoordinates().x)
                .ToList();
        }
        private GridObject GetFirstEmptySlot()
        {
            return _carPlaceGrid.FirstOrDefault(g => !g.HasCar());
        }
        
        private List<Vector3> GetPath(Vector2Int from, Vector2Int to)
        {
            List<PathNode> path = _pathfinding.FindPath(from.x, from.y, to.x, to.y);
            List<Vector3> result = new List<Vector3>();
            foreach (var item in path)
            {
                result.Add(_gridSystem.GetWorldPosition(item.Position));
            }

            return result;
        }

        private bool CheckLevelSuccess()
        {
            if (_gameHasFinished) return false;

            foreach (var item in _gridSystem.GetFlatGridObjectArray())
            {
                if (item.HasCar())
                {
                    return false;
                }
            }

            _gameHasFinished = true;
            return true;
        }

        private bool IsValidGridPosition(Vector2Int gridPosition) => _gridSystem.IsValidGridPosition(gridPosition) && _carPlaceGrid.Contains(_gridSystem.GetGridObject(gridPosition));
        private GridObject GetCarAtGridObject(Vector2Int position) => _gridSystem.GetGridObject(position);
        private void SetCarAtGridPosition(Vector2Int position, CarObject carObject) => _gridSystem.GetGridObject(position).SetCar(carObject);
        private void SetNullCarAtGridPosition(Vector2Int position) => _gridSystem.GetGridObject(position).SetNullCar();
        private bool HasCarAt(Vector2Int position) => _gridSystem.GetGridObject(position).HasCar();
        public bool HasAvailableSlot() => GetFirstEmptySlot() != null;
        
    }
}
