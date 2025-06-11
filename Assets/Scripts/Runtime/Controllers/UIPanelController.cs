using System.Collections.Generic;
using Runtime.Abstracts.Classes;
using Runtime.Enums;
using Runtime.Events;
using UnityEngine;

namespace Runtime.Controllers
{
    /// <summary>
    /// Manages the opening and closing of UI panels
    /// </summary>
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField] private List<Transform> layers = new();

        private void OnEnable()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Subscribes to Events
        /// </summary>
        private void SubscribeEvents()
        {
            CoreUIEvents.Instance.onOpenPanel += OnOpenPanel;
            CoreUIEvents.Instance.onClosePanel += OnClosePanel;
            CoreUIEvents.Instance.onCloseAllPanels += OnCloseAllPanels;
        }
        
        /// <summary>
        /// Opens a panel of the specified type on the specified UI layer
        /// </summary>
        private void OnOpenPanel(PanelTypes panelType, int layer)
        {
            OnClosePanel(layer);
            var panel = Instantiate(Resources.Load<GameObject>($"UIPanels/{panelType}Panel"), layers[layer]).GetComponent<BasePanel>();
            panel.OpenPanel();
        }
        
        /// <summary>
        /// Closes the panel on a specified layer
        /// </summary>
        private void OnClosePanel(int value)
        {
            if (layers[value].childCount <= 0) return;
            var panel = layers[value].GetChild(0).GetComponent<BasePanel>();
            panel.ClosePanel();
        }

        /// <summary>
        /// Closes all open panels
        /// </summary>
        private void OnCloseAllPanels()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                OnClosePanel(i);
            }
        }
        
        /// <summary>
        /// Unsubscribes to Events
        /// </summary>
        private void UnSubscribeEvents()
        {
            CoreUIEvents.Instance.onOpenPanel -= OnOpenPanel;
            CoreUIEvents.Instance.onClosePanel -= OnClosePanel;
            CoreUIEvents.Instance.onCloseAllPanels -= OnCloseAllPanels;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        /// <summary>
        /// Invoke onGameStart and open maninmenu panel
        /// </summary>
        private void Start()
        {
            CoreGameEvents.Instance.onGameStart?.Invoke();
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.MainMenu, 0);
        }
        
    }
}