using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Racing.Physics;
using Game.Racing.Tournament;
using Game.Terrain.DensityFunctions;
using Game.Terrain.Generation;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Racing
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private FinishLineTrigger finishLinePrefab;
        
        [Header("Positioning")]
        [SerializeField] private float racerSpacing = 2f;
        [SerializeField] private float finishLineWidth = 2f;
        
        [Header("Events")]
        [SerializeField] public UnityEvent<RaceState> OnRaceStateChanged;
        [SerializeField] public UnityEvent<Racer> OnRacerFinished;
        
        // References
        private TerrainGenerator terrainGenerator;
        private TournamentManager tournamentManager;
        
        // Internal References
        private List<Racer> spawnedRacers = new ();
        private FinishLineTrigger finishLine;

        // State Management
        private RaceState currentState = RaceState.Setup;
        
        // Accessors
        public List<Racer> SpawnedRacers => spawnedRacers;
        public RaceState CurrentState => currentState;
        
        #region Race State Function

        public void InitializeRace(TournamentManager tournamentManager, Texture2D mapTexture = null)
        {
            ChangeState(RaceState.Setup);
            
            // Get Tournament Manager
            this.tournamentManager = tournamentManager;
            
            // Terrain Setup
            terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
            
            if (mapTexture)
            {
                TextureDensityFunction textureDensityFunction = new TextureDensityFunction();
                textureDensityFunction.SetTexture(mapTexture);
                terrainGenerator.SetDensityFunction(textureDensityFunction);
            }

            terrainGenerator.Initialize();
            
            // Racers Setup
            SetupRacers();
            
            // Finish Line Setup
            SetupFinishLine();
        }
        
        public void PrepareRaceStart()
        {
            ChangeState(RaceState.Starting);
        }
        
        public void StartRace()
        {
            ChangeState(RaceState.Racing);
            
            float raceStartTime = Time.time;
            foreach (Racer racer in spawnedRacers)
            {
                racer.RaceData.StartRacing(raceStartTime);
                racer.SetControllerActive(true);
            }
        }
        
        public void RacerFinished(Racer racer)
        {
            if (racer.RaceData.hasFinished)
                return;
                
            // Update Racer
            racer.RaceData.FinishRace(Time.time);
            
            // Event
            OnRacerFinished?.Invoke(racer);
        }

        #endregion

        #region Setup Helpers

        private void SetupRacers()
        {
            // Position
            float terrainHeight = terrainGenerator.TerrainGrid.Height * terrainGenerator.TerrainGrid.CellSize;
            
            Vector3 startPosition = new Vector3(
                5f,
                terrainHeight / 2f, 
                0
            );
                        
            // Spawn Racers
            foreach (RacerTournamentData racerTournamentData in tournamentManager.RacersData)
            {
                Racer racer = Instantiate(
                    racerTournamentData.RacerPrefab, 
                    startPosition + new Vector3(racerSpacing * spawnedRacers.Count, 0, 0), 
                    Quaternion.identity,
                    transform
                );
                
                // Add Racer
                racer.RaceData.Reset(racer.transform.position);
                spawnedRacers.Add(racer);
                
                racer.SetControllerActive(false);
            }
        }
        
        private void SetupFinishLine()
        {
            // Position
            float terrainWidth = terrainGenerator.TerrainGrid.Width * terrainGenerator.TerrainGrid.CellSize;
            float terrainHeight = terrainGenerator.TerrainGrid.Height * terrainGenerator.TerrainGrid.CellSize;
            
            Vector3 finishLinePosition = new Vector3(terrainWidth, terrainHeight / 2f, 0);
            
            // Spawn Finish Line
            finishLine = Instantiate(
                finishLinePrefab,
                finishLinePosition,
                Quaternion.identity,
                transform
            );
            
            // Scale
            finishLine.transform.localScale = new Vector3(finishLineWidth, terrainHeight, 1);
        }

        #endregion
        
        #region Helpers

        private void ChangeState(RaceState newState)
        {
            currentState = newState;
            OnRaceStateChanged?.Invoke(currentState);
            Debug.Log($"Race state changed to: {currentState}");
        }

        #endregion
    }
}