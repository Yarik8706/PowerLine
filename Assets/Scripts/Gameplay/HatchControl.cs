using System;
using UnityEngine;

namespace Gameplay
{
    public class HatchControl : MonoBehaviour
    {
        [SerializeField] private GameObject hatch;

        private GameObject[] _activeHatches = Array.Empty<GameObject>();
        
        public bool IsHatchActive { get; private set; }
        public static HatchControl Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void ChangeHatchActive(bool active)
        {
            IsHatchActive = !IsHatchActive;
            YandexGameControl.SaveOtherData();
            foreach (var activeHatch in _activeHatches)
            {
                activeHatch.SetActive(IsHatchActive);
            }
        }

        public void GenerateNewHatches()
        {
            foreach (var activeHatch in _activeHatches)
            {
                Destroy(activeHatch);
            }
            _activeHatches = new GameObject[LevelControl.Instance.PowerLineElements.Count];
            for (int i = 0; i < _activeHatches.Length; i++)
            {
                _activeHatches[i] = Instantiate(hatch, LevelGenerationControl.Instance.powerLineElementsContainer);
                _activeHatches[i].transform.localPosition = LevelControl.Instance.PowerLineElements[i].transform.localPosition;
                _activeHatches[i].SetActive(IsHatchActive);
            }
        }
    }
}