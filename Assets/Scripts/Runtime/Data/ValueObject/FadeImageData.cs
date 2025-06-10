using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Data.ValueObject
{
    [Serializable]
    public struct FadeImageData
    {
        public Color NormalColor;
        public Color FadeColor;
        public float FadeDuration;
        public float WaitNextDuration;
        public Ease EaseMode;
    }
}