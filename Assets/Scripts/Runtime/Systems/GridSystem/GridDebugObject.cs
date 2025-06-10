using TMPro;
using UnityEngine;

namespace Runtime.Systems.GridSystem
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coordinateText;

        private GridObject _gridObject;

        public virtual void SetGridObject(GridObject gridObject)
        {
            _gridObject = gridObject;
        }

#if UNITY_EDITOR
        protected virtual void Update()
        {
            coordinateText.text = _gridObject.ToString();
        }
#endif

    }
}
