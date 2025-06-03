using Runtime.Extensions;
using Runtime.Systems.GridSystem;
using UnityEngine.Events;

namespace Runtime.Events
{
    public class CoreGameEvents : MonoSingleton<CoreGameEvents>
    {
        public UnityAction<GridPosition> onCarMoved;
    }
}