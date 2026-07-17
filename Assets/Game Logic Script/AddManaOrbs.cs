using System.Collections;
using UnityEngine;

public class AddManaOrbs : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject CanvasUI;

    [SerializeField] OrbShards orbShards;

    bool used;

    void Start()
    {
        if (PlayerController.Instance.excessMana >= PlayerController.Instance.ExcessMaxMana)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);

        CanvasUI.SetActive(true);
        orbShards.initialFillAmount = PlayerController.Instance.manaShards / PlayerController.Instance.manaShardsPerExcessUnit;
        PlayerController.Instance.manaShards++;
        orbShards.targetFillAmount = PlayerController.Instance.manaShards / PlayerController.Instance.manaShardsPerExcessUnit;

        StartCoroutine(orbShards.LerpFill());
        yield return new WaitForSeconds(2.5f);
        PlayerController.Instance.ConvertManaShards();

        CanvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
