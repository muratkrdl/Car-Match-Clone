using DG.Tweening;
using Runtime.Abstracts.Classes;
using Runtime.Events;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Objects.Panels
{
    /// <summary>
    /// Represents the Settings panel
    /// </summary>
    public class SettingsPanel : BasePanel
    {
        [SerializeField] private Slider[] _sliders;
        
        protected override void Awake()
        {
            base.Awake();
            _buttons[0].onClick.AddListener(OnClick_ClosePanel);
            _buttons[1].onClick.AddListener(OnClick_ClosePanel);
            _sliders[0].onValueChanged.AddListener(OnValueChanged_Music);
            _sliders[1].onValueChanged.AddListener(OnValueChanged_SFX);
        }
        
        /// <summary>
        /// Handles the Close button click
        /// </summary>
        private void OnClick_ClosePanel()
        {
            if (_clickedButton) return;
            
            _clickedButton = true;
            CoreUIEvents.Instance.onClosePanel?.Invoke(1);
        }

        /// <summary>
        /// Handles Nothing..
        /// </summary>
        private void OnValueChanged_SFX(float value)
        {
            
        }
        
        /// <summary>
        /// Handles Nothing..
        /// </summary>
        private void OnValueChanged_Music(float value)
        {
            
        }

        /// <summary>
        /// Handles open panel animation
        /// </summary>
        public override void OpenPanel()
        {
            transform.DOScale(ConstantsUtilities.One3, _data.AnimationDuration).SetEase(_data.AnimationEase);
        }

        /// <summary>
        /// Handles close panel animation
        /// </summary>
        public override void ClosePanel()
        {
            transform.DOScale(ConstantsUtilities.Zero3, _data.AnimationDuration)
                .SetEase(_data.AnimationEase)
                .OnComplete(OnClosePanel);
        }
        
    }
}