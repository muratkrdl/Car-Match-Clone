using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data.UnityObject.SO
{
    /// <summary>
    /// ScriptableObject representing a car type and sprite
    /// </summary>
    [CreateAssetMenu(fileName = "CarSO", menuName = "SO/CarSO")]
    public class CarSO : ScriptableObject
    {
        public CarTypes Type;
        public Sprite CarSprite;
    }
}