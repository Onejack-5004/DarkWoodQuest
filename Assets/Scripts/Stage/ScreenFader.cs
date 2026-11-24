
using UnityEngine;
using System;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Header("References")]
    public StageExitController stageExitController;
    public CanvasGroup fadeCanvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;
    private bool isFadingOut = false;

    public event Action OnFadeOutComplete;
    public event Action OnFadeInComplete;

    private void Awake()
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("Fade Canvas Group is missing!");
            enabled = false;
            return;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        if (stageExitController != null)
        {
            stageExitController.OnAllLivingPlayersExited +=
                StartFadeOut;
        }
    }

    private void OnDisable()
    {
        if (stageExitController != null)
        {
            stageExitController.OnAllLivingPlayersExited -=
                StartFadeOut;
        }
    }

    private void StartFadeOut()
    {
        if (isFadingOut)
            return;

        isFadingOut = true;

        Debug.Log("FADE OUT STARTED!");

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(1f));
    }

    public void FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        isFadingOut = false;

        fadeCoroutine = StartCoroutine(FadeRoutine(0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            fadeCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;

        if (targetAlpha >= 1f)
        {
            Debug.Log("FADE OUT COMPLETE!");
            OnFadeOutComplete?.Invoke();
        }
        else
        {
            Debug.Log("FADE IN COMPLETE!");
            OnFadeInComplete?.Invoke();
        }
    }
}
