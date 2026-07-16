using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField] private string Transitionto;

    public Color fadeColor = Color.black;
    public float fadeTime = 0.5f;

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
        StartCoroutine(UiScreen.FadeTo(fadeColor, -1, fadeTime));
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Terresquall.LightSpot.QuickSave();
            GameManager.Instance.Transitionfrom = SceneManager.GetActiveScene().name;

            PlayerController.Instance.Set(PlayerController.State.cutscene, true);
            PlayerController.Instance.Set(PlayerController.State.invincible, true);
            
            UIManager.Instance.LoadScene(Transitionto, fadeTime);
        }
        
    }
}
