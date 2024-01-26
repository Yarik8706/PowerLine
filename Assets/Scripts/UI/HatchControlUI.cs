using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HatchControlUI : MonoBehaviour
    {
        [SerializeField] private Image hatchIconImage;
        [SerializeField] private Sprite activeHatchIcon;
        [SerializeField] private Sprite disableHatchIcon;

        public static HatchControlUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ChangeHatchActive()
        {
            HatchControl.Instance.ChangeHatchActive(!HatchControl.Instance.IsHatchActive);
            hatchIconImage.sprite = HatchControl.Instance.IsHatchActive ? activeHatchIcon : disableHatchIcon;
        }
    }
}