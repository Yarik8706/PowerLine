using System;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public enum Direction
    {
        Right,
        Left,
        Top,
        Bottom,
        None
    }

    public class PowerLineElement : MonoBehaviour
    {
        public Direction[] directions;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform rotationObject;
        
        public Vector2 Position { get; set; }
        public bool IsActive { get; protected set; }

        private int _rotationZAngle;
        private Tween _activeRotateTween;

        protected virtual void Awake()
        {
            if (rotationObject == null)
            {
                rotationObject = transform;
            }
        }

        protected virtual void Start()
        {
            
        }

        public virtual void SetActive(bool active)
        {
            IsActive = active;
            spriteRenderer.color = active ? Constantions.ActiveElementColor : Constantions.InactiveElementActive;
        }

        private void OnMouseDown()
        {
            _activeRotateTween?.Kill();
            if (!LevelControl.Instance.IsGameStarted)
            {
                LevelControl.Instance.InitGame();
                return;
            }
            if(LevelControl.Instance.IsGameFinished) return;
            RotateWithoutSetRotateAndAnimation();
            _activeRotateTween = rotationObject.DORotate(Vector3.forward * _rotationZAngle, 0.5f);
            _activeRotateTween.OnComplete(()
                => LevelControl.Instance.UpdatePowerLineState());
        }

        private void OnDestroy()
        {
            _activeRotateTween?.Kill();
        }

        public void SetActiveRotate()
        {
            var transformRotation = Quaternion.identity;
            transformRotation.eulerAngles = Vector3.forward * _rotationZAngle;
            rotationObject.rotation = transformRotation;
        }

        public void RotateWithoutSetRotateAndAnimation()
        {
            _rotationZAngle += -90;
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = GetDirectionAfterRotate90(directions[i]);
            }
        }

        public static Direction GetDirectionAfterRotate90(Direction directionChanged)
        {
            return directionChanged switch
            {
                Direction.Right => Direction.Bottom,
                Direction.Left => Direction.Top,
                Direction.Top => Direction.Right,
                Direction.Bottom => Direction.Left,
                _ => Direction.Left
            };
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Right => Direction.Left,
                Direction.Left => Direction.Right,
                Direction.Top => Direction.Bottom,
                Direction.Bottom => Direction.Top,
                _ => Direction.Left
            };
        }

        public static Vector2 GetVectorByDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Right => Vector2.right,
                Direction.Left => Vector2.left,
                Direction.Top => Vector2.up,
                Direction.Bottom => Vector2.down,
                _ => Vector2.zero
            };
        }
    }
}