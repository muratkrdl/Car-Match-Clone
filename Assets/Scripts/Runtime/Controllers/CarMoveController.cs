using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Managers;
using Runtime.Objects;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Controllers
{
    public class CarMoveController : MonoBehaviour
    {
        private CarMoveData _moveData;
        private CarAnimationData _animationData;

        private CarObject _carObject;

        private Tween _currentPathTween;
        
        public void Initialize(CD_CAR data, CarObject carObject)
        {
            _moveData = data.moveData;
            _animationData = data.animationData;
            _carObject = carObject;
        }
        
        // First Position To CarPlaceGridPosition
        public void MoveToGridPosition(List<Vector3> path, TweenCallback onComplete = null)
        {
            _currentPathTween.Kill();
            float duration = path.Count * _moveData.MoveDurationEachGrid;
            duration = Mathf.Min(duration, _moveData.MaxMoveDuration);
            _currentPathTween = transform.DOLocalPath(path.ToArray(), duration).OnComplete(onComplete);
        }

        // Slide CarPlaceGridPosition
        public void MoveSingleToGridPosition(Vector2Int from, Vector2Int to)
        {
            float duration = Mathf.Abs(from.x - to.x) * _moveData.MoveDurationEachGrid;
            Vector3 targetPosition = LevelGridManager.Instance.GetWorldPosition(to);
            
            if (_currentPathTween.active)
            {
                _currentPathTween.Kill();
                duration = _moveData.MoveDurationEachGrid;
                targetPosition = LevelGridManager.Instance.GetWorldPosition(_carObject.GetCoordinates());
            }

            MoveVisualCard(targetPosition, duration);
        }

        public void BlastAnimation(Vector2Int pos, Vector2 cellSize)
        {
            Vector3 goPosY = transform.localPosition + ConstantsUtilities.Up3 * cellSize.y;
            Vector3 goPosX = pos.x.CompareTo(_carObject.GetCoordinates().x) switch
            {
                1 => ConstantsUtilities.Right3,
                -1 => ConstantsUtilities.Left3,
                _ => ConstantsUtilities.Zero3
            };
            goPosX *= cellSize.x;
            goPosX += goPosY;
            
            Sequence sequence = DOTween.Sequence().OnComplete(() =>
            {
                // Get ParticleFX From Pool
                
                _carObject.ReleasePool();
            });
            
            sequence
                .Append(ScaleVisualCard(_animationData.UpScale, _animationData.BlastAnimationDuration))
                .Join(MoveVisualCard(goPosY, _animationData.BlastAnimationDuration))
                .Append(ScaleVisualCard(_animationData.UDownScale, _animationData.BlastAnimationDuration))
                .Join(MoveVisualCard(goPosX, _animationData.BlastAnimationDuration));
        }
        
        private Tween MoveVisualCard(Vector3 pos, float duration)
        {
            return transform.DOLocalMove(pos, duration)/*.SetEase(_moveData.EaseMode)*/;
        }
        
        private Tween ScaleVisualCard(float target, float duration)
        {
            return transform.DOScale(target, duration)/*.SetEase(_animationData.EaseMode)*/;
        }
        
    }
}