using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SceneFaded sceneFaded;

    public static UIManager Instance;

    [SerializeField] GameObject deathScreen;
    [SerializeField] public GameObject inventory;

    [SerializeField] GameObject respawnMana, fullMana;

    public enum ManaState
    {
        Full,
        Respawn
    }

    public ManaState manaState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
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

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(sceneFaded.Fade(SceneFaded.FadeDirection.In));
        yield return new WaitForSeconds(1.4f);
        deathScreen.SetActive(true);
    }
    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFaded.Fade(SceneFaded.FadeDirection.Out));
    }
    private void Start()
    {
        sceneFaded = GetComponentInChildren<SceneFaded>();
    }
}
