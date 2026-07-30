using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terresquall;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

[DisallowMultipleComponent]
public class GameManager : PersistentObject
{
    public string Transitionfrom;

    public Vector2 PlatformrespawnPoint;
    public Vector2 respawnAfterDeath;
    public Vector2 defaultRespawnpoint;

    public Shade shadePrefab;
    Shade currentShade;

    [SerializeField] private UiScreen pauseMenu;
    [SerializeField] private GameObject uiCanvasPrefab;
    [SerializeField] private float fadeTime;
    public bool isPaused;
    float lasttimeScale = -1f;
    public static SaveData globalData = new SaveData();
    static Coroutine stopGameCorountine;

    public static bool isStopped { get { return stopGameCorountine != null; } }
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (PlayerController.Instance != null)
        {
            if (globalData.shadeScene == SceneManager.GetActiveScene().name)
            {
                SpawnShade(globalData.shadePosition, globalData.shadeRotation);
            }
        }
        if (Instance != null && Instance != this)
        {
            // A GameManager already exists (persisted via DontDestroyOnLoad from a
            // previous scene). Leave this one alone instead of destroying it - just
            // skip re-initializing so we don't overwrite the existing Instance.
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        if (UIManager.Instance == null && uiCanvasPrefab != null)
        {
            Instantiate(uiCanvasPrefab);
        }

        if (pauseMenu == null && UIManager.Instance != null)
        {
            pauseMenu = UIManager.Instance.pauseMenu;
        }

        SaveScene();
    }

    async void Start()
    {
        await Task.Delay(100);
        LightSpot.QuickLoad();

        if (PlayerController.Instance != null)
        {
            if (globalData.shadeScene == SceneManager.GetActiveScene().name)
            {
                SpawnShade(globalData.shadePosition, globalData.shadeRotation);
            }
        }
    }

    public static Shade SpawnShade(Vector3 position, Quaternion rotation)
    {
        if (Instance.currentShade)
        {
            return null;
        }
        Instance.currentShade = Instantiate(Instance.shadePrefab, position, rotation);
        return Instance.currentShade;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!isPaused);
        }
    }

    protected override void Reset()
    {
        saveID = "GameManager";
        defaultRespawnpoint = FindObjectOfType<PlayerController>().transform.position;
        pauseMenu = FindObjectOfType<UiScreen>();
    }
    public void Pause(bool pausing)
    {
        if (pauseMenu == null)
        {
            Debug.LogWarning("GameManager.Pause() called but no pause menu is assigned.");
            return;
        }
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

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void respawnPlayer(float manaPenalty = 0f)
    {
        if (!string.IsNullOrEmpty(globalData.lastScene) && globalData.lastScene != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(globalData.lastScene);
            SavePoint sp = LightSpot.FindObjectBySaveID(globalData.lastScene) as SavePoint;
            if (sp)
            {
                respawnAfterDeath = sp.getAnchorPos();
            }
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
                if (globalData.shadeScene == SceneManager.GetActiveScene().name || globalData.shadeScene == "")
                {
                    Instantiate(shadePrefab, globalData.shadePosition, globalData.shadeRotation);
                }
            }
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.deathScreen.Deactivate();
        }
        SaveScene();
        DontDestroyOnLoad(gameObject);
    }

    public override PersistentObject.SaveData Save()
    {
        if (CanSave())
        {
            globalData.saveID = saveID;
            return globalData;
        }

        Debug.Log("Failed to save the game");
        return null;
    }

    public override bool Load(PersistentObject.SaveData data)
    {
        if (data == null)
        {
            return false;
        }
        globalData = data as SaveData;
        if (globalData == null)
        {
            return false;
        }
        return true;
    }

    [System.Flags]
    public enum Flags : long { None = 0, TBHDefeated = 1 }
    public static bool Is(Flags f)
    {
        return globalData.flags.HasFlag(f);
    }
    public static void Set(Flags f, bool on)
    {
        if (on)
        {
            globalData.flags |= f;
        }
        else
        {
            globalData.flags &= ~f;
        }
    }

    [System.Serializable]
    public new class SaveData : PersistentObject.SaveData
    {
        public string lastScene;
        public string lightSpotSaveID;

        public string shadeScene;
        public float shadePositionX, shadePositionY, shadePositionZ;
        public float shadeRotationX, shadeRotationY, shadeRotationZ, shadeRotationW;
        public Flags flags;

        public Vector3 shadePosition
        {
            get { return new Vector3(shadePositionX, shadePositionY, shadePositionZ); }
            set
            {
                shadePositionX = value.x;
                shadePositionY = value.y;
                shadePositionZ = value.z;
            }
        }

        public Quaternion shadeRotation
        {
            get { return new Quaternion(shadeRotationX, shadeRotationY, shadeRotationZ, shadeRotationW); }
            set
            {
                shadeRotationX = value.x;
                shadeRotationY = value.y;
                shadeRotationZ = value.z;
                shadeRotationW = value.w;
            }
        }
    }
}