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

        private bool _isAvailable;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Image>();
            _button = GetComponentInChildren<Button>();
            _rectTransform = GetComponent<RectTransform>();
            
            _button.onClick.AddListener(OnClick_Button);
        }

        private void OnClick_Button()
        {
            if (!_isAvailable || !CarPlaceGrid.Instance.GetAvailableSlot()) return;
            
            LevelGrid.Instance.SetNullCarAtGridPosition(_gridPosition);
            CoreGameEvents.Instance.onCarMoved?.Invoke(_gridPosition);
            CoreGameEvents.Instance.onCarClicked?.Invoke(this);
            _gridPosition = new GridPosition(-99, -99);
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onCarMoved += OnCarMoved;
        }

        private void OnCarMoved(GridPosition arg0)
        {
            if (!_gridPosition.IsNearBy(arg0)) return;

            SetIsAvailable(true);
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onCarMoved -= OnCarMoved;
        }

        public void Initialize(CarSO carSo, GridPosition gridPosition)
        {
            _carSO = carSo;
            _renderer.sprite = _carSO.CarSprite;
            _renderer.color = fadeColor;
            _gridPosition = gridPosition;

            _rectTransform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
                
            if (LevelGrid.Instance.CheckAroundOfGridPosition(_gridPosition) != _gridPosition)
            {
                SetIsAvailable(true);
            }
        }

        public void MoveToGridPosition(GridPosition gridPosition)
        {
            LevelGrid.Instance.CarMovedGridPosition(this, _gridPosition);
            
            // TODO : Pathfind
            
        }

        public void DoMoveToPos(Vector3 pos)
        {
            transform.DOMove(pos,1);
        }

        public void SetIsAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
            Color newColor = _isAvailable ? normalColor : fadeColor;
            _renderer.color = newColor;
        }
        public bool GetIsAvailable() => _isAvailable;
        public CarSO GetCarSo() => _carSO;
        public GridPosition GetGridPosition() => _gridPosition;
        
    }
}