using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject.Car;
using Runtime.Managers;
using Runtime.Objects;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Controllers
{
    /// <summary>
    /// Controls the movement and move animations of car objects within the game
    /// </summary>
    public class CarMoveController : MonoBehaviour
    {
        private CarMoveData _moveData;
        private CarAnimationData _animationData;

        private CarObject _carObject;

        private Tween _currentPathTween;
        
        /// <summary>
        /// Initializes the cars movement and animation data
        /// </summary>
        public void Initialize(CD_CAR data, CarObject carObject)
        {
            _moveData = data.moveData;
            _animationData = data.animationData;
            _carObject = carObject;
        }
        
        /// <summary>
        /// Moves the car along a given list of grid positions
        /// Optionally executes a callback function upon completion
        /// </summary>
        public void MoveToGridPosition(List<Vector3> path, TweenCallback onComplete = null)
        {
            _currentPathTween.Kill();
            float duration = path.Count * _moveData.MoveDurationEachGrid;
            duration = Mathf.Min(duration, _moveData.MaxMoveDuration);
            _currentPathTween = transform.DOLocalPath(path.ToArray(), duration).OnComplete(onComplete);
        }
        
        /// <summary>
        /// Moves the car from one grid position to another by a single step
        /// If a movement tween is active, it cancels the current tween and resets the position
        /// </summary>
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

            MoveVisualCar(targetPosition, duration);
        }

        /// <summary>
        /// Plays a blast animation, moving the car up and to the side 
        /// </summary>
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
                // ParticleFX
                
                _carObject.ReleasePool();
            });
            
            sequence
                .Append(ScaleVisualCard(_animationData.UpScale, _animationData.BlastAnimationDuration))
                .Join(MoveVisualCar(goPosY, _animationData.BlastAnimationDuration))
                .Append(ScaleVisualCard(_animationData.UDownScale, _animationData.BlastAnimationDuration))
                .Join(MoveVisualCar(goPosX, _animationData.BlastAnimationDuration));
        }
        
        /// <summary>
        /// Moves the cars visual GameObject to a specified local position over a given duration
        /// </summary>
        private Tween MoveVisualCar(Vector3 pos, float duration)
        {
            return transform.DOLocalMove(pos, duration)/*.SetEase(_moveData.EaseMode)*/;
        }
        
        /// <summary>
        /// Scales the cars visual GameObject to a target scale value over a given duration
        /// </summary>
        private Tween ScaleVisualCard(float target, float duration)
        {
            return transform.DOScale(target, duration)/*.SetEase(_animationData.EaseMode)*/;
        }
        
    }
}