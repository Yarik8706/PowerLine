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
        StartCoroutine(LoadYandexGameData());
    }

    public static void GetLeaderboardData()
    {
        YandexGame.GetLeaderboard("GlobalScore",
                 Int32.MaxValue, Int32.MaxValue, Int32.MaxValue, "nonePhoto");
    }

    private IEnumerator LoadYandexGameData()
    {
        yield return new WaitUntil(() => YandexGame.SDKEnabled);
        GetLeaderboardData();
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

    public static void SaveOtherData()
    {
        YandexGame.savesData.isNeedCellHatch = HatchControl.Instance.IsHatchActive;
        YandexGame.savesData.isNeedMusic = MusicControl.Instance.MusicActive;
        YandexGame.savesData.currentDifficultIndex = LevelControl.Instance.LevelDifficultIndex;
        YandexGame.SaveProgress();
    }
    
    public static void SaveDataScore()
    {
        YandexGame.savesData.score = ScoreCounterUI.Instance.Score;
        YandexGame.SaveProgress();
        YandexGame.NewLeaderboardScores("GlobalScore", ScoreCounterUI.Instance.Score);
    }
}