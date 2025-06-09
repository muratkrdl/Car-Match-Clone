using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
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
        private CarMoveData _moveData;
        private CarAnimationData _animationData;
        private CarVisualData _visualData;

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
            GetData();
            _button.onClick.AddListener(OnClick_Button);
        }

        private void GetReferences()
        {
            _renderer = GetComponentInChildren<Image>();
            _button = GetComponentInChildren<Button>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void GetData()
        {
            var data = Resources.Load<CD_CAR>("Data/CD_CAR");
            _moveData = data.moveData;
            _animationData = data.animationData;
            _visualData = data.visualData;
        }
        
        private void OnClick_Button()
        {
            if (!_isAvailable || !LevelGridManager.Instance.HasAvailableSlot()) return;
            _isAvailable = false;

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
        
        public void MoveToGridPosition(List<Vector3> path, TweenCallback onComplete = null)
        {
            _currentPathTween.Kill();
            float duration = path.Count * _moveData.MoveDurationEachGrid;
            duration = Mathf.Min(duration, _moveData.MaxMoveDuration);
            _currentPathTween = transform.DOLocalPath(path.ToArray(), duration).OnComplete(onComplete);
        }

        public void MoveSingleToGridPosition(Vector2Int from, Vector2Int to)
        {
            float duration = Mathf.Abs(from.x - to.x) * _moveData.MoveDurationEachGrid;
            var targetPosition = LevelGridManager.Instance.GetWorldPosition(to);
            
            if (_currentPathTween.active)
            {
                _currentPathTween.Kill();
                transform.DOLocalMove(LevelGridManager.Instance.GetWorldPosition(_coordinates), _moveData.MoveDurationEachGrid);
                return;
            }

            transform.DOLocalMove(targetPosition, duration);
        }

        public void BlastAnimation(Vector2Int pos, Vector2 cellSize)
        {
            Vector3 goPosX = pos.x.CompareTo(_coordinates.x) switch
            {
                1 => Vector3.right,
                -1 => Vector3.left,
                _ => Vector3.zero
            };
            goPosX *= cellSize.x;
            Vector3 goPosY = Vector3.up * cellSize.y;
            Sequence sequence = DOTween.Sequence().OnComplete(() => gameObject.SetActive(false));
            sequence.Append(transform.DOScale(_animationData.UpScale, _animationData.BlastAnimationDuration/2))
                    .Join(transform.DOLocalMove(transform.localPosition + goPosY, _animationData.BlastAnimationDuration/2))
                    .Append(transform.DOScale(_animationData.UDownScale, _animationData.BlastAnimationDuration/2))
                    .Join(transform.DOLocalMove(transform.localPosition + goPosY + goPosX, _animationData.BlastAnimationDuration/2));
        }

        public void SetIsAvailableCar(bool isAvailable)
        {
            _isAvailable = isAvailable;
            _renderer.color = _isAvailable ? _visualData.normalColor : _visualData.fadeColor;
        }
        
        public void SetCoordinates(Vector2Int coordinates) => _coordinates = coordinates;
        public CarSO GetCarSo() => _carSO;
        public Vector2Int GetCoordinates() => _coordinates;
        
    }
}