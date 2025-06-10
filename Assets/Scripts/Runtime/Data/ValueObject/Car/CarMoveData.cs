using System;

namespace Runtime.Data.ValueObject.Car
{
    [Serializable]
    public struct CarMoveData
    {
        public float MoveDurationEachGrid;
        public float MaxMoveDuration;
    }
}