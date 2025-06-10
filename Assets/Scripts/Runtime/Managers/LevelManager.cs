using System.Linq;
using Runtime.Data.UnityObject;
using Runtime.Data.UnityObject.SO;
using Runtime.Events;
using Runtime.Extensions;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        // TODO : PlayerPrefs
        private int _currentLevel;
        private int _maxLevelCount;
        
        protected override void Awake()
        {
            _maxLevelCount = Resources.LoadAll<LevelSO>("Data/LevelSO").ToList().Count;
            _currentLevel = PlayerPrefs.GetInt(ConstantsUtilities.CURRENT_LEVEL_PLAYERPREFS);
        }

        public void OnLevelStart()
        {
            CoreGameEvents.Instance.onLevelStart?.Invoke(_currentLevel);
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onLevelSucceeded += OnLevelSucceeded;
        }

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

        private void OnDestroy()
        {
            PlayerPrefs.SetInt(ConstantsUtilities.CURRENT_LEVEL_PLAYERPREFS, _currentLevel);
        }
        
    }
}