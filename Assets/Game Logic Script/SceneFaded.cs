using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFaded : MonoBehaviour
{
    [SerializeField] public float fadeTime;

    private Image fadeoutImage;
    public enum FadeDirection
    {
        In,
        Out
    }
    private void Awake()
    {
        fadeoutImage = GetComponent<Image>();
    }

    public void CallFadeandLoad(string _sceneLoad)
    {
        StartCoroutine(FadeandLoad(FadeDirection.In, _sceneLoad));
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
        float fadeEnd = _fadeDirection == FadeDirection.Out ? 0 : 1;

        if (_fadeDirection == FadeDirection.Out)
        {
            while (_alpha > fadeEnd)
            {
                SetColor(ref _alpha, _fadeDirection);
                yield return null;
            }
            fadeoutImage.enabled = false;
        }
        else
        {
            fadeoutImage.enabled = true;
            while (_alpha < fadeEnd)
            {
                SetColor(ref _alpha, _fadeDirection);
                yield return null;
            }
        }

    }

    public IEnumerator FadeandLoad( FadeDirection _fadeDirection, string _sceneLoad)
    {
        fadeoutImage.enabled = true;
        yield return Fade(_fadeDirection);
        SceneManager.LoadScene(_sceneLoad);
    }

    void SetColor(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeoutImage.color = new Color(fadeoutImage.color.r, fadeoutImage.color.g, fadeoutImage.color.b, _alpha);

        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
