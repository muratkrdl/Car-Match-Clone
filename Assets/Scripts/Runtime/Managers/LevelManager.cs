using System.Linq;
using Runtime.Data.UnityObject.SO;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Managers
{
    /// <summary>
    /// Manages the current game level
    /// </summary>
    public class LevelManager : MonoSingleton<LevelManager>
    {
        // TODO : PlayerPrefs
        private int _currentLevel;
        private int _maxLevelCount;
        
        /// <summary>
        /// Initializes the current Level
        /// </summary>
        protected override void Awake()
        {
            _maxLevelCount = Resources.LoadAll<LevelSO>("Data/LevelSO").ToList().Count;
            _currentLevel = PlayerPrefs.GetInt(ConstantsUtilities.CURRENT_LEVEL_PLAYERPREFS);
        }

        /// <summary>
        /// Called to start the current level
        /// </summary>
        public void OnLevelStart()
        {
            CoreGameEvents.Instance.onLevelStart?.Invoke(_currentLevel);
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onLevelSucceeded += OnLevelSucceeded;
        }

        /// <summary>
        /// Increase the current level
        /// </summary>
        private void OnLevelSucceeded()
        {
            _currentLevel++;
            if (_currentLevel >= _maxLevelCount)
            {
                _currentLevel = 0;
            }
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onLevelSuccess -= OnLevelSucceeded;
        }

        /// <summary>
        /// Saves the current level index
        /// </summary>
        private void OnDestroy()
        {
            PlayerPrefs.SetInt(ConstantsUtilities.CURRENT_LEVEL_PLAYERPREFS, _currentLevel);
        }
        
    }
}