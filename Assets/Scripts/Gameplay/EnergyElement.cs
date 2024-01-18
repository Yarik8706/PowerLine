using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public sealed class EnergyElement : PowerLineElement
    {
        private static readonly List<PowerLineElement> CheckedElements = new();

        protected override void Awake()
        {
            LevelControl.Instance.UpdatePowerLineStateEvent.AddListener(ChargeNearbyObjects);
            base.Awake();
        }

        public override void SetActive(bool active)
        {
            base.SetActive(true);
        }

        private void ChargeNearbyObjects()
        {
            CheckedElements.Clear();
            SetPowerLineElementStateAndContinueActivation(this, Direction.None);
        }

        public static void SetPowerLineElementStateAndContinueActivation(PowerLineElement powerLineElement, Direction lastDirection)
        {
            if (CheckedElements.Contains(powerLineElement))
            {
                return;
            }
            CheckedElements.Add(powerLineElement);
            if(!powerLineElement.IsActive)powerLineElement.SetActive(true);
            foreach (var direction in powerLineElement.directions)
            {
                if (!GetConnectedPowerLineElementByDirection(direction, powerLineElement.Position,
                        out var element)) continue;
                if(lastDirection == GetOppositeDirection(direction)) continue;
                SetPowerLineElementStateAndContinueActivation(element, direction);
            }
        }

        private static bool GetConnectedPowerLineElementByDirection(Direction direction, Vector2 mainElementPosition, out PowerLineElement connectedPoswerLineElement)
        {
            connectedPoswerLineElement = null;
            var oppositeDirection = GetOppositeDirection(direction);
            var position = GetVectorByDirection(direction) 
                           + mainElementPosition;
            var cells = LevelControl.Instance.PowerLineElements.Where(
                _ => position == _.Position).ToArray();
            if (cells.Length == 0) return false;
            var cell = cells[0];
            if (cell.directions.All(cellDirection => cellDirection != oppositeDirection)) return false;
            connectedPoswerLineElement = cell;
            return true;
        }
    }
}