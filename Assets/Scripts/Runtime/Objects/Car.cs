using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Events;
using Runtime.Systems.GridSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Objects
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private Color fadeColor;
        [SerializeField] private Color normalColor;

        private Image _renderer;
        private Button _button;
        private RectTransform _rectTransform;

        private CarSO _carSO;
        private Vector2Int _coordinates;
        private GridObject _gridObject;

        private bool _isAvailable;

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
            if (!_isAvailable || !LevelGrid.Instance.HasAvailableSlot()) return;

            LevelGrid.Instance.CarMovedGridPosition(_coordinates);

            CoreGameEvents.Instance.onNewFreeSpace?.Invoke(_coordinates);
            CoreGameEvents.Instance.onCarClicked?.Invoke(this);
            
            _coordinates = new Vector2Int(-99, -99);
        }

        public void Initialize(CarSO carSo, Vector2Int gridPosition, GridObject gridObject)
        {
            _carSO = carSo;
            _coordinates = gridPosition;
            _gridObject = gridObject;

            _renderer.sprite = _carSO.CarSprite;
            _rectTransform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);

            _gridObject.SetIsWalkable(false);
        }
        
        public void MoveToGridPosition(GridObject targetGridObject, TweenCallback onComplete = null)
        {
            var targetPosition = LevelGrid.Instance.GetCarPlaceWorldPosition(targetGridObject.GetCoordinates());

            transform.DOMove(targetPosition, 0.5f).OnComplete(onComplete);
        }

        public void SetIsAvailableCar(bool isAvailable)
        {
            _isAvailable = isAvailable;
            _renderer.color = _isAvailable ? normalColor : fadeColor;
        }
        
        public bool GetIsAvailable() => _isAvailable;
        public CarSO GetCarSo() => _carSO;
        public Vector2Int GetGridPosition() => _coordinates;
        
    }
}