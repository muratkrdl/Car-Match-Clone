using System.Collections.Generic;
using System.Linq;
using Runtime.Data.UnityObject;
using Runtime.Enums;
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
    
        private int _width;
        private int _height;
        private Vector2 _cellSize;

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
    
        protected override void Awake()
        {
            base.Awake();
            _currentLevel = Resources.Load<LevelSO>("Data/LevelSO/Level1");
            _allCarsSOs = Resources.LoadAll<CarSO>("Data/CarSO").ToList();
            
            _width = _currentLevel.Width;
            _height = _currentLevel.Height;
            _cellSize = _currentLevel.CellSize;
            
            _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize, (g, gridPosition) => new GridObject(g, gridPosition));

#if UNITY_EDITOR
            // _gridSystem.CreateDebugObjects(gridDebugObjectPrefab, carPrefab, transform);
#endif

            CreateLevel();
        }

        private void CreateLevel()
        {
            SetGridTypes(_currentLevel.obstacleCoordinates, GridTypes.Obstacle);
            InitializeObstacle(_currentLevel.obstacleCoordinates);
            
            SetGridTypes(_currentLevel.spaceCoordinates, GridTypes.Space);

            List<CarSO> carsSo = new List<CarSO>(_allCarsSOs);
            InitializeCar(_currentLevel.color1Coordinates, carsSo);
            InitializeCar(_currentLevel.color2Coordinates, carsSo);
            InitializeCar(_currentLevel.color3Coordinates, carsSo);
            InitializeCar(_currentLevel.color4Coordinates, carsSo);
            InitializeCar(_currentLevel.color5Coordinates, carsSo);

            float totalWidth = (_width-1) * _cellSize.x / 2f;
            float totalHeight = (_height-1) * _cellSize.y / 4f;

            transform.position = new Vector3(_centerOfWidth - totalWidth, _centerOfHeight-totalHeight, 0);
        }

        private void SetGridTypes(Vector2Int[] coordinates, GridTypes types)
        {
            if (coordinates.Length == 0) return;
            
            foreach (Vector2Int item in coordinates)
            {
                _gridSystem.GetGridObject(new GridPosition(item.x, item.y)).SetGridType(types);
            }
        }

        private void InitializeCar(Vector2Int[] coordinates, List<CarSO> carsSo)
        {
            if (coordinates.Length == 0) return;

            SetGridTypes(coordinates, GridTypes.Car);
            
            var carSo = carsSo[Random.Range(0, carsSo.Count)];
            carsSo.Remove(carSo);
            
            foreach (Vector2Int item in coordinates)
            {
                GridPosition gridPosition = new(item.x, item.y);
                Car car = Instantiate(carPrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform).GetComponent<Car>();
                SetCarAtGridPosition(gridPosition, car);
                car.Initialize(carSo ,gridPosition);
            }
        }

        private void InitializeObstacle(Vector2Int[] coordinates)
        {
            foreach (Vector2Int item in coordinates)
            {
                GridPosition gridPosition = new(item.x, item.y);
                Instantiate(obstaclePrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform);
            }
        }
        
        public void CarMovedGridPosition(GridPosition fromGridPosition)
        {
            SetNullCarAtGridPosition(fromGridPosition);
        }
    
        private void SetCarAtGridPosition(GridPosition gridPosition, Car car)
        {
            _gridSystem.GetGridObject(gridPosition).SetCar(car);
        }
    
        private void SetNullCarAtGridPosition(GridPosition gridPosition)
        {
            _gridSystem.GetGridObject(gridPosition).SetNullCar();
        }
    
        public bool HasCarOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.HasCar();
        }
    
        public Car GetCarAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.GetCar() as Car;
        }
        
        public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition);
        public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);
        
        public int GetWidth() => _gridSystem.GetWidth();
        public int GetHeight() => _gridSystem.GetHeight();

        public GridPosition CheckAroundOfGridPosition(GridPosition gridPosition)
        {
            foreach (Vector2Int item in _checkPoses)
            {
                var checkPosition = new GridPosition(gridPosition.x + item.x, gridPosition.y + item.y);

                if (!IsValidGridPosition(checkPosition))
                    continue;

                var gridObject = _gridSystem.GetGridObject(checkPosition);
                if (gridObject.GetIsInteractable())
                    return checkPosition;
            }

            return gridPosition;
        }
       
    }
}

