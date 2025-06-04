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
        private RectTransform _rectTransform;
        
        private CarSO _carSO;
        private GridPosition _gridPosition;

        private bool isAvailable;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Image>();
            _rectTransform = GetComponent<RectTransform>();
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
                SetIsAvailable(_gridPosition.IsNearBy(gridPosition));
            }
        }

        public void MoveToGridPosition(GridPosition gridPosition)
        {
            LevelGrid.Instance.CarMovedGridPosition(this, _gridPosition, gridPosition);
            
            // TODO : Pathfind
            
        }

        public void SetIsAvailable(bool isAvailable)
        {
            this.isAvailable = isAvailable;
            Color newColor = isAvailable ? fadeColor : normalColor;
            _renderer.color = newColor;
        }
        public bool GetIsAvailable() => isAvailable;
        
    }
}