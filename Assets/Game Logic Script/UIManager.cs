using UnityEngine;

public class UIManager : UiScreen
{

    public static UIManager Instance;

    [Header("Ui Manager")]
    public UiHealth healthUI;
    public UiMana manaUI;
    public UiScreen deathScreen;
    public GameObject inventory;

    public static void UpdateHealthUI(int health, int maxHealth, int excessHealth = 0)
    {
        if (Instance && Instance.healthUI)
        {
            Instance.healthUI.Refresh(health, maxHealth, excessHealth);
        }
    }
    public static void UpdateManaUI(float mana, float maxMana, float excessMana, float excessMaxMana, float manaPenalty = 1f)
    {
        if (Instance && Instance.manaUI)
        {
            Instance.manaUI.Refresh(mana, maxMana, excessMana, excessMaxMana);
        }
        Instance.manaUI.SetMode(manaPenalty);
    }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
}
