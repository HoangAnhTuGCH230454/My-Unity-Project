using UnityEngine;

public class UIManager : UiScreen
{

    public static UIManager Instance;

    [Header("Ui Manager")]
    [SerializeField] public UiScreen deathScreen;
    [SerializeField] public GameObject inventory;

    [SerializeField] GameObject respawnMana, fullMana;

    public enum ManaState
    {
        Full,
        Respawn
    }

    public ManaState manaState;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void ManaSwitch(ManaState _manastate)
    {
        switch(_manastate)
        {
            case ManaState.Full:
                fullMana.SetActive(true);
                respawnMana.SetActive(false);
                break;

            case ManaState.Respawn:
                fullMana.SetActive(false);
                respawnMana.SetActive(true);
                break;
        }
        manaState = _manastate;
    }
}
