using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class VersusPlayer : MonoBehaviour
{
    public VersusProjectileSpawner projectileSpawner;
    public GameObject enemy;
    public HealthManager healthManager;
    public GameObject floatingTextPrefab;
    public GameObject crossPrefab;
    public AudioClip eatSound;
    public AudioClip successSound;
    public LocalizedString localPlayer;

    private VersusPlayer enemyPlayer;

    private Queue<GameObject> crosses = new Queue<GameObject>();
    private GameObject nextCross;

    private float speed = 10f;

    private Vector3 enemyPositionDiff;
    private bool isOnRightSide;

    private Rigidbody2D rb;

    private bool isDead;
    private Animator animator;

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    public delegate void OnProjectileShotDelegate();
    public event OnProjectileShotDelegate OnProjectileShot;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPlayer = enemy.GetComponent<VersusPlayer>();
        rb = GetComponent<Rigidbody2D>();

        isOnRightSide = transform.position.x > 0;

        healthManager.SetName(GetName());
        healthManager.OnHealthZero += OnHealthZeroHandler;
        enemyPlayer.OnDeath += OnEnemyDeath;
    }

    private void OnDestroy()
    {
        healthManager.OnHealthZero -= OnHealthZeroHandler;
        enemyPlayer.OnDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        animator.SetTrigger("Laugh");
    }

    private void OnHealthZeroHandler()
    {
        if (SettingsManager.isVibrationEnabled)
        {
            Vibration.Vibrate(100);
        }

        isDead = true;
        while (crosses.TryDequeue(out GameObject cross)) // Destroy all crosses
        {
            Destroy(cross);
        }
        if (nextCross != null) Destroy(nextCross);

        rb.AddForce((-1) * enemyPositionDiff * 1000);
        rb.AddTorque(1000 * (isOnRightSide ? (-1) : 1));

        if (OnDeath != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnDeath();
        }
    }

    public string GetName()
    {
        return localPlayer.GetLocalizedString() + " " + (isOnRightSide ? "2" : "1");
    }

    public void DisableShield()
    {
        healthManager.DisableShieldLayer();
    }

    public void StartSpawningProjectiles()
    {
        StartCoroutine(InfiniteAttackRandomize());
    }

    public bool IsDead()
    {
        return isDead;
    }

    private IEnumerator InfiniteAttackRandomize()
    {
        while (!IsDead() && !enemyPlayer.IsDead())
        {
            animator.SetTrigger("StartAttack");
            yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
        }
    }

    public void SpawnProjectile()
    {
        projectileSpawner.SpawnProjectile((enemy.transform.position - gameObject.transform.position).normalized, gameObject.name);

        if (OnProjectileShot != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnProjectileShot();
        }
    }

    public void AddTargetPosition(Vector3 position)
    {
        GameObject crossGameObject = Instantiate(crossPrefab, GameObject.Find("Canvas Crosses").transform);
        crossGameObject.transform.position = position;
        crossGameObject.GetComponent<Image>().color = isOnRightSide ? new Color32(255, 117, 117, 255) : new Color32(112, 191, 255, 255);

        crosses.Enqueue(crossGameObject);
    }

    public bool IsAtTargetPosition()
    {
        if(nextCross == null)
        {
            return true;
        }

        return Vector3.Distance(transform.position, nextCross.transform.position) < 0.1f; // Include a small margin of error
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Border")) // if collides with border divider, stop movement (assign current position to target position)
        {
            PickNextCross();
        }
    }

    private void PickNextCross()
    {
        if(nextCross != null) Destroy(nextCross);

        if (crosses.TryDequeue(out GameObject cross))
        {
            nextCross = cross;
        } else
        {
            nextCross = null;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsDead() && !enemyPlayer.IsDead())
        {
            healthManager.TakeDamage(damage);
        }
    }

    public void Heal(int amount)
    {
        if (!IsDead())
        {
            healthManager.Heal(amount);

            AudioManager.instance.PlaySound(eatSound);
            AudioManager.instance.PlaySound(successSound);

            // Show floating text animation
            GameObject floatingText = Instantiate(floatingTextPrefab, gameObject.transform.position + Vector3.up, Quaternion.identity);
            floatingText.GetComponentInChildren<TextMesh>().color = new Color32(137, 255, 0, 255);
            floatingText.GetComponentInChildren<TextMesh>().text = "+" + amount.ToString();
        }
    }

    private void Update()
    {
        if(IsDead())
        {
            return;
        }

        enemyPositionDiff = enemy.transform.position - transform.position;
        enemyPositionDiff.Normalize();
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            (Mathf.Atan2(enemyPositionDiff.y, enemyPositionDiff.x) * Mathf.Rad2Deg) + (isOnRightSide ? 180 : 0)
        );

        if (!IsAtTargetPosition())
        {
            // transform.position = Vector3.MoveTowards(transform.position, nextCross.transform.position, speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, nextCross.transform.position, speed * Time.deltaTime);
        } else
        {
            PickNextCross();
        }
    }
}
