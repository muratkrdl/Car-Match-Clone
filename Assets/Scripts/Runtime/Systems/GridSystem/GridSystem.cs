using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Systems.GridSystem
{
    public class GridSystem<T>
    {
        private int width;
        private int height;
        private Vector2 cellSize;

        private T[,] gridObjectArray;

        public GridSystem(int width, int height, Vector2 cellSize, Func<GridSystem<T>, Vector2Int, T> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridObjectArray = new T[width, height];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    gridObjectArray[x,y] = createGridObject(this, new Vector2Int(x, y));
                }
            }
        }

        public Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x * cellSize.x , gridPosition.y * cellSize.y, 0);
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int
            (
                Mathf.RoundToInt(worldPosition.x / cellSize.x),
                Mathf.RoundToInt(worldPosition.y / cellSize.y)
            );
        }

        public T GetGridObject(Vector2Int gridPosition)
        {
            return gridObjectArray[gridPosition.x, gridPosition.y];
        }

        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < width && gridPosition.y < height;
        }

        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }

        public T[] GetFlatGridObjectArray()
        {
            int width = gridObjectArray.GetLength(0);
            int height = gridObjectArray.GetLength(1);
            T[] flatArray = new T[width * height];

            int index = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    flatArray[index] = gridObjectArray[x, y];
                    index++;
                }
            }
            return flatArray;
        }
        
        public void CreateDebugObjects(Transform debugPrefab, Transform carPrefab, Transform parent = null)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Vector2Int gridPosition = new(x,y);
                    var obj = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parent).GetComponent<GridDebugObject>();
                    obj.SetGridObject(GetGridObject(gridPosition) as GridObject);
                }
            }
        }

    }
}
