using System;
using UnityEngine;

namespace Game.Racing.Physics
{
    [RequireComponent(typeof(Collider2D))]
    public class FinishLineTrigger : MonoBehaviour
    {
        private RaceManager raceManager;

        private void Awake()
        {
            raceManager = FindFirstObjectByType<RaceManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (raceManager == null)
                return;
                
            RacerCollider racerCollider = other.GetComponent<RacerCollider>();
            if (racerCollider == null)
                return;

            Racer racer = racerCollider.Racer;
            raceManager.RacerFinished(racer);
        }
    }
}