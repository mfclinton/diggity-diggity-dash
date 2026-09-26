using System;
using Game.Racing.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Racing
{
    public class Racer : MonoBehaviour
    {
        [Header("Racer Profile")]
        [SerializeField] private RacerProfileData profileData;
        
        [Header("Controller References")]
        [SerializeField] private Component[] controllerComponents;
        
        // Data
        public RacerProfileData ProfileData => profileData;
        public RacerRaceData RaceData { get; private set; }

        private void Awake()
        {
            RaceData = new RacerRaceData();
        }
        
        public void SetControllerActive(bool isActive)
        {
            foreach (var component in controllerComponents)
                if (component is MonoBehaviour monoBehaviour)
                    monoBehaviour.enabled = isActive;
        }
    }
}