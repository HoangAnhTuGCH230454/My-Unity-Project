using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField] private string Transitionto;
    [SerializeField] private Transform Startpoint;
    [SerializeField] private Vector2 Exit;
    [SerializeField] private float Endtime;
    void Start()
    {
        if (Transitionto == GameManager.Instance.Transitionfrom)
        {
            PlayerController.Instance.transform.position = Startpoint.position;

            StartCoroutine(PlayerController.Instance.WalktoScene(Exit, Endtime));
        }
        StartCoroutine(UIManager.Instance.sceneFaded.Fade(SceneFaded.FadeDirection.Out));
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            CheckShade();
            GameManager.Instance.Transitionfrom = SceneManager.GetActiveScene().name;
            PlayerController.Instance.pState.cutscene = true;
            SceneManager.LoadScene(Transitionto);
        }
        StartCoroutine(UIManager.Instance.sceneFaded.FadeandLoad(SceneFaded.FadeDirection.In, Transitionto));
    }
    void CheckShade()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.saveinstance.SaveShadeData();
            }
        }
    }
}
