using Runtime.Enums;
using Runtime.Events;
using Runtime.Extensions;

namespace Runtime.Managers
{
    /// <summary>
    /// Manages panel
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        private void OnEnable()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        private void SubscribeEvents()
        {
            CoreGameEvents.Instance.onLevelSuccess += OnLevelSuccess;
            CoreGameEvents.Instance.onLevelFailed += OnLevelFailed;
        }

        /// <summary>
        /// Open WinPanel
        /// </summary>
        private void OnLevelFailed()
        {
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.Fail,2);
        }

        /// <summary>
        /// Open FailPanel
        /// </summary>
        private void OnLevelSuccess()
        {
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.Win,2);
        }

        /// <summary>
        /// Unsubscribes to events
        /// </summary>
        private void UnSubscribeEvents()
        {
            CoreGameEvents.Instance.onLevelSuccess -= OnLevelSuccess;
            CoreGameEvents.Instance.onLevelFailed -= OnLevelFailed;
        }
        
        private void OnDisable()
        {
            UnSubscribeEvents();
        }

    }
}