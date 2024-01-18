using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChangeLevelDifficultUI : MonoBehaviour
    {
        [SerializeField] private Color baseColorButtonImage;
        [SerializeField] private Color hideColorButtonImage;
        [SerializeField] private Image nextButtonImage;
        [SerializeField] private Image prevButtonImage;

        public static ChangeLevelDifficultUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateState()
        {
            if (LevelControl.Instance.LevelDifficultIndex == 0)
            {
                nextButtonImage.color = baseColorButtonImage;
                prevButtonImage.color = hideColorButtonImage;
            } else if(LevelControl.Instance.LevelDifficultIndex == LevelControl.Instance.difficultLevelsData.Length - 1)
            {
                nextButtonImage.color = hideColorButtonImage;
                prevButtonImage.color = baseColorButtonImage;
            }
            else
            {
                nextButtonImage.color = baseColorButtonImage;
                prevButtonImage.color = baseColorButtonImage;
            }
        }

        public void NextDifficult()
        {
            if(LevelGenerationControl.Instance.isGenerateLvl) return;
            var newDifficult = LevelControl.Instance.LevelDifficultIndex + 1;
            switch (newDifficult)
            {
                case 3:
                    return;
                case 2:
                    nextButtonImage.color = hideColorButtonImage;
                    break;
            }

            prevButtonImage.color = baseColorButtonImage;
            SceneBlackoutControlUI.Instance.StartBlackoutOverTime(nextButtonImage.transform.position);
            LevelControl.Instance.ChangeDifficultAndGenerateLvl(newDifficult);
        }

        public void PrevDifficult()
        {
            if(LevelGenerationControl.Instance.isGenerateLvl) return;
            var newDifficult = LevelControl.Instance.LevelDifficultIndex - 1;
            switch (newDifficult)
            {
                case -1:
                    return;
                case 0:
                    prevButtonImage.color = hideColorButtonImage;
                    break;
            }

            nextButtonImage.color = baseColorButtonImage;
            SceneBlackoutControlUI.Instance.StartBlackoutOverTime(prevButtonImage.transform.position);
            LevelControl.Instance.ChangeDifficultAndGenerateLvl(newDifficult);
        }
    }
}