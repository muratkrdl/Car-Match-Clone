using Runtime.Extensions;
using Runtime.Objects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Events
{
    public class LevelGridEvents : MonoSingleton<LevelGridEvents>
    {
                
        public UnityAction<Vector2Int> onNewFreeSpace;
        public UnityAction<CarObject> onCarClicked;
    }
}