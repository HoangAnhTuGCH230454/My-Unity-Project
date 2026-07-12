using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }
    public void CallSceneStartGame(string _scenetoLoad)
    {
        StartCoroutine(FadeandStartGame(_scenetoLoad));
    }
    IEnumerator FadeandStartGame(string _scenetoLoad)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(_scenetoLoad);
    }
    void Update()
    {
        
    }
}
