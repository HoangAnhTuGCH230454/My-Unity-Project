using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    [SerializeField] Vector2 exitDir;
    bool callOnce;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        if (TheBlindHuntress.instance != null)
        {
            Destroy(TheBlindHuntress.instance);
            callOnce = false;
            boxCollider.isTrigger = true;
        }
        if(GameManager.Is(GameManager.Flags.TBHDefeated))
        {
            callOnce = true;
        }
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!callOnce)
            {
                StartCoroutine(WalkintoRoom());
                callOnce = true;
            }
        }
    }

    IEnumerator WalkintoRoom()
    {
        StartCoroutine(PlayerController.Instance.WalktoScene(exitDir, 1));
        PlayerController.Instance.Set(PlayerController.State.cutscene, true);
        yield return new WaitForSeconds(1f);
        boxCollider.isTrigger = false;
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
        OnBossFightStarted?.Invoke(); // NEW
    }

    public void isNotTrigger()
    {
        boxCollider.isTrigger = true;
    }

    public static event System.Action OnBossFightStarted;
}
