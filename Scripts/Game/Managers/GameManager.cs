using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;

using Game.Racing;
using Game.Racing.Tournament;
using Game.Core.Classes;
using Game.Core.Constants;
using Game.Managers;
using SceneManager = Game.Managers.SceneManager;

public class GameManager : SingletonBehaviour<GameManager>
{
    [Header("Race Settings")]
    [SerializeField] private Racer[] racerPrefabs;

    [SerializeField] private Texture2D[] mapTextures;
    [SerializeField] private int totalRaces = 4;
    
    [Header("UI Settings")]
    [SerializeField] private float panelDisplayDuration = 2.0f;
    
    // References
    private UIManager uiManager;
    private CountdownUI countdownUI;
    
    private AudioManager audioManager;
    
    private RaceManager raceManager;
    
    // Internal References
    private TournamentManager tournamentManager;
    
    // State
    private bool isRaceEnding = false;
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Get References
        FindReferences();
        
        // Start Race
        isRaceEnding = false;
        if (raceManager != null)
        {
            raceManager.OnRacerFinished.AddListener(HandleRacerFinished);
            StartCoroutine(StartRaceSequence());
        }
    }

    private void FindReferences()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        countdownUI = FindFirstObjectByType<CountdownUI>();
        audioManager = FindFirstObjectByType<AudioManager>();
        raceManager = FindFirstObjectByType<RaceManager>();
    }

    public void StartTournament(bool soloMode = false)
    {
        var chosenRacers = racerPrefabs;
        if (soloMode)
            chosenRacers = new Racer[] { racerPrefabs[0] };

        tournamentManager = new TournamentManager(chosenRacers, totalRaces);
        tournamentManager.StartTournament();
        
        SceneManager.Instance.LoadScene(SceneConstants.SceneGame);
    }

    private IEnumerator StartRaceSequence()
    {
        Debug.Log("Loading Map: " + tournamentManager.CurrentRace);
        Texture2D currentMap = mapTextures[tournamentManager.CurrentRace];
        raceManager.InitializeRace(tournamentManager, currentMap);
        yield return StartCoroutine(ShowTournamentStandings());

        raceManager.PrepareRaceStart();
        yield return StartCoroutine(ShowCountdown());

        raceManager.StartRace();
    }

    private IEnumerator EndRaceSequence()
    {
        yield return new WaitForSeconds(3f);
        
        // Results
        tournamentManager.RecordRaceResults(raceManager.SpawnedRacers);
        yield return StartCoroutine(ShowRaceResults());
        
        Debug.Log($"Race {tournamentManager.CurrentRace} finished and Total: {totalRaces}");
        bool isLastRace = totalRaces - 1 <= tournamentManager.CurrentRace;
        if (isLastRace)
        {
            ShowFinalResults();
            yield return new WaitForSeconds(3f);
        }
        else
        {
            tournamentManager.AdvanceToNextRace();
            SceneManager.Instance.LoadScene(SceneConstants.SceneGame);
        }
    }
    
    private void HandleRacerFinished(Racer racer)
    {
        if (isRaceEnding)
            return;
        
        bool isPlayer = racer.ProfileData.racerName == "Player";
        audioManager.PlayFinishSound(isPlayer);
        StartCoroutine(EndRaceSequence());
        isRaceEnding = true;
    }
    
    private IEnumerator ShowTournamentStandings()
    {
        var sortedRacers = tournamentManager.RacersData.OrderByDescending(r => r.RaceResults.Sum());
        
        // Display Tournament Standings
        uiManager.UpdateTournamentStandings(sortedRacers.ToList(), tournamentManager.CurrentRace + 1, totalRaces);
        uiManager.ShowPanel(uiManager.tournamentPanel);
        yield return new WaitForSeconds(panelDisplayDuration);
        
        // Hide Tournament Standings
        var hideRoutine = StartCoroutine(HidePanelWithDelay(uiManager.tournamentPanel, uiManager.panelFadeOutDuration));
        yield return hideRoutine;
    }
    
    private IEnumerator ShowCountdown()
    {
        var timeoutDuration = 10f;
        var isCompleted = false;

        uiManager.ShowPanel(uiManager.countdownPanel);

        // Create a one-time event listener
        UnityAction completeAction = null;
        completeAction = () =>
        {
            Debug.Log("GameManager: Received countdown completion event");
            isCompleted = true;
            countdownUI.onCountdownComplete.RemoveListener(completeAction);
        };
        countdownUI.onCountdownComplete.AddListener(completeAction);
        countdownUI.PlayCountdown();

        // Wait for completion or timeout
        var startTime = Time.time;
        while (!isCompleted && Time.time < startTime + timeoutDuration) yield return null;

        // Remove the listener if we timed out
        if (!isCompleted)
        {
            Debug.LogWarning($"ShowCountdown: Timed out after {timeoutDuration} seconds!");
            countdownUI.onCountdownComplete.RemoveListener(completeAction);
        }

        uiManager.HidePanel(uiManager.countdownPanel);
    }

    private IEnumerator ShowRaceResults()
    {
        // Update Results
        var sortedRacers = tournamentManager.RacersData.OrderByDescending(r => r.RaceResults.Last());
        uiManager.UpdateRaceResults(sortedRacers.ToList(), tournamentManager.CurrentRace + 1);
        
        // Display Results
        uiManager.ShowPanel(uiManager.resultsPanel);
        yield return new WaitForSeconds(panelDisplayDuration * 1.5f);
        uiManager.HidePanel(uiManager.resultsPanel);
    }

    private void ShowFinalResults()
    {
        var sortedRacers = tournamentManager.RacersData.OrderByDescending(r => r.RaceResults.Sum());
        uiManager.ShowFinalResults(sortedRacers.ToList());
    }
    
    private IEnumerator HidePanelWithDelay(CanvasGroup panel, float delay)
    {
        uiManager.HidePanel(panel);
        yield return new WaitForSeconds(delay);
    }
}
