using Runtime.Data.UnityObject;
using Runtime.Extensions;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class CarPlaceGrid : MonoSingleton<CarPlaceGrid>
    {
        [SerializeField] private CarPlaceObject carPlaceObjectPrefab;
    
        private int _width;
        private int _height;
        private Vector2 _cellSize;

        private readonly float _centerOfWidth = 540;
    
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

        private void CreateLevel()
        {
            InitializePlaces();
        }
        
        private void InitializePlaces()
        {
            foreach (GridObject item in _gridSystem.GetFlatGridObjectArray())
            {
                Instantiate(carPlaceObjectPrefab, GetWorldPosition(item.GetGridPosition()), Quaternion.identity, transform);
            }
        }
        
        public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);
        
    }
}