using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]

public class UiScreen : MonoBehaviour
{
    public enum Transition { None, Fade, colorFade, colorFadeandFade }
    public Transition sceneEntrance;
    public float fadeTime = 1f;
    public Color fadeColor = Color.black;
    [Range(0, 1)] public float crossFade = 0f;
    public float updateFrequency = 0.05f;

    [System.Flags] public enum TimeScaleMode { affectByGlobalTimeScale = 1, affectByPause = 2 }
    public TimeScaleMode timeScaleMode = (TimeScaleMode)1;

    protected CanvasGroup Group;
    protected float originalAlpha = 1f;
    protected Coroutine currentAnim;

    protected bool isAnimating = false;
    public bool isAnimate()
    {
        return isAnimating;
    }

    [Header("UI Feedback")]
    public UiAudioFeedback feedbackConfig;
    protected AudioSource audioSource;

    public float GetTimeScale()
    {
        if ((timeScaleMode & TimeScaleMode.affectByPause) > 0)
        {
            if (GameManager.Instance && GameManager.Instance.isPaused)
            {
                return 0f;
            }
        }
        if ((timeScaleMode & TimeScaleMode.affectByGlobalTimeScale) > 0)
        {
            return Time.timeScale;
        }
        return 1f;
    }

    public static float GetTimeScale(UiScreen target)
    {
        return target.GetTimeScale();
    }
    public const int COLOR_FADE_PRIORITY = 1000;

    protected virtual void Awake()
    {
        Group = GetComponent<CanvasGroup>();
        originalAlpha = Group.alpha;

        switch (sceneEntrance)
        {
            case Transition.Fade:
                StartCoroutine(Fade(fadeTime, 1));
                break;
            case Transition.colorFade:
                StartCoroutine(FadeTo(fadeColor, -1, fadeTime));
                break;
            case Transition.colorFadeandFade:
                StartCoroutine(ColorFadeandFade(fadeTime, 1, fadeColor));
                break;
        }
    }

    public void PlayAudioFeedback(string type)
    {
        if (!audioSource && !feedbackConfig)
        {
            return;
        }
        AudioClip sfx = feedbackConfig.GetSound(type);
        if (sfx)
        {
            audioSource.PlayOneShot(sfx);
        }
    }

    public void DeactivateAll(float fadeDuration = -1)
    {
        UiScreen[] all = FindObjectsOfType<UiScreen>();
        foreach (UiScreen screen in all)
        {
            if(screen == this)
            {
                continue;
            }
            screen.Deactivate(fadeDuration);
        }
    }

    public virtual void Activate(bool exclusive = false)
    {
        if (isAnimating)
        {
            return;
        }
        float ActivationDelay = 0;
        if (exclusive)
        {
            if (crossFade > 0)
            {
                DeactivateAll(crossFade);
                ActivationDelay = crossFade;
            }
            else
            {
                ActivationDelay = fadeTime / 2;
                DeactivateAll(ActivationDelay);
            }
        }
        gameObject.SetActive(true);
        Group.alpha = 0;

        StartCoroutine(Activate(ActivationDelay));
    }

    public virtual IEnumerator Activate(float delay)
    {
        if (isAnimating)
        {
            yield break;
        }
        isAnimating = true;

        if (delay > 0)
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(updateFrequency);
            while (delay > 0)
            {
                yield return wait;
                float Timescale = GetTimeScale();
                if (Timescale <= 0)
                {
                    continue;
                }
                delay -= wait.waitTime * Timescale;
            }
        }
        gameObject.SetActive(true);
        if (Group)
        {
            Group.alpha = 0;
            StartCoroutine(Fade(fadeTime, 1));
        }
        isAnimating = false;
    }

    public virtual void Deactivate(float fadeDuration = -1)
    {
        if (fadeDuration < 0)
        {
            fadeDuration = fadeTime;
        }
        if (Group)
        {
            Group.alpha = originalAlpha;
            StartCoroutine(Fade(fadeDuration, -1));
        }
    }

    protected virtual IEnumerator Fade(float duration, int direction = 1)
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(updateFrequency);
        while (isAnimating)
        {
            yield return wait;
        }

        isAnimating = true;
        float currentDuration = duration;
        Group.alpha = direction > 0 ? 0 : originalAlpha;
        gameObject.SetActive(true);
        while (currentDuration > 0)
        {
            yield return wait;
            float Timescale = GetTimeScale();
            if (Timescale <= 0)
            {
                continue;
            }
            currentDuration -= wait.waitTime * Timescale;
            float ratio = currentDuration / duration;
            Group.alpha = (direction > 0 ? 1f - ratio : ratio) * originalAlpha;
        }
        Group.alpha = direction > 0 ? originalAlpha : 0;

        if (direction < 0)
        {
            gameObject.SetActive(false);
        }
        isAnimating = false;
    }
    public static IEnumerator FadeTo(Color color, int direction = 1, float duration = 0.5f, TimeScaleMode timeScaleMode = 0)
    {
        GameObject go = new GameObject("Fader (Temp)");
        RectTransform rect = go.AddComponent<RectTransform>();
        Canvas canvas = go.AddComponent<Canvas>();
        Image image = go.AddComponent<Image>();
        CanvasGroup group = go.AddComponent<CanvasGroup>();
        UiScreen ui = go.AddComponent<UiScreen>();
        ui.timeScaleMode = timeScaleMode;

        image.color = color;
        group.alpha = direction > 0 ? 0 : 1;
        group.blocksRaycasts = group.interactable = false;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = COLOR_FADE_PRIORITY;

        yield return ui.Fade(duration, direction);
        Destroy(go, ui.updateFrequency);
    }

    protected virtual IEnumerator ColorFadeandFade(float fadeTime, int direction, Color color)
    {
        Group.alpha = 0;
        yield return FadeTo(color, -direction, fadeTime);
        yield return Fade(fadeTime, direction);
    }

    public virtual void LoadScene(string sceneName)
    {
        LoadScene(sceneName, -1);
    }
    public virtual void Loadscene(int buildIndex)
    {
        LoadScene(buildIndex, -1);
    }
    public virtual void LoadScene(string sceneName, float fadeDuration = -1, float newTimeScale = 1f)
    {
        int index = -1;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName)
            {
                index = i;
                break;
            }
        }
        if (index < 0)
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings.");
            return;
        }
        LoadScene(index, fadeDuration, newTimeScale);
    }
    public virtual void LoadScene(int buildIndex, float fadeDuration = -1, float newTimeScale = 1f)
    {
        StartCoroutine(LoadScene(buildIndex, LoadSceneMode.Single, fadeDuration, newTimeScale));
    }
    protected virtual IEnumerator LoadScene(int buildIndex, LoadSceneMode mode, float fadeDuration = -1, float newTimeScale = 1f)
    {
        if (fadeDuration < 0)
        {
            fadeDuration = fadeTime;
        }
        yield return FadeTo(fadeColor, 1,fadeDuration, 0);
        Time.timeScale = Mathf.Max(0, newTimeScale);
        SceneManager.LoadScene(buildIndex, mode);
    }

    public virtual void Quit()
    {
        Application.Quit();
    }
}