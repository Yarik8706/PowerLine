using System;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.Events;
using YG;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class LevelControl : MonoBehaviour
    {
        public Vector2Int[] difficultLevelsData;
        
        [SerializeField] private ActiveElement showMenuButton;

        private float _approximationAmount = 0.1f;

        private Vector3 baseGameZoneScale;
        private Vector3 activeGameZoneScale;

        public bool IsGameFinished { get; private set; }
        public bool IsGameStarted { get; private set; }
        public int LevelDifficultIndex { get; private set; }
        public List<PowerLineElement> PowerLineElements { get; } = new();
        public UnityEvent UpdatePowerLineStateEvent { get; } = new();
        public static LevelControl Instance { get; private set; }

        private void Awake()
        {
            if (Camera.main.pixelHeight < Camera.main.pixelWidth)
            {
                _approximationAmount = 0.25f;
                Camera.main.fieldOfView = 45 * (16f/9f) / ((float)Camera.main.pixelWidth / Camera.main.pixelHeight);
            }
            else
            {
                Camera.main.fieldOfView = 55 * (16f/9f) / ((float)Camera.main.pixelHeight / Camera.main.pixelWidth);
            }
            Instance = this;
        }

        private void Update()
        {
            if (!IsGameFinished) return;
            if (!Input.GetMouseButtonDown(0) &&
                            (Input.touches.Length == 0 || Input.touches[0].phase != TouchPhase.Began)) return;
            StartNewGame();
        }

        private void StartNewGame()
        {
            ScoreCounterUI.Instance.AddScore(LevelDifficultIndex);
            ShowMenuButton.Instance.ChangeActive(true);
            BackgroundGradientControl.Instance.SetRandomGradient();
            SceneBlackoutControlUI.Instance.StartBlackoutOverTime(Input.mousePosition);
            IsGameFinished = false;
            IsGameStarted = false;
            LevelGenerationControl.Instance.powerLineElementsContainer.localScale = Vector3.one;
            LevelGenerationControl.Instance.GenerateLvl(difficultLevelsData[LevelDifficultIndex]);
            baseGameZoneScale = LevelGenerationControl.Instance.powerLineElementsContainer.localScale;
            activeGameZoneScale = baseGameZoneScale + baseGameZoneScale * _approximationAmount;
        }

        public void InitGame()
        {
            LevelGenerationControl.Instance.powerLineElementsContainer.DOScale(
                activeGameZoneScale,
                0.8f
            );
            IsGameStarted = true;
            ShowMenuButton.Instance.ChangeActive(false);
        }

        public void StopGame()
        {
            LevelGenerationControl.Instance.powerLineElementsContainer.DOScale(
                baseGameZoneScale,
                0.8f
            );
            IsGameStarted = false;
            ShowMenuButton.Instance.ChangeActive(true);
        }

        public void UpdatePowerLineState()
        {
            foreach (var element in PowerLineElements)
            {
                element.SetActive(false);
            }
            UpdatePowerLineStateEvent.Invoke();
            UpdateWinState();
        }

        private void UpdateWinState()
        {
            foreach (var powerLineElement in PowerLineElements)
            {
                if(!powerLineElement.IsActive) return;
            }

            LevelGenerationControl.Instance.powerLineElementsContainer.DOScale(
                baseGameZoneScale,
                0.8f);
            IsGameFinished = true;
            showMenuButton.ChangeActive(false);
        }

        public void ChangeDifficultAndGenerateLvl(int newDifficult)
        {
            YandexGameControl.Instance.SaveOtherData();
            LevelDifficultIndex = newDifficult;
            LevelGenerationControl.Instance.GenerateLvl(difficultLevelsData[newDifficult]);
            baseGameZoneScale = LevelGenerationControl.Instance.powerLineElementsContainer.localScale;
            activeGameZoneScale = baseGameZoneScale + baseGameZoneScale * _approximationAmount;
        }
    }
}