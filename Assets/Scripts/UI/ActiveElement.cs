using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class ActiveElement : MonoBehaviour
    {
        [SerializeField] private Transform hidePosition;
        [SerializeField] private bool hideInStart;
        
        public Vector3 ActivePosition { get; set; }
        
        private void Awake()
        {
            if(ActivePosition == Vector3.zero) ActivePosition = transform.position;
            if (hideInStart) transform.position = hidePosition.position;
        }

        public Tween ChangeActive(bool isActive)
        {
            return transform.DOMove(isActive ? ActivePosition : hidePosition.position, 0.7f).SetLink(gameObject)
                .SetEase(Ease.InOutExpo).Play();
        }
    }
}