using Runtime.Extensions;
using Runtime.Objects;
using Runtime.Systems.GridSystem;
using UnityEngine.Events;

namespace Runtime.Events
{
    public class CoreGameEvents : MonoSingleton<CoreGameEvents>
    {
        public UnityAction<GridPosition> onNewFreeSpace;
        
        public UnityAction<Car> onCarClicked;
    }
}