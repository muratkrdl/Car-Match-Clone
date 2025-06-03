using Runtime.Data.ValueObject;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_CAR", menuName = "Data/CD_CAR")]
    public class CD_CAR : ScriptableObject
    {
        public CarData Data;
    }
}