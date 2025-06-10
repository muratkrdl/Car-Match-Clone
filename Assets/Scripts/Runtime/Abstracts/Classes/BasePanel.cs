using Runtime.Abstracts.Interfaces;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Abstracts.Classes
{
    public abstract class BasePanel : MonoBehaviour, IPanelAction
    {
        [SerializeField] protected Button[] _buttons;
        [SerializeField] protected PanelTypes _panelType;
        
        protected bool _clickedButton = false;
        
        protected PanelPopUpData _data;
        
        protected virtual void Awake()
        {
            _data = Resources.Load<CD_PANEL>("Data/CD_PANEL").PanelPopUpData;
        }

        public abstract void OpenPanel();
        public abstract void ClosePanel();

        protected void OnClosePanel()
        {
            Destroy(gameObject);
        }

    }
}