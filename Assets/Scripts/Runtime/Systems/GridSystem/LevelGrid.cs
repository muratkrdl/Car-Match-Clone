using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data.UnityObject;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Objects;
using Runtime.Systems.Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Systems.GridSystem
{
    public class LevelGrid : MonoSingleton<LevelGrid>
    {
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private Transform carPrefab;
        [SerializeField] private Transform obstaclePrefab;
        [SerializeField] private CarPlaceObject carPlaceObjectPrefab;
        [SerializeField] private Transform carPlaceObjectParent;
    
        private readonly float _centerOfWidth = 540;
        private readonly float _centerOfHeight = 720;
        
        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;
        private List<CarSO> _allCarsSOs;

        private Pathfinding.Pathfinding _pathfinding;
        private CarPlaceGrid _carPlaceGrid;

        private int _width;
        private int _height;
        private Vector2 _cellSize;
        
        protected override void Awake()
        {
            base.Awake();
            // TODO : CurrentLevelIndex
            LoadResources();
            InitializeGridSystem();

#if UNITY_EDITOR
            // _gridSystem.CreateDebugObjects(gridDebugObjectPrefab, carPrefab, transform);
#endif

            CreateLevel();
        }

        private void LoadResources()
        {
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level");
            _allCarsSOs = Resources.LoadAll<CarSO>("Data/CarSO").ToList();
        }

        private void InitializeGridSystem()
        {
            _width = _currentLevel.Width;
            _height = _currentLevel.Height;
            _cellSize = _currentLevel.CellSize;

            _gridSystem = new GridSystem<GridObject>
            (
                Mathf.Max(_currentLevel.CarPlaceWidth, _width),
                Mathf.Max(_currentLevel.CarPlaceHeight, _height) + 2,
                _cellSize,
                (g, gridPosition) => new GridObject(g, gridPosition)
            );

            _pathfinding = new Pathfinding.Pathfinding(_width, _height, _gridSystem);

            _carPlaceGrid = new CarPlaceGrid(_gridSystem, _currentLevel, carPlaceObjectParent);
        }

        public List<PathNode> GetPath(Vector2Int from, Vector2Int to)
        {
            List<PathNode> path = _pathfinding.FindPath(from.x, from.y, 0, 1);
            Debug.Log("Null");
            if (path != null)
            {
                return path;
                Debug.Log("Null değil");
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.Log(path[i].x + " " + path[i].y);
                    Debug.DrawLine(GetWorldPosition(new Vector2Int(path[i].x, path[i].y)),
                        GetWorldPosition(new Vector2Int(path[i + 1].x, path[i + 1].y)), Color.green, 999f);
                }
            }

            return null;
        }

        private void CreateLevel()
        {
            InitializeObstacle(_currentLevel.obstacleCoordinates);

            List<CarSO> carsSo = new List<CarSO>(_allCarsSOs);
            InitializeCar(_currentLevel.color1Coordinates, carsSo);
            InitializeCar(_currentLevel.color2Coordinates, carsSo);
            InitializeCar(_currentLevel.color3Coordinates, carsSo);
            InitializeCar(_currentLevel.color4Coordinates, carsSo);
            InitializeCar(_currentLevel.color5Coordinates, carsSo);

            InitializeSpaces(_currentLevel.spaceCoordinates);

            InitializeCarPlace();

            PositionGridOnCanvas();
        }
        
        private void PositionGridOnCanvas()
        {
            // TODO : Refactor
            
            float totalWidth = (_currentLevel.CarPlaceWidth-1) * _cellSize.x/2;
            float totalHeight = (_height+1) * _cellSize.y/2;

            transform.position = new Vector3(_centerOfWidth - totalWidth, _centerOfHeight - totalHeight, 0f);
        }
        private void SetGridTypes(IEnumerable<Vector2Int> coordinates, GridTypes type)
        {
            foreach (var coord in coordinates)
            {
                GetGridObject(new Vector2Int(coord.x, coord.y)).SetGridType(type);
            }
        }
        private void InitializeCar(List<Vector2Int> coordinates, List<CarSO> carsSo)
        {
            if (coordinates.Count == 0) return;

            SetGridTypes(coordinates, GridTypes.Car);
            
            var carSo = carsSo[Random.Range(0, carsSo.Count)];
            carsSo.Remove(carSo);
            
            foreach (Vector2Int item in coordinates)
            {
                Vector2Int gridPosition = new(item.x, item.y);
                Car car = Instantiate(carPrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform).GetComponent<Car>();
                SetCarAtGridPosition(gridPosition, car);
                car.Initialize(carSo ,gridPosition, GetGridObject(gridPosition));
            }
        }
        private void InitializeObstacle(List<Vector2Int> coordinates)
        {
            SetGridTypes(coordinates, GridTypes.Obstacle);
            
            foreach (Vector2Int item in coordinates)
            {
                Vector2Int gridPosition = new(item.x, item.y);
                Instantiate(obstaclePrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform);
            }
        }
        private void InitializeSpaces(List<Vector2Int> coordinates)
        {
            SetGridTypes(coordinates, GridTypes.Space);

            // InitialTimeSetInteractable
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                Vector2Int pos = new(i, 1);
                
                CoreGameEvents.Instance.onNewFreeSpace?.Invoke(pos);
            }
        }
        private void InitializeCarPlace()
        {
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                Vector2Int gridPosition = new(i, 0);
                Instantiate(carPlaceObjectPrefab, GetWorldPosition(gridPosition), Quaternion.identity, carPlaceObjectParent);
            }
        }

        public void CarMovedGridPosition(Vector2Int fromPosition) => SetNullCarAtGridPosition(fromPosition);
        private void SetCarAtGridPosition(Vector2Int pos, Car car) => _gridSystem.GetGridObject(pos).SetCar(car);
        private void SetNullCarAtGridPosition(Vector2Int pos) => _gridSystem.GetGridObject(pos).SetNullCar();
        private GridObject GetGridObject(Vector2Int gridPosition) => _gridSystem.GetGridObject(gridPosition);

        public Vector3 GetWorldPosition(Vector2Int gridPos) => _gridSystem.GetWorldPosition(gridPos);
        public Vector3 GetCarPlaceWorldPosition(Vector2Int gridPos) => _carPlaceGrid.GetWorldPosition(gridPos);

        public bool HasAvailableSlot() => _carPlaceGrid.HasAvailableSlot();

    }
}

