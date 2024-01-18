using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class ShowMenuButton : MonoBehaviour
    {
        private ActiveElement _showMenuButtonActiveElement;
        [SerializeField] private ActiveElement[] elements;

        public static ShowMenuButton Instance;

        private void Awake()
        {
            _showMenuButtonActiveElement = GetComponent<ActiveElement>();
            Instance = this;
        }

        public void ChangeActive(bool menuActive)
        {
            transform.DORotate(Vector3.forward * 180, 0.5f).OnComplete(() =>
            {
                transform.DORotate(Vector3.zero, 0.5f);
            });
            _showMenuButtonActiveElement.ChangeActive(!menuActive);
            foreach (var element in elements)
            {
                element.ChangeActive(menuActive);
            }
        }
    }
}