using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string Transitionfrom;

    public Vector2 PlatformrespawnPoint;
    public Vector2 respawnAfterDeath;
    [SerializeField] LightSpot lightSpot;

    public GameObject Shade;
    [SerializeField] private FadeUI pauseMenu;
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
        if (pausing)
        {
            if (lasttimeScale < 0)
            {
                lasttimeScale = Time.timeScale;
            }
            Time.timeScale = 0;
        }
        else
        {
            if (!isStopped)
            {
                Time.timeScale = lasttimeScale;
                lasttimeScale = -1f;
            }
        }
        pauseMenu.Fade(fadeTime, pausing);
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

        Time.timeScale = Instance.lasttimeScale * slowMultiply;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (duration > 0)
        {
            if (Instance.isPaused)
            {
                yield return wait;
                continue;
            }
            Time.timeScale = Instance.lasttimeScale * slowMultiply;
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
                Time.timeScale = currentTimeScale;
                yield return wait;
            }
        }
        if (!isStopped)
        {
            Time.timeScale = timeScaleToRestore;
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

    public void respawnPlayer()
    {
        SaveData.saveinstance.LoadLightSpot();
        if (SaveData.saveinstance.spotSceneName != null)
        {
            SceneManager.LoadScene(SaveData.saveinstance.spotSceneName);
        }
        if (SaveData.saveinstance.lightPos != null)
        {
            respawnAfterDeath = SaveData.saveinstance.lightPos;
        }
        else
        {
            respawnAfterDeath = PlatformrespawnPoint;
        }
        PlayerController.Instance.transform.position = respawnAfterDeath;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawn();
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
            if (PlayerController.Instance.respawnMana)
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
            StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        }
        SaveScene();
        DontDestroyOnLoad(gameObject);
        lightSpot = FindObjectOfType<LightSpot>();
    }
}