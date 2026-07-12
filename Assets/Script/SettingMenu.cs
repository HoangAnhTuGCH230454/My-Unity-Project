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
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
