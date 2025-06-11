using DG.Tweening;
using Runtime.Abstracts.Classes;
using Runtime.Events;
using Runtime.Utilities;

namespace Runtime.Objects.Panels
{
    /// <summary>
    /// Represents the pause panel
    /// </summary>
    public class PausePanel : BasePanel
    {
        protected override void Awake()
        {
            base.Awake();
            _buttons[0].onClick.AddListener(OnClick_Resume);
            _buttons[1].onClick.AddListener(OnClick_Restart);
            _buttons[2].onClick.AddListener(OnClick_Quit);
        }

        /// <summary>
        /// Handles the Resume button click
        /// </summary>
        private void OnClick_Resume()
        {
            if (_clickedButton) return;
            
            _clickedButton = true;
            CoreUIEvents.Instance.onClosePanel?.Invoke(4);
        }

        /// <summary>
        /// Handles the Restart button click
        /// </summary>
        private void OnClick_Restart()
        {
            if (_clickedButton) return;
            
            _clickedButton = true;
            CoreGameEvents.Instance.onLoadLevel?.Invoke();
            CoreUIEvents.Instance.onClosePanel?.Invoke(4);
        }

        /// <summary>
        /// Handles the Quit button click
        /// </summary>
        private void OnClick_Quit()
        {
            if (_clickedButton) return;
            
            _clickedButton = true;
            CoreGameEvents.Instance.onGameOver?.Invoke();
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