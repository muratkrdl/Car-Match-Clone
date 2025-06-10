using System.Collections.Generic;
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

        private void OnCloseAllPanels()
        {
            foreach (var layer in layers)
            {
                if (layer.childCount <= 0) return;
                Destroy(layer.GetChild(0).gameObject);
            }
        }

        private void OnClosePanel(int value)
        {
            if (layers[value].childCount <= 0) return;
            Destroy(layers[value].GetChild(0).gameObject);
        }

        private void OnOpenPanel(PanelTypes panelType, int layer)
        {
            OnClosePanel(layer);
            Instantiate(Resources.Load<GameObject>($"UIPanels/{panelType}Panel"), layers[layer]);
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
        
    }
}