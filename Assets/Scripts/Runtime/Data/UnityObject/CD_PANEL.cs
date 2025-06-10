using Runtime.Data.ValueObject;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_PANEL", menuName = "Data/CD_PANEL", order = 0)]
    public class CD_PANEL : ScriptableObject
    {
        public PanelPopUpData PanelPopUpData;
    }
}