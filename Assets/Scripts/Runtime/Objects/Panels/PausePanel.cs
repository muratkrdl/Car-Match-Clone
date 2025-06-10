using DG.Tweening;
using Runtime.Abstracts.Classes;
using Runtime.Events;
using Runtime.Utilities;

namespace Runtime.Objects.Panels
{
    public class PausePanel : BasePanel
    {
        protected override void Awake()
        {
            base.Awake();
            _buttons[0].onClick.AddListener(OnClick_Resume);
            _buttons[1].onClick.AddListener(OnClick_Restart);
            _buttons[2].onClick.AddListener(OnClick_Quit);
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

        private void OnClick_Resume()
        {
            CoreUIEvents.Instance.onClosePanel?.Invoke(4);
        }

        private void OnClick_Restart()
        {
            CoreGameEvents.Instance.onLoadLevel?.Invoke();
            CoreUIEvents.Instance.onClosePanel?.Invoke(4);
        }

        private void OnClick_Quit()
        {
            CoreGameEvents.Instance.onLoadLevel?.Invoke();
            CoreUIEvents.Instance.onClosePanel?.Invoke(4);
        }
        
    }
}