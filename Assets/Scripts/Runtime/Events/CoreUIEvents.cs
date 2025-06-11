using Runtime.Enums;
using Runtime.Extensions;
using UnityEngine.Events;

namespace Runtime.Events
{
    /// <summary>
    /// Manages core UI event actions
    /// </summary>
    public class CoreUIEvents : MonoSingleton<CoreUIEvents>
    {
        public UnityAction<PanelTypes, int> onOpenPanel;
        public UnityAction<int> onClosePanel;
        public UnityAction onCloseAllPanels;
    }
}