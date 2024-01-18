using System;
using UnityEngine;

namespace Gameplay
{
    public class LampElement: PowerLineElement
    {
        private SpriteRenderer _lampPartSpriteRenderer;

        protected override void Awake()
        {
            _lampPartSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void SetActive(bool active)
        {
            _lampPartSpriteRenderer.color = active ? Constantions.ActiveElementColor : Constantions.InactiveElementActive;
            base.SetActive(active);
        }
    }
}