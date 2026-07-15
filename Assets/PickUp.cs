using System.Collections;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    protected bool used;

    [Tooltip("How long after touching the item before the Use() function fires")]
    public float useDelay = 0f;

    [Tooltip("How long after using the item before the Used() function fires")]
    public float usedDelay = 0f;

    [System.Serializable]
    public struct Animation
    {
        public float Frequency;
        public Vector3 direction;
        public Vector3 torque;

        [Tooltip("Effect that play when the item is touched")]
        public ParticleSystem destroyEffectPrefab;
        [Tooltip("Effect that play on the target affected by the item")]
        public ParticleSystem targetEffectPrefab;
    }

    public Animation anim = new Animation
    {
        Frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    Vector3 initialPosition;
    float initialOffset;
    
    protected virtual void Update()
    {
        transform.position = initialPosition + anim.direction * Mathf.Sin((Time.time + initialOffset) * anim.Frequency);
        transform.Rotate(anim.torque * Time.deltaTime);
    }

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, anim.Frequency);
    }

    protected virtual void OnTriggerEnter2D(Collider2D _other)
    {
        if (used)
        {
            return;
        }

        if (_other.TryGetComponent(out PlayerController p))
        {
            StartCoroutine(HandleUse(p));
        }
    }

    public virtual void Touch(PlayerController p)
    {
        if (anim.destroyEffectPrefab)
        {
            ParticleSystem fx = Instantiate(anim.destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, fx.main.duration);
        }
    }

    public virtual void Use(PlayerController p)
    {
        used = true;
        if (anim.targetEffectPrefab)
        {
            ParticleSystem fx = Instantiate(anim.targetEffectPrefab, p.transform);
            Destroy(fx, fx.main.duration);
        }
    }

    public virtual void Used(PlayerController p)
    {
        Destroy(gameObject);
    }

    protected virtual IEnumerator HandleUse(PlayerController p)
    {
        Touch(p);

        yield return Delay(useDelay);
        Use(p);

        yield return Delay(usedDelay);
        Used(p);
    }

    protected virtual IEnumerator Delay(float duration)
    {
        WaitForSecondsRealtime r = new WaitForSecondsRealtime(.05f);
        while (duration > 0)
        {
            yield return r;
            if (!GameManager.Instance.isPaused)
            {
                duration -= r.waitTime;
            }
        }
    }
}
