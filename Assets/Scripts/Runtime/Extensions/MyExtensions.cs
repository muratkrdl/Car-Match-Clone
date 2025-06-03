using UnityEngine;

namespace Runtime.Extensions
{
    public static class MyExtensions
    {
        public static bool LayerMaskContains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}