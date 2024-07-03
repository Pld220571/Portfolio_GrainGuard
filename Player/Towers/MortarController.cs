using UnityEngine;

public class MortarController : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius;

    [Header("Shooting")]
    [SerializeField] private float baseMortarCooldown;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileExit;
    private float lastMortarShot;
    [SerializeField] private float mortarCooldown;

    private PauseHandler pauseHandler;
    private TownhallUpgrade townhallUpgrade;
    private AudioManager audioManager;

    private void Start()
    {
        pauseHandler = FindObjectOfType<PauseHandler>();
        townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (!pauseHandler.gameOver)
        {
            EnemyHealth closestEnemy = DetectClosestEnemy();
            if (closestEnemy != null)
            {
                Shoot(closestEnemy);
            }
        }
    }

    EnemyHealth DetectClosestEnemy()
    {
        // Find all colliders within the detection radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        EnemyHealth closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // Iterate through the colliders to find the closest enemy with an EnemyHealth component
        foreach (var collider in hitColliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemyHealth != null && enemy.movementType == Enemy.MovementType.Land)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemyHealth;
                }
            }
        }

        return closestEnemy;
    }

    private void Shoot(EnemyHealth closestEnemy)
    {
        if (Time.time <= lastMortarShot + mortarCooldown)
        {
            return;
        }

        lastMortarShot = Time.time;

        GameObject projectile = Instantiate(projectilePrefab, projectileExit.position, projectileExit.rotation);
        audioManager.PlaySFX(audioManager.MortarshootSFX, 1);

        MortarBullet projScript = projectile.GetComponent<MortarBullet>();
        if (projScript != null)
        {
            projScript.SetShooter(this); // Set the shooter reference on the projectile
            projScript.Launch(closestEnemy.transform, projectileSpeed);
            CalculateCooldown();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void CalculateCooldown()
    {
        mortarCooldown = baseMortarCooldown * townhallUpgrade.cooldownMultiplier;
    }
}
