using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockingSideSpell : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] GameObject canvasUI;
    bool used;
    void Start()
    {
        if (PlayerController.Instance.unlockingSideSpell)
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
        yield return new WaitForSeconds(4f);
        PlayerController.Instance.unlockingSideSpell = true;
        SaveData.saveinstance.SavePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
