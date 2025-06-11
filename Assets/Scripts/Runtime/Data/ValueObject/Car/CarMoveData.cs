using System;

namespace Runtime.Data.ValueObject.Car
{
    /// <summary>
    /// Car Move Data
    /// </summary>
    [Serializable]
    public struct CarMoveData
    {
        public float MoveDurationEachGrid;
        public float MaxMoveDuration;
    }
}