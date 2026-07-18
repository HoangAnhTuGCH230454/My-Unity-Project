using UnityEngine;
using UnityEngine.Audio;

public class SettingMenu : UiScreen
{
    [Header("Menu Settings")]
    [SerializeField] AudioMixer audioMixer;
    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
    }
    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }
    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}