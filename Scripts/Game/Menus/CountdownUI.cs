using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownUI : MonoBehaviour
{
    [Header("UI References")] public TextMeshProUGUI countdownText;

    public CanvasGroup canvasGroup;

    [Header("Animation Settings")] public float fadeInDuration = 0.3f;

    public float fadeOutDuration = 0.15f;
    public float countdownInterval = 0.25f;
    public float goDisplayDuration = 1f;

    [Header("Animation Options")] public bool pulseOnCount = true;

    public float pulseScale = 1.2f;
    public float pulseDuration = 0.5f;

    [Header("Audio Reference")] [SerializeField]
    private AudioManager audioManager;

    // Event that will be triggered when countdown completes
    public UnityEvent onCountdownComplete = new();

    // Flag to prevent multiple simultaneous countdowns
    private bool isPlaying;

    public void PlayCountdown(Action onComplete = null)
    {
        if (isPlaying) return;

        // Add callback to the event if provided
        if (onComplete != null)
        {
            UnityAction tempAction = null;
            tempAction = () =>
            {
                onComplete.Invoke();
                onCountdownComplete.RemoveListener(tempAction);
            };
            onCountdownComplete.AddListener(tempAction);
        }

        isPlaying = true;
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        // Pause background music during countdown and increase SFX speed
        if (audioManager != null)
        {
            audioManager.PauseBackgroundMusic();
            audioManager.SetSFXPitch(1.6f);
        }

        countdownText.text = "DIGGITY!";
        canvasGroup.gameObject.SetActive(true);

        yield return FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration);

        // Display "DIGGITY!" three times
        for (var i = 0; i < 3; i++)
        {
            countdownText.text = "DIGGITY!";
            if (audioManager != null)
                audioManager.PlayDiggitySound();

            if (pulseOnCount)
                yield return PulseText();
            yield return new WaitForSeconds(countdownInterval);
        }

        countdownText.text = "LET'S GO RACING!";
        if (audioManager != null)
            audioManager.PlayGoSound();

        if (pulseOnCount)
            yield return PulseText();

        yield return new WaitForSeconds(goDisplayDuration);
        yield return FadeCanvasGroup(canvasGroup, 1f, 0f, fadeOutDuration);

        // Resume background music and reset SFX pitch after countdown
        if (audioManager != null)
        {
            audioManager.ResumeBackgroundMusic();
            audioManager.ResetSFXPitch();
        }

        isPlaying = false;
        onCountdownComplete.Invoke();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        var startTime = Time.time;
        var endTime = startTime + duration;
        group.alpha = startAlpha;
        while (Time.time < endTime)
        {
            var t = (Time.time - startTime) / duration;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        group.alpha = endAlpha;
    }

    private IEnumerator PulseText()
    {
        var originalScale = countdownText.transform.localScale;
        var targetScale = originalScale * pulseScale;
        var halfDuration = pulseDuration / 2f;

        yield return ScaleText(originalScale, targetScale, halfDuration);
        yield return ScaleText(targetScale, originalScale, halfDuration);
    }

    private IEnumerator ScaleText(Vector3 startScale, Vector3 endScale, float duration)
    {
        var startTime = Time.time;
        var endTime = startTime + duration;
        while (Time.time < endTime)
        {
            var t = (Time.time - startTime) / duration;
            countdownText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        countdownText.transform.localScale = endScale;
    }
}