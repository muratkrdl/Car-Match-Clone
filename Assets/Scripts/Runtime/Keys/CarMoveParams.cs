using Runtime.Objects;
using UnityEngine;

namespace Runtime.Keys
{
    /// <summary>
    /// Represents parameters for a car movement action
    /// </summary>
    public struct CarMoveParams
    {
        public CarObject CarObject;
        public Vector2Int From;
        public Vector2Int To;
    }
}