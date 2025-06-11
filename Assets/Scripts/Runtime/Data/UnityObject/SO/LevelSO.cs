using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data.UnityObject.SO
{
    /// <summary>
    /// ScriptableObject representing the data for a game level
    /// </summary>
    [CreateAssetMenu(fileName = "LevelSO", menuName = "SO/LevelSO", order = 0)]
    public class LevelSO : ScriptableObject
    {
        public int LevelID;
        public string LevelName = "Level";
        public Sprite BackgroundSprite;
        public int CarCount = 25;
        
        [Range(1,7)] public int Width = 5;
        [Range(1,8)] public int Height = 5;
        
        public Vector2 CellSize = new Vector2(80, 140f);

        public List<Vector2Int> obstacleCoordinates;
        public List<Vector2Int> spaceCoordinates;
        public List<Vector2Int> color1Coordinates;
        public List<Vector2Int> color2Coordinates;
        public List<Vector2Int> color3Coordinates;
        public List<Vector2Int> color4Coordinates;
        public List<Vector2Int> color5Coordinates;

        public int CarPlaceWidth = 7;
        public int CarPlaceHeight = 1;
    }
}