using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpot : MonoBehaviour
{
    public bool inRange;
    public bool interacted;
    
    void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            SaveData.saveinstance.spotSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SaveData.saveinstance.lightPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SaveData.saveinstance.SavedLightSpot();
            SaveData.saveinstance.SavePlayerData();
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
