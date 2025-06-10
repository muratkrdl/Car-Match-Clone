using Runtime.Abstracts.Classes;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Managers;
using Runtime.Utilities;

namespace Runtime.Objects.Panels
{
    public class MainMenuPanel : BasePanel
    {
        protected override void Awake()
        {
            base.Awake();
            _buttons[0].onClick.AddListener(OnClick_Start);
            _buttons[1].onClick.AddListener(OnClick_Settings);
            _buttons[2].onClick.AddListener(OnClick_Quit);
        }

        public override void OpenPanel()
        {
            transform.localScale = ConstantsUtilities.One3;
        }

        public override void ClosePanel()
        {
            OnClosePanel();
        }
        
        private void OnClick_Start()
        {
            LevelManager.Instance.OnLevelStart();
        }
        private void OnClick_Settings()
        {
            CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.Settings, 1);
        }
        private void OnClick_Quit()
        {
            CoreGameEvents.Instance.onQuitGame?.Invoke();
        }
        
    }
}