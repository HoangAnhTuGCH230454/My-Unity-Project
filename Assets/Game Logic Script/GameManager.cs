using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public string Transitionfrom;

    public Vector2 PlatformrespawnPoint;
    public Vector2 respawnAfterDeath;
    public Vector2 defaultRespawnpoint;
    [SerializeField] LightSpot lightSpot;

    public GameObject Shade;
    [SerializeField] private UiScreen pauseMenu;
    [SerializeField] private float fadeTime;
    public bool isPaused;
    float lasttimeScale = -1f;
    public bool TBHDefeated = false;
    static Coroutine stopGameCorountine;

    public static bool isStopped { get { return stopGameCorountine != null; } }
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        SaveData.saveinstance.Instantiate();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!isPaused);
        }
    }
    public void Pause(bool pausing)
    {
        if (pauseMenu.isAnimate())
        {
            return;
        }
        if (pausing)
        {
            if (lasttimeScale < 0)
            {
                lasttimeScale = Time.timeScale;
            }
            Time.timeScale = 0;
            pauseMenu.Activate();
        }
        else
        {
            if (!isStopped)
            {
                Time.timeScale = lasttimeScale > 0f ? lasttimeScale : 1f;
                lasttimeScale = -1f;
            }
            pauseMenu.Deactivate();
        }
        isPaused = pausing;
    }

    public static void Stop(float duration = .5f, float restoreDelay = .1f, float slowMultiply = 0f)
    {
        if (stopGameCorountine != null)
        {
            return;
        }
        stopGameCorountine = Instance.StartCoroutine(HandleStopGame(duration, restoreDelay, slowMultiply));
    }

    static IEnumerator HandleStopGame(float duration, float restoreDelay, float slowMultiply = 0f)
    {
        if (Instance.lasttimeScale < 0)
        {
            Instance.lasttimeScale = Time.timeScale;
        }

        Time.timeScale = Mathf.Max(0, Instance.lasttimeScale * slowMultiply);
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (duration > 0)
        {
            if (Instance.isPaused)
            {
                yield return wait;
                continue;
            }
            Time.timeScale = Mathf.Max(0, Instance.lasttimeScale * slowMultiply);
            duration -= Time.unscaledDeltaTime;
            yield return wait;
        }

        float timeScaleToRestore = Instance.lasttimeScale;
        Instance.lasttimeScale = -1;
        stopGameCorountine = null;
        if (restoreDelay > 0)
        {
            float currentTimeScale = timeScaleToRestore * slowMultiply;
            float restoreSpeed = (timeScaleToRestore - currentTimeScale) / restoreDelay;
            while (currentTimeScale < timeScaleToRestore)
            {
                if (Instance.isPaused)
                {
                    yield return wait;
                    continue;
                }
                if (isStopped)
                {
                    yield break;
                }
                currentTimeScale += restoreSpeed * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Max(0, currentTimeScale);
                yield return wait;
            }
        }
        if (!isStopped)
        {
            Time.timeScale = Mathf.Max(0, timeScaleToRestore);
        }
    }
    public void SaveGame()
    {
        SaveData.saveinstance.SavePlayerData();
    }

    public void SaveScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SaveData.saveinstance.sceneNames.Add(currentSceneName);
    }

    public void respawnPlayer(float manaPenalty = 0f)
    {
        SaveData.saveinstance.LoadLightSpot();
        if (SaveData.saveinstance.spotSceneName != null)
        {
            SceneManager.LoadScene(SaveData.saveinstance.spotSceneName);
        }
        if (Mathf.Approximately(SaveData.saveinstance.lightPos.sqrMagnitude, 0))
        {
            respawnAfterDeath = SaveData.saveinstance.lightPos;
        }
        else
        {
            respawnAfterDeath = defaultRespawnpoint;
        }
        PlayerController.Instance.transform.position = respawnAfterDeath;
        UIManager.Instance.deathScreen.Deactivate();
        PlayerController.Instance.Respawn(manaPenalty);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.manaPenalty > 0f)
            {
                SaveData.saveinstance.LoadShadeData();
                if (SaveData.saveinstance.scenewithShade == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name || SaveData.saveinstance.scenewithShade == "")
                {
                    Instantiate(Shade, SaveData.saveinstance.shadePos, SaveData.saveinstance.shadeRotation);
                }
            }
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.deathScreen.Deactivate();
        }
        SaveScene();
        DontDestroyOnLoad(gameObject);
        lightSpot = FindObjectOfType<LightSpot>();
    }
}