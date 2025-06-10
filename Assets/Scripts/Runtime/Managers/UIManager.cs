using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;

namespace Runtime.Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameEvents.Instance.onLevelSuccess += OnLevelSuccess;
            CoreGameEvents.Instance.onLevelFailed += OnLevelFailed;
        }

        private void OnLevelFailed()
        {
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.Fail,2);
        }

        private void OnLevelSuccess()
        {
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.Win,2);
        }

        private void OnReset()
        {
            CoreUIEvents.Instance.onCloseAllPanels?.Invoke();
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.MainMenu, 1);
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameEvents.Instance.onLevelSuccess -= OnLevelSuccess;
            CoreGameEvents.Instance.onLevelFailed -= OnLevelFailed;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        public void Play()
        {
            CoreUIEvents.Instance.onClosePanel(1);
        }

    }
}