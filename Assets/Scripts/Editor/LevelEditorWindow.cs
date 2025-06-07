using System;
using System.Collections.Generic;
using Runtime.Data.UnityObject;
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
                }
                return;
            }
            
            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            levelSO.LevelName = EditorGUILayout.TextField("Level Name", levelSO.LevelName);
            
            levelSO.Width = EditorGUILayout.IntSlider("Grid Width", levelSO.Width, 1, 11);
            levelSO.Height = EditorGUILayout.IntSlider("Grid Height", levelSO.Height, 1, 8);
            levelSO.CellSize = EditorGUILayout.Vector2Field("Cell Size", levelSO.CellSize);

            EditorGUILayout.Space();

            DrawGrid();

            EditorGUILayout.EndScrollView();
            
            if (GUI.changed)
            {
                if (!levelSO) return;
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

                levelSO.obstacleCoordinates = obstacleCoordinates;
                levelSO.spaceCoordinates = spaceCoordinates;
                levelSO.color1Coordinates = color1Coordinates;
                levelSO.color2Coordinates = color2Coordinates;
                levelSO.color3Coordinates = color3Coordinates;
                levelSO.color4Coordinates = color4Coordinates;
                levelSO.color5Coordinates = color5Coordinates;
                
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

    }
}
