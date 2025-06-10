using System.Collections.Generic;
using Runtime.Abstracts.Classes;
using Runtime.Enums;
using Runtime.Events;
using UnityEngine;

namespace Runtime.Controllers
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField] private List<Transform> layers = new();

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreUIEvents.Instance.onOpenPanel += OnOpenPanel;
            CoreUIEvents.Instance.onClosePanel += OnClosePanel;
            CoreUIEvents.Instance.onCloseAllPanels += OnCloseAllPanels;
        }
        
        private void OnOpenPanel(PanelTypes panelType, int layer)
        {
            OnClosePanel(layer);
            var panel = Instantiate(Resources.Load<GameObject>($"UIPanels/{panelType}Panel"), layers[layer]).GetComponent<BasePanel>();
            panel.OpenPanel();
        }
        
        private void OnClosePanel(int value)
        {
            if (layers[value].childCount <= 0) return;
            var panel = layers[value].GetChild(0).GetComponent<BasePanel>();
            panel.ClosePanel();
        }

        private void OnCloseAllPanels()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                OnClosePanel(i);
            }
        }
        
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

        private void Start()
        {
            CoreGameEvents.Instance.onGameStart?.Invoke();
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.MainMenu, 0);
        }
        
    }
}