using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data.UnityObject.SO;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Objects;
using Runtime.Systems.GridSystem;
using Runtime.Systems.ObjectPool;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Runtime.Managers
{
    public class LevelGridManager : MonoSingleton<LevelGridManager>
    {
        [SerializeField] private Image backgroundImage;
        
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private Transform carPrefab;
        [SerializeField] private Transform obstaclePrefab;
        [SerializeField] private Transform carPlaceObjectPrefab;
        
        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;
        private List<CarSO> _allCarsSOs;

        private CarPlaceGrid _carPlaceGrid;

        private int _currentLevelID;
        private int _width;
        private int _height;
        private Vector2 _cellSize;

        private void OnEnable()
        {
            CoreGameEvents.Instance.onLevelInitialize += OnLevelInitialize;
            CoreGameEvents.Instance.onResetLevel += OnResetLevel;
            LevelGridEvents.Instance.onCarClicked += OnCarClicked;
            LevelGridEvents.Instance.onNewFreeSpace += OnNewFreeSpace;
            _allCarsSOs = Resources.LoadAll<CarSO>("Data/CarSO").ToList();
        }

        private void OnNewFreeSpace(Vector2Int arg0)
        {
            foreach (var item in _gridSystem.GetFlatGridObjectArray())
            {
                item.OnNewFreeSpace(arg0);
            }
        }

        private void OnLevelInitialize(int level)
        {
            // TODO : CurrentLevelIndex
            _carPlaceGrid?.OnReset();
            _gridSystem?.Clear();
            _gridSystem = null;
            _carPlaceGrid = null;
            LoadResources(level);
            InitializeGridSystem();
            CreateLevel();
        }
        
        private void OnResetLevel()
        {
            transform.position = ConstantsUtilities.Zero3;
        }

        private void OnCarClicked(CarObject carObject)
        {
            _carPlaceGrid.PlaceCarOnGrid(carObject);
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onLevelInitialize -= OnLevelInitialize;
            CoreGameEvents.Instance.onResetLevel -= OnResetLevel;
            LevelGridEvents.Instance.onCarClicked -= OnCarClicked;
            LevelGridEvents.Instance.onNewFreeSpace -= OnNewFreeSpace;
        }

        private void LoadResources(int level)
        {
            // TODO : LevelManager for _currentLevel
            _currentLevelID = level;
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level" + level);

            backgroundImage.sprite = _currentLevel.BackgroundSprite;
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
            
            _carPlaceGrid = new CarPlaceGrid(_gridSystem, _currentLevel);
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
            float totalWidth = (_currentLevel.CarPlaceWidth-1) * _cellSize.x/2;
            float totalHeight = (_height+1) * _cellSize.y/2;

            transform.position = new Vector3(ConstantsUtilities.CenterOfWidth - totalWidth, ConstantsUtilities.CenterOfHeight - totalHeight, 0f);
        }
        private void SetGridTypes(List<Vector2Int> coordinates, GridTypes type)
        {
            foreach (var coordinate in coordinates)
            {
                GetGridObject(new Vector2Int(coordinate.x, coordinate.y)).SetGridType(type);
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
                CarObject carObject = CarObjectObjPool.Instance.GetFromPool();
                SetCarAtGridPosition(gridPosition, carObject);
                carObject.Initialize(carSo ,gridPosition, GetGridObject(gridPosition), transform);
            }
        }
        private void InitializeObstacle(List<Vector2Int> coordinates)
        {
            SetGridTypes(coordinates, GridTypes.Obstacle);
            
            foreach (Vector2Int item in coordinates)
            {
                Vector2Int gridPosition = new(item.x, item.y);
                ObstacleObject obstacleObject = ObstacleObjPool.Instance.GetFromPool();
                obstacleObject.Initialize(GetWorldPosition(gridPosition), transform);
            }
        }
        private void InitializeSpaces(List<Vector2Int> coordinates)
        {
            SetGridTypes(coordinates, GridTypes.Space);

            // InitialTimeSetInteractable
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                Vector2Int pos = new(i, 1);
                
                LevelGridEvents.Instance.onNewFreeSpace?.Invoke(pos);
            }
        }
        private void InitializeCarPlace()
        {
            for (int i = 0; i < _currentLevel.CarPlaceWidth; i++)
            {
                Vector2Int gridPosition = new(i, 0);
                CarPlaceObject carPlaceObject = CarPlaceObjPool.Instance.GetFromPool();
                carPlaceObject.Initialize(GetWorldPosition(gridPosition), transform);
            }
        }

        private void SetCarAtGridPosition(Vector2Int pos, CarObject carObject) => _gridSystem.GetGridObject(pos).SetCar(carObject);
        private GridObject GetGridObject(Vector2Int gridPosition) => _gridSystem.GetGridObject(gridPosition);

        public Vector3 GetWorldPosition(Vector2Int gridPos) => _gridSystem.GetWorldPosition(gridPos);

        public bool HasAvailableSlot() => _carPlaceGrid.HasAvailableSlot();

    }
}

