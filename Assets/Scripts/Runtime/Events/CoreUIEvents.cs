using Runtime.Enums;
using Runtime.Extensions;
using UnityEngine.Events;

namespace Runtime.Events
{
    public class CoreUIEvents : MonoSingleton<CoreUIEvents>
    {
        public UnityAction<PanelTypes, int> onOpenPanel;
        public UnityAction<int> onClosePanel;
        public UnityAction onCloseAllPanels;
    }
}