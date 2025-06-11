using Runtime.Abstracts.Interfaces;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Abstracts.Classes
{
    /// <summary>
    /// Abstract base class for all UI panels in the game
    /// </summary>
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

        /// <summary>
        /// Opens the panel
        /// </summary>
        public abstract void OpenPanel();
        
        /// <summary>
        /// Closes the panel
        /// </summary>
        public abstract void ClosePanel();

        /// <summary>
        /// Destroys the panel with OnClosePanel event
        /// </summary>
        protected void OnClosePanel()
        {
            Destroy(gameObject);
        }

    }
}