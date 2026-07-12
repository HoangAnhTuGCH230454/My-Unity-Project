using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlindHuntress : Enemy
{
    public static TheBlindHuntress instance;
    public Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    public Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] GameObject[] slashEffects;

    public float attackRange;
    public float attackTimer;

    [HideInInspector] public bool facingRight;

    [Header("Ground Check Settings")]
    [SerializeField] public Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatisGround;

    int hitCounter;
    bool stunneed, canStun;
    bool isAlive;

    [HideInInspector] public float runSpeed;

    public GameObject impactParticle;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatisGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatisGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatisGround))
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    protected override void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.TBH_Stage1);
        isAlive = true;
    }

    protected override void UpdateEnemyStates()
    {
        if (PlayerController.Instance !=  null)
        {
            switch(GetCurrentEnemyStates)
            {
                case EnemyStates.TBH_Stage1:
                    canStun = true;
                    attackTimer = 3f;
                    runSpeed = speed;
                    break;
                case EnemyStates.TBH_Stage2:
                    canStun = true;
                    attackTimer = 2f;
                    break;
                case EnemyStates.TBH_Stage3:
                    canStun = false;
                    attackTimer = 1f;
                    runSpeed = speed * 1.5f;
                    break;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (health <= 0 && isAlive)
        {
            Death(0);
        }

        if (!isAttacking)
        {
            countdownAttack -= Time.deltaTime;
        }
        if (stunneed)
        {
            rb.velocity = Vector2.zero;
        }
    }
    public void Flip()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
    }
    protected override void OnCollisionStay2D(Collision2D _other)
    {
        
    }
    public void ResetAllAttack()
    {
        isAttacking = false;
        StopCoroutine(TripletSLash());
        StopCoroutine(LungeAttack());

        DiveAttack = false;
    }
    #region attacking
    #region variables
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public float countdownAttack;
    [HideInInspector] public bool damagePlayer = false;

    [HideInInspector] public Vector2 JumptoPosition;
    [HideInInspector] public bool DiveAttack;
    public GameObject diveCollider;

    #endregion
    #region Controller

    public void AttackHandle()
    {
        if (currentState == EnemyStates.TBH_Stage1)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange) 
            {
                StartCoroutine(TripletSLash());
            }
        }
        if (currentState == EnemyStates.TBH_Stage2)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripletSLash());
            }
            else
            {
                StartCoroutine(LungeAttack());
            }
        }
        if (currentState == EnemyStates.TBH_Stage3)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripletSLash());
            }
            else
            {
                int _attackChosen = Random.Range(1, 2);
                if (_attackChosen == 1)
                {
                    StartCoroutine(LungeAttack());
                }
                else if (_attackChosen == 2)
                {
                    DiveAttackJump();
                }
            }
        }
    }
    #endregion
    #region Stage 1
    IEnumerator TripletSLash()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        SlashAngle();
        yield return new WaitForSeconds(0.2f);
        anim.ResetTrigger("Slash");

        ResetAllAttack();
    }
    void SlashAngle()
    {
        if (PlayerController.Instance.transform.position.x > transform.position.x || PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Instantiate(slashEffects[1], SideAttackTransform);
        }
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            SlashEffectAtAngle(slashEffects[0], 90, UpAttackTransform);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            SlashEffectAtAngle(slashEffects[1], -90, UpAttackTransform);
        }
    }
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        GameObject spawnedSlash = Instantiate(_slashEffect, _attackTransform);
        spawnedSlash.transform.localEulerAngles = new Vector3(0, 0, _effectAngle);
    }
    IEnumerator LungeAttack()
    {
        Flip();
        isAttacking = true;

        anim.SetBool("Lunge", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Lunge", false);
        damagePlayer = false;
        ResetAllAttack();
    }
    #endregion
    #region Stage 2
    public void DiveAttackJump()
    {
        isAttacking = true;
        JumptoPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
        DiveAttack = true;
        anim.SetBool("Dive", true);
    }

    public void Dive()
    {
        anim.SetBool("Dive", true);
        anim.SetBool("Jump", false);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<PlayerController>() != null && DiveAttack)
        {
            _other.GetComponent<PlayerController>().TakeDamage(damage * 2);
            PlayerController.Instance.pState.recoilingX = true;
        }
    }
    #endregion
    #endregion
    public void EnemyGethits()
    {
        if (!stunneed)
        {
            if (canStun)
            {
                hitCounter++;
                if (hitCounter >= 5)
                {
                    ResetAllAttack();
                    StartCoroutine(Stunned());
                }
            }
        }
        else
        {
            StopCoroutine(Stunned());
            anim.SetBool("Stun", false);
            stunneed = false;
        }
        #region State
        if (health > 75)
        {
            ChangeState(EnemyStates.TBH_Stage1);
        }
        else if (health <= 75 && health > 50)
        {
            ChangeState(EnemyStates.TBH_Stage2);
        }
        else if (health <= 50)
        {
            ChangeState(EnemyStates.TBH_Stage3);
        }
        else if (health <= 0)
        {
            Death(0);
        }
        #endregion
    }
    
    public IEnumerator Stunned()
    {
        stunneed = true;
        hitCounter = 0;
        anim.SetBool("Stun", true);
        yield return new WaitForSeconds(5f);
        anim.SetBool("Stun", false);
        stunneed = false;
    }
    protected override void Death(float _destroyTime)
    {
        ResetAllAttack();
        isAlive = false;
        rb.velocity = new Vector2(rb.velocity.x, -25);
        anim.SetTrigger("Died");
    }
    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}
