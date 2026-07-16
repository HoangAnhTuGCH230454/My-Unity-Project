using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public Color hitFadeColor = Color.black;
    public float hitFadeTime = 0.0f;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerController.Instance.Set(PlayerController.State.cutscene, true);
        PlayerController.Instance.Set(PlayerController.State.invincible, true);
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UiScreen.FadeTo(hitFadeColor, 1, hitFadeTime));
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(hitFadeTime);
        PlayerController.Instance.transform.position = GameManager.Instance.PlatformrespawnPoint;
        StartCoroutine(UiScreen.FadeTo(hitFadeColor, -1, hitFadeTime));
        yield return new WaitForSecondsRealtime(hitFadeTime);
        PlayerController.Instance.Set(PlayerController.State.cutscene, false);
        PlayerController.Instance.Set(PlayerController.State.invincible, false);
        Time.timeScale = 1;
    }
}
