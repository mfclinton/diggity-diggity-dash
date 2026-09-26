using System;
using System.Collections.Generic;

namespace Game.Racing.Tournament
{
    [Serializable]
    public class RacerTournamentData
    {
        public Racer RacerPrefab;
        public List<int> RaceResults;
        
        public RacerTournamentData(Racer racerPrefab)
        {
            RacerPrefab = racerPrefab;
            RaceResults = new List<int>();
        }
    }
}