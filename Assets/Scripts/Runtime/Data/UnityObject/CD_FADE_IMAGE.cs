using Runtime.Data.ValueObject;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    /// <summary>
    /// ScriptableObject representing the data for a Fade Image
    /// </summary>
    [CreateAssetMenu(fileName = "CD_FADE_IMAGE", menuName = "Data/CD_FADE_IMAGE", order = 0)]
    public class CD_FADE_IMAGE : ScriptableObject
    {
        public FadeImageData FadeImageData;
    }
}