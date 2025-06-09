using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Events;
using Runtime.Managers;
using Runtime.Systems.GridSystem;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Objects
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private float moveDurationEachGrid;
        [SerializeField] private float maxMoveDuration;
        
        [SerializeField] private Color fadeColor;
        [SerializeField] private Color normalColor;

        private Image _renderer;
        private Button _button;
        private RectTransform _rectTransform;

        private CarSO _carSO;
        private Vector2Int _coordinates;
        private GridObject _gridObject;

        private bool _isAvailable;
        
        private Tween _currentPathTween;

        private void Awake()
        {
            GetReferences();
            _button.onClick.AddListener(OnClick_Button);
        }

        private void GetReferences()
        {
            _renderer = GetComponentInChildren<Image>();
            _button = GetComponentInChildren<Button>();
            _rectTransform = GetComponent<RectTransform>();
        }
        
        private void OnClick_Button()
        {
            if (!_isAvailable || !LevelGridManager.Instance.HasAvailableSlot()) return;

            CoreGameEvents.Instance.onCarClicked?.Invoke(this);
        }

        public void Initialize(CarSO carSo, Vector2Int gridPosition, GridObject gridObject)
        {
            _carSO = carSo;
            _coordinates = gridPosition;
            _gridObject = gridObject;

            _renderer.sprite = _carSO.CarSprite;
            _rectTransform.position = LevelGridManager.Instance.GetWorldPosition(gridPosition);

            _gridObject.SetIsWalkable(false);
        }
        
        public void SetCoordinates(Vector2Int coordinates) => _coordinates = coordinates;
        
        public void MoveToGridPosition(List<Vector3> path, TweenCallback onComplete = null)
        {
            _currentPathTween.Kill();
            float duration = path.Count * moveDurationEachGrid;
            duration = Mathf.Min(duration, maxMoveDuration);
            _currentPathTween = transform.DOLocalPath(path.ToArray(), duration).OnComplete(onComplete);
        }

        public void MoveSingleToGridPosition(Vector2Int from, Vector2Int to, TweenCallback onComplete = null)
        {
            float duration = Mathf.Abs(from.x - to.x) * moveDurationEachGrid;
            var targetPosition = LevelGridManager.Instance.GetWorldPosition(to);
            
            if (_currentPathTween.active)
            {
                _currentPathTween.Kill();
                transform.DOLocalMove(LevelGridManager.Instance.GetWorldPosition(_coordinates), moveDurationEachGrid).OnComplete(onComplete).OnComplete(() =>
                {
                    transform.DOLocalMove(targetPosition, duration).OnComplete(onComplete);
                });
                return;
            }

            transform.DOLocalMove(targetPosition, duration).OnComplete(onComplete);
        }

        public void SetIsAvailableCar(bool isAvailable)
        {
            _isAvailable = isAvailable;
            _renderer.color = _isAvailable ? normalColor : fadeColor;
        }
        
        public CarSO GetCarSo() => _carSO;
        public Vector2Int GetCoordinates() => _coordinates;
        
    }
}