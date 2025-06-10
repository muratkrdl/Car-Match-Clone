using DG.Tweening;
using Runtime.Abstracts.Classes;
using Runtime.Events;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Objects.Panels
{
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
        
        private void OnClick_ClosePanel()
        {
            if (_clickedButton) return;
            
            _clickedButton = true;
            CoreUIEvents.Instance.onClosePanel?.Invoke(1);
        }

        private void OnValueChanged_SFX(float value)
        {
            
        }
        
        private void OnValueChanged_Music(float value)
        {
            
        }

        public override void OpenPanel()
        {
            transform.DOScale(ConstantsUtilities.One3, _data.AnimationDuration).SetEase(_data.AnimationEase);
        }

        public override void ClosePanel()
        {
            transform.DOScale(ConstantsUtilities.Zero3, _data.AnimationDuration)
                .SetEase(_data.AnimationEase)
                .OnComplete(OnClosePanel);
        }
        
    }
}