using Runtime.Extensions;
using Runtime.Objects;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class LevelGrid : MonoSingleton<LevelGrid>
    {
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private Transform carPrefab;
    
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 2f;
    
        private GridSystem<GridObject> gridSystem;
        
        // TODO : LevelData
        
    
        protected override void Awake()
        {
            gridSystem = new GridSystem<GridObject>(width, height, cellSize, (g, gridPosition) => new GridObject(g, gridPosition));
#if UNITY_EDITOR
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab, carPrefab, transform);
#endif
        }
    
        public void SetCarAtGridPosition(GridPosition gridPosition, Car car)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetCar(car);
        }
    
        public void SetNullCarAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetNullCar();
        }
    
        public void CarMovedGridPosition(Car car, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            SetNullCarAtGridPosition(fromGridPosition);
    
            SetCarAtGridPosition(toGridPosition, car);
        }
    
        public bool HasCarOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.HasCar();
        }
    
        public Car GetCarAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetCar();
        }
        
        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
        
        public int GetWidth() => gridSystem.GetWidth();
        public int GetHeight() => gridSystem.GetHeight();
       
    }
}

