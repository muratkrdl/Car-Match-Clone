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
    public class LevelGrid : MonoSingleton<LevelGrid>
    {
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private Transform carPrefab;
        [SerializeField] private Transform obstaclePrefab;
    
        private readonly float _centerOfWidth = 540;
        private readonly float _centerOfHeight = 720;
        
        private GridSystem<GridObject> _gridSystem;
        private LevelSO _currentLevel;
        private List<CarSO> _allCarsSOs;

        private Vector2Int[] _checkPoses = 
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };
        
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
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level1");
            _allCarsSOs = Resources.LoadAll<CarSO>("Data/CarSO").ToList();
        }

        private void InitializeGridSystem()
        {
            _width = _currentLevel.Width;
            _height = _currentLevel.Height;
            _cellSize = _currentLevel.CellSize;

            _gridSystem = new GridSystem<GridObject>
            (
                _width,
                _height,
                _cellSize,
                (g, gridPosition) => new GridObject(g, gridPosition)
            );
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

            PositionGridOnCanvas();
        }
        
        private void PositionGridOnCanvas()
        {
            float totalWidth = (_width-1) * _cellSize.x / 2f;
            float totalHeight = (_height-1) * _cellSize.y / 4f;

            transform.position = new Vector3(_centerOfWidth - totalWidth, _centerOfHeight - totalHeight, 0f);
        }
        
        private void SetGridTypes(IEnumerable<Vector2Int> coordinates, GridTypes type)
        {
            foreach (var coord in coordinates)
            {
                _gridSystem.GetGridObject(new GridPosition(coord.x, coord.y)).SetGridType(type);
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
                GridPosition gridPosition = new(item.x, item.y);
                Car car = Instantiate(carPrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform).GetComponent<Car>();
                SetCarAtGridPosition(gridPosition, car);
                car.Initialize(carSo ,gridPosition, _gridSystem.GetGridObject(gridPosition));
            }
        }

        private void InitializeObstacle(List<Vector2Int> coordinates)
        {
            SetGridTypes(coordinates, GridTypes.Obstacle);
            
            foreach (Vector2Int item in coordinates)
            {
                GridPosition gridPosition = new(item.x, item.y);
                Instantiate(obstaclePrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform);
            }
        }

        private void InitializeSpaces(List<Vector2Int> coordinates)
        {
            SetGridTypes(_currentLevel.spaceCoordinates, GridTypes.Space);

            for (int i = 0; i < _width; i++)
            {
                GridPosition gridPosition = new(i, -1);
                
                CoreGameEvents.Instance.onNewFreeSpace?.Invoke(gridPosition);
            }
        }
        
        public void CarMovedGridPosition(GridPosition fromPosition) => SetNullCarAtGridPosition(fromPosition);
        private void SetCarAtGridPosition(GridPosition pos, Car car) => _gridSystem.GetGridObject(pos).SetCar(car);
        private void SetNullCarAtGridPosition(GridPosition pos) => _gridSystem.GetGridObject(pos).SetNullCar();

        public bool HasCarOnGridPosition(GridPosition pos) => _gridSystem.GetGridObject(pos).HasCar();
        public Car GetCarAtGridPosition(GridPosition pos) => _gridSystem.GetGridObject(pos).GetCar();
        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystem.GetGridPosition(worldPos);
        
        public GridObject GetGridObject(GridPosition gridPosition) => _gridSystem.GetGridObject(gridPosition);
        public GridSystem<GridObject> GetGridSystem() => _gridSystem;
        
        public Vector3 GetWorldPosition(GridPosition gridPos) => _gridSystem.GetWorldPosition(gridPos);
        public bool IsValidGridPosition(GridPosition pos) => _gridSystem.IsValidGridPosition(pos);

        public int GetWidth() => _gridSystem.GetWidth();
        public int GetHeight() => _gridSystem.GetHeight();

    }
}

