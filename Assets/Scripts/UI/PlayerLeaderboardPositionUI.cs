using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

namespace UI
{
    public class PlayerLeaderboardPositionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerRankText;
        [SerializeField] private Image rankImage;

        private int _currentRank;
        
        private void Start()
        {
            YandexGame.onGetLeaderboard += OnUpdateLB;
        }

        private void OnUpdateLB(LBData lb)
        {
            foreach (var player in lb.players)
            {
                if (player.uniqueID == YandexGame.playerId)
                {
                    if (_currentRank != player.rank)
                    {
                        playerRankText.transform.DOScale(playerRankText.transform.localScale * 1.2f, 0.5f).OnComplete(() =>
                        {
                            playerRankText.text = "" + player.rank;
                            playerRankText.transform.DOScale(playerRankText.transform.localScale * 0.8f, 0.5f);
                        }).SetDelay(0.4f);
                    }
                    else
                    {
                        playerRankText.text = "" + player.rank;
                    }

                    _currentRank = player.rank;
                    
                    switch (player.rank)
                    {
                        case 1:
                            rankImage.color = Color.yellow;
                            break;
                        case 2:
                            rankImage.color = Color.gray;
                            break;
                        case 3:
                            rankImage.color = new Color(0.59f, 0.29f, 0f);
                            break;
                    }
                    return;
                }
            }
            
            playerRankText.text = "" + (lb.players.Length + 1);
            _currentRank = lb.players.Length;
        }
    }
}