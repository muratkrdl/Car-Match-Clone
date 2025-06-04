using UnityEngine;

namespace Runtime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        public int LevelID;
        public string LevelName = "Level ";
        public Sprite BackgroundSprite;
        public int CarCount = 25;
        
        [Range(1,11)] public int Width = 5;
        [Range(1,8)] public int Height = 5;
        
        public Vector2 CellSize = new Vector2(67.5f, 135f);

        public Vector2Int[] obstacleCoordinates;
        public Vector2Int[] spaceCoordinates;
        public Vector2Int[] color1Coordinates;
        public Vector2Int[] color2Coordinates;
        public Vector2Int[] color3Coordinates;
        public Vector2Int[] color4Coordinates;
        public Vector2Int[] color5Coordinates;

        public int CarPlaceWidth;
        public int CarPlaceHeight;
        public Vector2 CarPlaceCellSize;
    }
}