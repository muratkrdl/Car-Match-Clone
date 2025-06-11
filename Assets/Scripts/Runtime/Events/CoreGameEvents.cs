using Runtime.Extensions;
using UnityEngine.Events;

namespace Runtime.Events
{
    /// <summary>
    /// Manages core game event actions
    /// </summary>
    public class CoreGameEvents : MonoSingleton<CoreGameEvents>
    {
        public UnityAction onGameStart;
        public UnityAction onGameOver;
        
        public UnityAction<int> onLevelStart;
        public UnityAction<int> onLevelInitialize;
        
        public UnityAction onLevelSuccess;
        public UnityAction onLevelSucceeded;
        public UnityAction onLevelFailed;
        public UnityAction onLoadLevel;
        public UnityAction onResetLevel;
        
        public UnityAction onQuitGame;
    }
}