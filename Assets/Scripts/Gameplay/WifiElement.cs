using UnityEngine;

namespace Gameplay
{
    public class WifiElement : LampElement
    {
        public WifiElement otherWifiElement;

        protected override void Start()
        {
            LevelControl.Instance.UpdatePowerLineStateEvent.AddListener(ChargeNearbyObjects);
            base.Start();
        }

        public void InitTwoWifi(WifiElement otherWifi)
        {
            otherWifiElement = otherWifi;
            otherWifi.otherWifiElement = this;
        }

        protected virtual void ChargeNearbyObjects()
        {
            if(!IsActive) return;
            EnergyElement.SetPowerLineElementStateAndContinueActivation(this, Direction.None);
        }
        
        public override void SetActive(bool active)
        {
            if (active && !IsActive)
            {
                IsActive = true;
                otherWifiElement.SetActive(true);
                otherWifiElement.ChargeNearbyObjects();
            }
            IsActive = active;
            base.SetActive(active);
        }
    }
}