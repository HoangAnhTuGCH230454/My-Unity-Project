using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class increasingHeartShard : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] GameObject canvasUI;
    [SerializeField] HeartShards heartShards;
    bool used;
    void Start()
    {
        if (PlayerController.Instance.maxHealth >= PlayerController.Instance.maxTotalHealth)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player") && !used)
        {
            used = true;

            StartCoroutine(showCanvas());
        }
    }
    IEnumerator showCanvas()
    {
        GameObject _particle = Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(_particle, 1f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        canvasUI.SetActive(true);
        heartShards.InitialFillAmount = PlayerController.Instance.heartShards * 0.25f;
        PlayerController.Instance.heartShards++;
        heartShards.fillAmount = PlayerController.Instance.heartShards * 0.25f;

        StartCoroutine(heartShards.LerpFill());

        yield return new WaitForSeconds(2.5f);
        SaveData.saveinstance.SavePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
