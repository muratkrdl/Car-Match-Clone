using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CarSO", menuName = "SO/CarSO")]
    public class CarSO : ScriptableObject
    {
        public CarTypes Type;
        public Sprite CarSprite;
    }
}