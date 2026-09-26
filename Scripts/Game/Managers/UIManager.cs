using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Utils;
using Game.Racing.Tournament;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    public CanvasGroup tournamentPanel;
    public CanvasGroup resultsPanel;
    public CanvasGroup countdownPanel;
    public CanvasGroup gameplayPanel;

    [Header("Tournament Panel")]
    public TextMeshProUGUI tournamentRaceNumTitle;
    public Transform tournamentRacerRowsContainer;
    public GameObject tournamentRacerRowPrefab;
    public Button mainMenuButton;

    [Header("Results Panel")]
    public TextMeshProUGUI resultsTitle;
    public Transform resultsRacerRowsContainer;
    public GameObject resultRacerRowPrefab;

    [Header("Gameplay Panel")]
    public Transform leaderboardContainer;
    public GameObject leaderboardRowPrefab;

    [Header("Animation Settings")]
    public float panelFadeInDuration = 0.3f;
    public float panelFadeOutDuration = 0.2f;
    public float panelScaleInDuration = 0.3f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float rowAnimationDelay = 0.1f;

    private readonly List<RacerRowUI> tournamentRows = new();
    private readonly List<RacerRowUI> resultsRows = new();
    private readonly List<RacerRowUI> leaderboardRows = new();

    private const string sceneMainMenu = "MainMenu";

    public void ShowPanel(CanvasGroup panel)
    {
        panel.gameObject.SetActive(true);
        StartCoroutine(UIUtils.FadePanel(panel, true, panelFadeInDuration, scaleCurve));
    }
    
    public void HidePanel(CanvasGroup panel)
    {
        StartCoroutine(UIUtils.FadePanel(panel, false, panelFadeOutDuration, scaleCurve));
    }

    public void UpdateTournamentStandings(List<RacerTournamentData> racers, int currentRace, int totalRaces)
    {
        tournamentRaceNumTitle.text = $"Standings - Race {currentRace} of {totalRaces}";
        UIUtils.ClearUIElements(tournamentRows);
        
        StartCoroutine(CreateAndAnimateRows(
            racers, 
            tournamentRacerRowsContainer, 
            tournamentRacerRowPrefab, 
            tournamentRows, 
            racer => racer.RaceResults.Sum(),
            true
        ));
    }

    public void UpdateLeaderboard(List<RacerTournamentData> racers)
    {
        UIUtils.ClearUIElements(leaderboardRows);
        PopulateRows(
            racers,
            leaderboardContainer,
            leaderboardRowPrefab,
            leaderboardRows,
            racer => 0
        );
    }
    
    public void UpdateRaceResults(List<RacerTournamentData> racers, int currentRace)
    {
        resultsTitle.text = $"Race {currentRace} Results";
        UIUtils.ClearUIElements(resultsRows);
        PopulateRows(
            racers,
            resultsRacerRowsContainer,
            resultRacerRowPrefab,
            resultsRows,
            racer => racer.RaceResults.Last()
        );
    }

    public void ShowFinalResults(List<RacerTournamentData> racers)
    {
        tournamentRaceNumTitle.text = "Final Tournament Results";
        UIUtils.ClearUIElements(tournamentRows);
        
        PopulateRows(
            racers,
            tournamentRacerRowsContainer,
            tournamentRacerRowPrefab,
            tournamentRows,
            racer => racer.RaceResults.Sum()
        );

        mainMenuButton.gameObject.SetActive(true);
        ShowPanel(tournamentPanel);
    }

    private void PopulateRows(
        List<RacerTournamentData> racers, 
        Transform container, 
        GameObject prefab, 
        List<RacerRowUI> rowsList,
        System.Func<RacerTournamentData, int> scoreSelector)
    {
        for (var i = 0; i < racers.Count; i++)
        {
            var row = UIUtils.CreateUIRow<RacerRowUI>(container, prefab);
            rowsList.Add(row);
            row.SetData(i + 1, racers[i].RacerPrefab.ProfileData.racerName, scoreSelector(racers[i]));
        }
    }

    private IEnumerator CreateAndAnimateRows(
        List<RacerTournamentData> racers, 
        Transform container, 
        GameObject prefab, 
        List<RacerRowUI> rowsList,
        System.Func<RacerTournamentData, int> scoreSelector,
        bool animate = false)
    {
        for (var i = 0; i < racers.Count; i++)
        {
            var row = UIUtils.CreateUIRow<RacerRowUI>(container, prefab);
            rowsList.Add(row);
            row.SetData(i + 1, racers[i].RacerPrefab.ProfileData.racerName, scoreSelector(racers[i]));
            
            if (animate)
            {
                CanvasGroup rowCanvas = row.GetComponent<CanvasGroup>();
                if (!rowCanvas) rowCanvas = row.gameObject.AddComponent<CanvasGroup>();
                rowCanvas.alpha = 0f;
                
                StartCoroutine(UIUtils.AnimateUIElementIn(rowCanvas, i * rowAnimationDelay));
            }
            
            yield return null;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(sceneMainMenu);
    }
}
