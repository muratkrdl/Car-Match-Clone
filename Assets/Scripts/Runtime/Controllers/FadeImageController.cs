using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Enums;
using Runtime.Events;
using Runtime.Managers;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Controllers
{
    /// <summary>
    /// Controls the screen fade image effects
    /// </summary>
    public class FadeImageController : MonoBehaviour
    {
        private Image _renderer;
        private FadeImageData _data;

        /// <summary>
        /// Initializes data
        /// </summary>
        private void Awake()
        {
            _renderer = GetComponent<Image>();
            _data = Resources.Load<CD_FADE_IMAGE>("Data/CD_FADE_IMAGE").FadeImageData;
        }

        /// <summary>
        /// Subscribes Events
        /// </summary>
        private void OnEnable()
        {
            CoreGameEvents.Instance.onGameStart += OnGameStart;
            CoreGameEvents.Instance.onLevelStart += OnLevelStart;
            CoreGameEvents.Instance.onQuitGame += OnQuitGame;
            CoreGameEvents.Instance.onLoadLevel += OnLoadLevel;
            CoreGameEvents.Instance.onGameOver += OnGameOver;
        }

        /// <summary>
        /// Delays a fade screen transition to mainmenu
        /// </summary>
        private void OnGameOver()
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration, () =>
            {
                CoreGameEvents.Instance.onResetLevel?.Invoke();
                CoreUIEvents.Instance.onCloseAllPanels?.Invoke();
                CoreUIEvents.Instance.onOpenPanel?.Invoke(PanelTypes.MainMenu, 0);
                DoFadeScreenDelayed(_data.FadeColor, _data.FadeDuration).Forget();
            });
        }

        /// <summary>
        /// Handles fade sequence when the onLoadLevel event invoked
        /// </summary>
        private void OnLoadLevel()
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration, () =>
            {
                CoreGameEvents.Instance.onResetLevel?.Invoke();
                LevelManager.Instance.OnLevelStart();
                DoFadeScreenDelayed(_data.FadeColor, _data.FadeDuration).Forget();
            });
        }

        /// <summary>
        /// Fades out to quit the game
        /// </summary>
        private void OnQuitGame()
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration);
        }

        /// <summary>
        /// Handles fade sequence when the onLevelStart event invoked
        /// </summary>
        private void OnLevelStart(int arg0)
        {
            DoFadeScreen(_data.NormalColor, _data.FadeDuration, () =>
            {
                CoreGameEvents.Instance.onLevelInitialize?.Invoke(arg0);
                CoreUIEvents.Instance.onCloseAllPanels?.Invoke();
                DoFadeScreenDelayed(_data.FadeColor, _data.FadeDuration).Forget();
            });
        }

        /// <summary>
        /// Handles fade sequence when the onGameStart event invoked
        /// </summary>
        private void OnGameStart()
        {
            DoFadeScreen(_data.FadeColor, _data.FadeDuration);
        }

        /// <summary>
        /// Unsubscribes Events
        /// </summary>
        private void OnDisable()
        {
            CoreGameEvents.Instance.onGameStart -= OnGameStart;
            CoreGameEvents.Instance.onLevelStart -= OnLevelStart;
            CoreGameEvents.Instance.onQuitGame -= OnQuitGame;
            CoreGameEvents.Instance.onLoadLevel -= OnLoadLevel;
        }

        /// <summary>
        /// Delays a fade screen transition by a duration
        /// </summary>
        private async UniTaskVoid DoFadeScreenDelayed(Color color, float duration)
        {
            await UnitaskUtilities.WaitForSecondsAsync(_data.WaitNextDuration);
            DoFadeScreen(color, duration);
        }

        /// <summary>
        /// Fades the screen image to a color
        /// </summary>
        private void DoFadeScreen(Color newColor, float duration, TweenCallback onComplete = null)
        {
            _renderer.DOColor(newColor, duration).SetEase(_data.EaseMode).OnComplete(onComplete);
        }
        
    }
}