using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Events;
using Runtime.Managers;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Controllers
{
    public class FadeImageController : MonoBehaviour
    {
        private Image _renderer;
        private FadeImageData _data;

        private void Awake()
        {
            _renderer = GetComponent<Image>();
            _data = Resources.Load<CD_FADE_IMAGE>("Data/CD_FADE_IMAGE").FadeImageData;
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onGameStart += OnGameStart;
            CoreGameEvents.Instance.onLevelStart += OnLevelStart;
            CoreGameEvents.Instance.onQuitGame += OnQuitGame;
            CoreGameEvents.Instance.onLoadLevel += OnLoadLevel;
        }

        private void OnLoadLevel()
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration, () =>
            {
                CoreGameEvents.Instance.onResetLevel?.Invoke();
                LevelManager.Instance.OnLevelStart();
                DoFadeScreenDelayed(_data.FadeColor, _data.FadeDuration).Forget();
            });
        }

        private void OnQuitGame()
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration);
        }

        private void OnLevelStart(int arg0)
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration, () =>
            {
                CoreGameEvents.Instance.onLevelInitialize?.Invoke(arg0);
                CoreUIEvents.Instance.onCloseAllPanels?.Invoke();
                DoFadeScreenDelayed(_data.FadeColor, _data.FadeDuration).Forget();
            });
        }

        private void OnGameStart()
        {
            DoFadeScreen(_data.FadeColor, _data.FadeDuration);
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onGameStart -= OnGameStart;
            CoreGameEvents.Instance.onLevelStart -= OnLevelStart;
            CoreGameEvents.Instance.onQuitGame -= OnQuitGame;
            CoreGameEvents.Instance.onLoadLevel -= OnLoadLevel;
        }

        private async UniTaskVoid DoFadeScreenDelayed(Color color, float duration)
        {
            await UnitaskUtilities.WaitForSecondsAsync(_data.WaitNextDuration);
            DoFadeScreen(color, duration);
        }

        private void DoFadeScreen(Color newColor, float duration, TweenCallback onComplete = null)
        {
            _renderer.DOColor(newColor, duration).OnComplete(onComplete);
        }
        
    }
}