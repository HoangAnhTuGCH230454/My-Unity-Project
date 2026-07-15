using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] protected PlayerController player;
    [SerializeField] public float speed;
    [SerializeField] public float damage;
    [SerializeField] protected GameObject EnemyBlood;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    protected enum EnemyStates
    {
        Mushroom_Idle,
        Mushroom_Flip,

        Flyingorg_Idle,
        Flyingorg_Chase,
        Flyingorg_Stun,
        Flyingorg_Died,

        Golem_Idle,
        Golem_Suprise,
        Golem_Attack,

        Shade_Idle,
        Shade_Chase,
        Shade_Stun,
        Shade_Died,

        TBH_Stage1,
        TBH_Stage2,
        TBH_Stage3
    }
    protected EnemyStates currentState;

    protected virtual EnemyStates GetCurrentEnemyStates
    {
        get { return currentState; }
        set 
        {
            if (currentState != value) 
            {
                currentState = value;

                ChangeCurrentAnim();
            }
        }
    }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerController.Instance;
    }
    protected virtual void Update()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (isRecoiling)
        {
            if (recoilTimer <= recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            GameObject _enemyBlood = Instantiate(EnemyBlood, transform.position, Quaternion.identity);
            Destroy(_enemyBlood, 2.5f);
            rb.velocity = _hitDirection * -_hitForce * recoilFactor;
            isRecoiling = true;
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }
    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            if (PlayerController.Instance.pState.alive)
            {
                GameManager.Stop();
            }
        }
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected virtual void ChangeCurrentAnim() { }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyStates = _newState;
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}