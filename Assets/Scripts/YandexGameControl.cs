using System;
using System.Collections;
using Gameplay;
using UI;
using UnityEngine;
using YG;

public class YandexGameControl : MonoBehaviour
{
    public static YandexGameControl Instance { get; private set; } 
    
    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        YandexGame.ResetSaveProgress();
#endif
        StartCoroutine(LoadPassedLvlsName());
    }

    private IEnumerator LoadPassedLvlsName()
    {
        yield return new WaitUntil(() => YandexGame.SDKEnabled);
        if (!YandexGame.savesData.isFirstSession)
        {
            ScoreCounterUI.Instance.UpdateScore(YandexGame.savesData.score);
            if (HatchControl.Instance.IsHatchActive != YandexGame.savesData.isNeedCellHatch)
            {
                HatchControlUI.Instance.ChangeHatchActive();
            }
            if (MusicControl.Instance.MusicActive != YandexGame.savesData.isNeedMusic)
            {
                MusicControl.Instance.ChangeMusicActive();
            }
            LevelControl.Instance.ChangeDifficultAndGenerateLvl(YandexGame.savesData.currentDifficultIndex);
            ChangeLevelDifficultUI.Instance.UpdateState();
        }
        else
        {
            ScoreCounterUI.Instance.UpdateScore(1);
            LevelControl.Instance.ChangeDifficultAndGenerateLvl(0);
        }
    }

    public void SaveOtherData()
    {
        YandexGame.savesData.isNeedCellHatch = HatchControl.Instance.IsHatchActive;
        YandexGame.savesData.isNeedMusic = MusicControl.Instance.MusicActive;
        YandexGame.savesData.currentDifficultIndex = LevelControl.Instance.LevelDifficultIndex;
        YandexGame.SaveProgress();
    }
    
    public void SaveDataScore()
    {
        YandexGame.savesData.score = ScoreCounterUI.Instance.Score;
        YandexGame.SaveProgress();
        YandexGame.NewLeaderboardScores("GlobalScore", ScoreCounterUI.Instance.Score);
    }
}