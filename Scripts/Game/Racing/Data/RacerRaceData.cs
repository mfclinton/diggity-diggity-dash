using System;
using UnityEngine;

namespace Game.Racing.Data
{
    [Serializable]
    public class RacerRaceData
    {
        // Initialized State
        public Vector3 startPosition;
        public float startTime;
        
        // Active State
        public bool hasFinished = false;
        public bool isRacing = false;
        
        // Finished State
        public float finishTime;
        
        // Performance metrics
        public float RaceTime => finishTime - startTime;
        
        public void Reset(Vector3 startPosition)
        {
            this.startPosition = startPosition;
            this.startTime = -1f;
            
            this.hasFinished = false;
            this.finishTime = -1f;
            
            this.isRacing = false;
        }
        
        public void StartRacing(float startTime)
        {
            this.startTime = startTime;
            this.isRacing = true;
            this.hasFinished = false;
        }
        
        public void FinishRace(float finishTime)
        {                
            this.finishTime = finishTime;
            this.hasFinished = true;
        }
    }
}