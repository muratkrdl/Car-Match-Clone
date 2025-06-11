using Runtime.Data.ValueObject;
using Runtime.Data.ValueObject.Car;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    /// <summary>
    /// ScriptableObject representing the data for a car object
    /// </summary>
    [CreateAssetMenu(fileName = "CD_CAR", menuName = "Data/CD_CAR")]
    public class CD_CAR : ScriptableObject
    {
        public CarMoveData moveData;
        public CarAnimationData animationData;
        public CarVisualData visualData;
    }
}