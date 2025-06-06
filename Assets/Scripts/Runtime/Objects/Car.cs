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
        private GridPosition _gridPosition;
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
            if (!_isAvailable || !CarPlaceGrid.Instance.HasAvailableSlot()) return;

            LevelGrid.Instance.CarMovedGridPosition(_gridPosition);

            CoreGameEvents.Instance.onNewFreeSpace?.Invoke(_gridPosition);
            CoreGameEvents.Instance.onCarClicked?.Invoke(this);

            _gridPosition = new GridPosition(-99, -99);
        }

        public void Initialize(CarSO carSo, GridPosition gridPosition, GridObject gridObject)
        {
            _carSO = carSo;
            _gridPosition = gridPosition;
            _gridObject = gridObject;

            _renderer.sprite = _carSO.CarSprite;
            _rectTransform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);

            _gridObject.SetIsInteractable(false);
        }
        
        public void MoveToGridPosition(GridObject targetGridObject, TweenCallback onComplete = null)
        {
            // Optional: Add pathfinding logic later
            var targetPosition = CarPlaceGrid.Instance.GetWorldPosition(targetGridObject.GetGridPosition());

            transform.DOMove(targetPosition, 0.5f)
                .OnComplete(onComplete);
        }

        public void SetIsAvailableCar(bool isAvailable)
        {
            _isAvailable = isAvailable;
            _renderer.color = _isAvailable ? normalColor : fadeColor;
        }
        
        public bool GetIsAvailable() => _isAvailable;
        public CarSO GetCarSo() => _carSO;
        public GridPosition GetGridPosition() => _gridPosition;
        
    }
}