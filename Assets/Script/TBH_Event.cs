using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBH_Event : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        if (PlayerController.Instance.transform.position.x > transform.position.x || PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Hit(TheBlindHuntress.instance.SideAttackTransform, TheBlindHuntress.instance.SideAttackArea);
        }
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Hit(TheBlindHuntress.instance.UpAttackTransform, TheBlindHuntress.instance.UpAttackArea);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Hit(TheBlindHuntress.instance.DownAttackTransform, TheBlindHuntress.instance.DownAttackArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<PlayerController>() != null && !PlayerController.Instance.Is(PlayerController.State.invincible))
            {
                _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(TheBlindHuntress.instance.damage);
                if (PlayerController.Instance.Is(PlayerController.State.alive))
                {
                    GameManager.Stop();
                }
            }
        }
    }

    void DestroyAfterDeath()
    {
        SpawnBoss.instance.isNotTrigger();
        TheBlindHuntress.instance.DestroyAfterDeath();
        GameManager.Set(GameManager.Flags.TBHDefeated, true);
        Terresquall.LightSpot.SaveGameAsync();
    }
}
