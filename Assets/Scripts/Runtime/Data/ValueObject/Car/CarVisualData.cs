using System;
using UnityEngine;

namespace Runtime.Data.ValueObject.Car
{
    /// <summary>
    /// Car Visual Data
    /// </summary>
    [Serializable]
    public struct CarVisualData
    {
        public Color fadeColor;
        public Color normalColor;
    }
}