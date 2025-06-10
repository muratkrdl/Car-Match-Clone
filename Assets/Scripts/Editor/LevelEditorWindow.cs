using System;
using System.Collections.Generic;
using Runtime.Data.UnityObject;
using Runtime.Data.UnityObject.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private Dictionary<Vector2Int, int> cellColorIndexes = new();
        
        private readonly Color[] colors =
        {
            Color.white,
            Color.blue,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow,
            Color.black,
        };
        
        private LevelSO levelSO;
        private Vector2 scrollPos;
        
        private List<Vector2Int> obstacleCoordinates = new();
        private List<Vector2Int> spaceCoordinates = new();
        private List<Vector2Int> color1Coordinates = new();
        private List<Vector2Int> color2Coordinates = new();
        private List<Vector2Int> color3Coordinates = new();
        private List<Vector2Int> color4Coordinates = new();
        private List<Vector2Int> color5Coordinates = new();

        [MenuItem("Tools/Car Match/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Editor", EditorStyles.boldLabel);

            if (!levelSO)
            {
                if (GUILayout.Button("New Level"))
                {
                    levelSO = CreateInstance<LevelSO>();
                    cellColorIndexes.Clear();
                    obstacleCoordinates.Clear();
                    spaceCoordinates.Clear();
                    color1Coordinates.Clear();
                    color2Coordinates.Clear();
                    color3Coordinates.Clear();
                    color4Coordinates.Clear();
                    color5Coordinates.Clear();
                }
                return;
            }
            
            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            levelSO.LevelName = EditorGUILayout.TextField("Level Name", levelSO.LevelName);
            
            levelSO.Width = EditorGUILayout.IntSlider("Grid Width", levelSO.Width, 1, 11);
            levelSO.Height = EditorGUILayout.IntSlider("Grid Height", levelSO.Height, 1, 8);
            levelSO.CellSize = EditorGUILayout.Vector2Field("Cell Size", levelSO.CellSize);

            if (levelSO.Width % 2 != 1) return;

            EditorGUILayout.Space();

            DrawGrid();

            EditorGUILayout.EndScrollView();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelSO);
            }
        }

        private void DrawGrid()
        {
            float buttonSize = 30;
            for (int y = levelSO.Height -1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < levelSO.Width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!cellColorIndexes.ContainsKey(pos))
                        cellColorIndexes[pos] = 0;

                    int colorIndex = cellColorIndexes[pos];
                    GUI.backgroundColor = colors[colorIndex];
                    
                    if (GUILayout.Button($"{x},{y}", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        colorIndex = (colorIndex + 1) % colors.Length;
                        cellColorIndexes[pos] = colorIndex;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("CreateLevel", GUILayout.Height(buttonSize)))
            {
                foreach (var item in cellColorIndexes)
                {
                    if (PositionAtDeadZone(item.Key)) continue;
                    
                    var list = item.Value switch
                    {
                        0 => spaceCoordinates,
                        1 => color1Coordinates,
                        2 => color2Coordinates,
                        3 => color3Coordinates,
                        4 => color4Coordinates,
                        5 => color5Coordinates,

                        _ => obstacleCoordinates
                    };
                    list.Add(item.Key);
                }

                levelSO.obstacleCoordinates = ParseToRealFormat(obstacleCoordinates);
                levelSO.spaceCoordinates = ParseToRealFormat(spaceCoordinates);
                levelSO.color1Coordinates = ParseToRealFormat(color1Coordinates);
                levelSO.color2Coordinates = ParseToRealFormat(color2Coordinates);
                levelSO.color3Coordinates = ParseToRealFormat(color3Coordinates);
                levelSO.color4Coordinates = ParseToRealFormat(color4Coordinates);
                levelSO.color5Coordinates = ParseToRealFormat(color5Coordinates);

                for (int i = 0; i < levelSO.CarPlaceWidth; i++)
                {
                    for (int j = 0; j < 2; j++)
                    { 
                        levelSO.spaceCoordinates.Add(new Vector2Int(i, j));
                    }
                }
                
                string path = $"Assets/Resources/Data/LevelSO/{levelSO.LevelName}.asset";
                if (AssetDatabase.LoadAssetAtPath<LevelSO>(path))
                {
                    Debug.Log("Change folder name");
                }
                else
                {
                    AssetDatabase.CreateAsset(levelSO, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    levelSO = null;
                }
            }
        }

        private List<Vector2Int> ParseToRealFormat(List<Vector2Int> parseList)
        {
            List<Vector2Int> returnList = new List<Vector2Int>();

            Vector2Int make = new Vector2Int((levelSO.CarPlaceWidth - levelSO.Width)/2, +2);
            
            foreach (var item in parseList)
            {
                returnList.Add(item + make);
            }

            return returnList;
        }

        private bool PositionAtDeadZone(Vector2Int pos)
        {
            return pos.x >= levelSO.Width || pos.y >= levelSO.Height;
        }

    }
}
