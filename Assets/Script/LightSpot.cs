using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Terresquall;

public class SavePoint : PersistentObject
{
    public bool inRange;
    public bool interacted;
    public Vector2 anchor;

    public Vector3 getAnchorPos()
    {
        return transform.position + (Vector3)anchor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon((Vector2)transform.position + anchor, "sv_icon_dot8_sml");
    }

    void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            GameManager.globalData.lightSpotSaveID = saveID;

            Terresquall.LightSpot.SaveGameAsync();
        }
    }


    public override SaveData Save()
    {
        return null;
    }
    public override bool Load(SaveData data)
    {
        return true;
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
