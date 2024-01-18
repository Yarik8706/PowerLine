using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SceneBlackoutControlUI : MonoBehaviour
    {
        [SerializeField] private Transform blackout;

        public static SceneBlackoutControlUI Instance { get; private set; }

        private Color _startFade;
        private int _blackoutMaxLenght;
        private Image _blackoutImage;

        private void Awake()
        {
            _blackoutImage = blackout.GetComponent<Image>();
            _startFade = _blackoutImage.color;
            Instance = this;
        }

        private void Start()
        {
            _blackoutMaxLenght = (Screen.height > Screen.width ? Screen.height : Screen.width) * 2;
            StartBlackoutOverTime(blackout.transform.position);
        }

        public void StartBlackoutOverTime(Vector3 startPosition)
        {
            blackout.gameObject.SetActive(true);
            blackout.transform.position = startPosition;
            _blackoutImage.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() =>
            {
                _blackoutImage.color = _startFade;
            });
            blackout.transform.DOScale(
                new Vector3(_blackoutMaxLenght,
                    _blackoutMaxLenght, 1), 0.75f).OnKill(() =>
            {
                blackout.transform.localScale = Vector3.one;
                blackout.gameObject.SetActive(false);
            });
        }
    }
}