using System;
using System.Collections.Generic;
using System.Linq;
using Game.Racing.Data;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Racing.Tournament
{
    public class TournamentManager
    {
        // Configuration
        private int totalRaces = 5;
        private Racer[] racerPrefabs;
        
        // State
        private List<RacerTournamentData> racerData = new();
        private int currentRace = 0;
        
        // Properties
        public IReadOnlyList<RacerTournamentData> RacersData => racerData;
        public int CurrentRace => currentRace;
        
        public TournamentManager(Racer[] racerPrefabs, int totalRaces)
        {
            this.racerPrefabs = racerPrefabs;
            this.totalRaces = totalRaces;
        }
        
        public void StartTournament()
        {
            currentRace = 0;
            
            // Initialize Racers
            racerData.Clear();
            foreach (var racerPrefab in racerPrefabs)
            {
                var tournamentData = new RacerTournamentData(racerPrefab);
                racerData.Add(tournamentData);
            }
            
            Debug.Log($"Tournament started with {racerData.Count} racers");
        }
        
        public void AdvanceToNextRace()
        {
            currentRace++;
            
            Debug.Log($"Advanced to race {currentRace} of {totalRaces}");
        }
        
        public void RecordRaceResults(List<Racer> spawnedRacers)
        {
            var sortedRacers = spawnedRacers.OrderBy(r => r.RaceData.finishTime);

            var finishers = sortedRacers.Where(r => r.RaceData.hasFinished).ToList();
            var nonFinishers = sortedRacers.Where(r => !r.RaceData.hasFinished)
                                           .OrderByDescending(r => r.transform.Find("Agent").position.x)
                                           .ToList();

            var finalSortedRacers = finishers.Concat(nonFinishers);

            int points = spawnedRacers.Count;
            foreach (var racer in finalSortedRacers)
            {
                var tournamentData = racerData.FirstOrDefault(data => data.RacerPrefab.ProfileData.racerName == racer.ProfileData.racerName);
                
                tournamentData.RaceResults.Add(points);
                points--;
            }
        }
    }
}