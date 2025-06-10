using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Controllers;
using Runtime.Data.UnityObject;
using Runtime.Data.UnityObject.SO;
using Runtime.Data.ValueObject;
using Runtime.Data.ValueObject.Car;
using Runtime.Events;
using Runtime.Managers;
using Runtime.Systems.GridSystem;
using Runtime.Systems.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Objects
{
    public class CarObject : MonoBehaviour, IPoolObject<CarObject>
    {
        private CarVisualData _visualData;

        private Image _renderer;
        private Button _button;
        private RectTransform _rectTransform;
        private CarMoveController _carMoveController;

        private CarSO _carSO;
        private Vector2Int _coordinates;
        private GridObject _gridObject;

        private bool _isAvailable;
        
        private Tween _currentPathTween;

        private ObjectPool<CarObject> _pool;

        private void Awake()
        {
            GetReferences();
            GetData();
            _button.onClick.AddListener(OnClick_Button);
        }

        private void OnEnable()
        {
            CoreGameEvents.Instance.onResetLevel += ReleasePool;
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onResetLevel -= ReleasePool;
        }

        private void GetReferences()
        {
            _renderer = GetComponentInChildren<Image>();
            _button = GetComponentInChildren<Button>();
            _rectTransform = GetComponent<RectTransform>();
            _carMoveController = GetComponent<CarMoveController>();
        }

        private void GetData()
        {
            var data = Resources.Load<CD_CAR>("Data/CD_CAR");
            _carMoveController.Initialize(data, this);
            _visualData = data.visualData;
        }
        
        private void OnClick_Button()
        {
            if (!_isAvailable || !LevelGridManager.Instance.HasAvailableSlot()) return;
            
            _isAvailable = false;

            LevelGridEvents.Instance.onCarClicked?.Invoke(this);
        }

        public void Initialize(CarSO carSo, Vector2Int gridPosition, GridObject gridObject, Transform parent)
        {
            transform.SetParent(parent);
            
            _carSO = carSo;
            _coordinates = gridPosition;
            _gridObject = gridObject;

            _renderer.sprite = _carSO.CarSprite;
            _rectTransform.position = LevelGridManager.Instance.GetWorldPosition(gridPosition);

            _gridObject.SetIsWalkable(false);
        }
        
        // First Position To CarPlaceGridPosition
        public void MoveToGridPosition(List<Vector3> path, TweenCallback onComplete = null)
        {
            _carMoveController.MoveToGridPosition(path, onComplete);
        }

        // Slide CarPlaceGridPosition
        public void MoveSingleToGridPosition(Vector2Int from, Vector2Int to)
        {
            _carMoveController.MoveSingleToGridPosition(from, to);
        }

        // Blast Animation
        public void BlastAnimation(Vector2Int pos, Vector2 cellSize)
        {
            _carMoveController.BlastAnimation(pos, cellSize);
        }

        public void SetIsAvailableCar(bool isAvailable)
        {
            _isAvailable = isAvailable;
            _renderer.color = _isAvailable ? _visualData.normalColor : _visualData.fadeColor;
        }
        
        public CarSO GetCarSo() => _carSO;
        public Vector2Int GetCoordinates() => _coordinates;
        public void SetCoordinates(Vector2Int coordinates) => _coordinates = coordinates;

        public void SetPool(ObjectPool<CarObject> pool) => _pool = pool;
        public void ReleasePool() => _pool.Release(this);

    }
}