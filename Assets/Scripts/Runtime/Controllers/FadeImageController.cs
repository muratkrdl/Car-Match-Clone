using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Controllers
{
    public class FadeImageController : MonoBehaviour
    {
        private Image _renderer;

        private Color _normalColor;
        private Color _fadeColor;

        private void Awake()
        {
            _renderer = GetComponent<Image>();
            _normalColor = _renderer.color;
            _fadeColor = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0f);
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            DoFadeScreen(_fadeColor, .5f);
        }

        private void DoFadeScreen(Color newColor, float duration)
        {
            _renderer.DOColor(newColor, duration);
        }
        
    }
}