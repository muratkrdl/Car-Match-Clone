using System;
using UnityEngine;

namespace Editor
{
    [Serializable]
    public struct CellData
    {
        public Vector2Int position;
        public int colorIndex;
    }
}