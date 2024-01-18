using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreCounterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        public int Score { get; private set; } = 1;

        public static ScoreCounterUI Instance;

        private void Awake()
        {
            Instance = this;
            UpdateScore(1);
        }

        public void UpdateScore(int score)
        {
            Score = score;
            scoreText.text = Score + "";
        }

        public void AddScore(int difficultIndex)
        {
            var addScore = 1;
            switch (difficultIndex)
            {
                case 1:
                    addScore = 3;
                    break;
                case 2:
                    addScore = 5;
                    break;
            }
            
            Score += addScore;
            YandexGameControl.Instance.SaveDataScore();
            scoreText.transform.DOScale(scoreText.transform.localScale * 1.2f, 0.5f).OnComplete(() =>
            {
                scoreText.text = Score + "";
                scoreText.transform.DOScale(scoreText.transform.localScale * 0.8f, 0.5f);
            }).SetDelay(0.4f);
        }
    }
}