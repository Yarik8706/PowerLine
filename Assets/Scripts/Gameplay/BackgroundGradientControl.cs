using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class BackgroundGradientControl : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        
        private ColorAdjustments _colorAdjustments;

        public static BackgroundGradientControl Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetCameraComponents();
            SetRandomGradient();
        }

        private void SetCameraComponents()
        {
            foreach (var volumeComponent in volume.sharedProfile.components)
            {
                switch (volumeComponent)
                {
                    case ColorAdjustments colorAdjustments:
                        _colorAdjustments = colorAdjustments;
                        return;
                }
            }
        }

        public void SetRandomGradient()
        {
            while (true)
            {
                var randomNumber = Random.Range(-180, 180);
                if (Mathf.Abs(randomNumber - _colorAdjustments.hueShift.value) < 10)
                {
                    continue;
                }

                _colorAdjustments.hueShift.value = randomNumber;
                break;
            }
        }
    }
}