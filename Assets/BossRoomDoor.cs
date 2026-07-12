// BossRoomDoor.cs — put on Boss Room Door (1) and (2)
using UnityEngine;

public class BossRoomDoor : MonoBehaviour
{
    private Collider2D doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SpawnBoss.OnBossFightStarted += Lock;
        TheBlindHuntress.OnBossDefeated += Unlock;
    }

    private void OnDisable()
    {
        SpawnBoss.OnBossFightStarted -= Lock;
        TheBlindHuntress.OnBossDefeated -= Unlock;
    }

    private void Lock()
    {
        doorCollider.isTrigger = false; // solid, blocks the player in
    }

    private void Unlock()
    {
        gameObject.SetActive(false); // "disappear" once boss is defeated
    }
}
