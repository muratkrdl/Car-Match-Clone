using Runtime.Extensions;
using UnityEngine.Events;

namespace Runtime.Events
{
    public class CoreGameEvents : MonoSingleton<CoreGameEvents>
    {
        public UnityAction onGameStart;
        public UnityAction onGameOver;
        
        public UnityAction<int> onLevelStart;
        public UnityAction<int> onLevelInitialize;
        
        public UnityAction onLevelFinish;
        public UnityAction onLevelSuccess;
        public UnityAction onLevelSucceeded;
        public UnityAction onLevelFailed;
        public UnityAction onLoadLevel;
        public UnityAction onResetLevel;
        
        public UnityAction onQuitGame;
    }
}