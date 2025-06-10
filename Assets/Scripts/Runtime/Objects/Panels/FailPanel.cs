using DG.Tweening;
using Runtime.Abstracts.Classes;
using Runtime.Events;
using Runtime.Utilities;

namespace Runtime.Objects.Panels
{
    public class FailPanel : BasePanel
    {
        protected override void Awake()
        {
            base.Awake();
            _buttons[0].onClick.AddListener(OnClick_Menu);
            _buttons[1].onClick.AddListener(OnClick_Reset);
            _buttons[2].onClick.AddListener(OnClick_Forward);
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
        
        private void OnClick_Menu()
        {
            
        }

        private void OnClick_Reset()
        {
            CoreGameEvents.Instance.onLoadLevel?.Invoke();
            CoreUIEvents.Instance.onClosePanel?.Invoke(2);
        }

        private void OnClick_Forward()
        {
            CoreGameEvents.Instance.onLoadLevel?.Invoke();
            CoreUIEvents.Instance.onClosePanel?.Invoke(2);
        }
        
    }
}