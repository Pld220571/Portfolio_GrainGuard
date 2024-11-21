using UnityEngine;

public class MortarController : MonoBehaviour
{
    [Header("Detection")]
    public float DetectionRadius;

    [Header("Shooting")]
    [SerializeField] private float _BaseMortarCooldown;
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private GameObject _ProjectilePrefab;
    [SerializeField] private Transform _ProjectileExit;
    [SerializeField] private float _MortarCooldown;

    private float _lastMortarShot; // Timestamp of the last shot fired
    private PauseHandler _pauseHandler;
    private TownhallUpgrade _townhallUpgrade;
    private AudioManager _audioManager;

    private void Start()
    {
        _pauseHandler = FindObjectOfType<PauseHandler>();
        _townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            EnemyHealth closestEnemy = DetectClosestEnemy();

            if (closestEnemy != null)
            {
                Shoot(closestEnemy);
            }
        }
    }

    EnemyHealth DetectClosestEnemy() // Method to find the closest enemy within the detection radius
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, DetectionRadius); // Get all colliders within the detection radius
        EnemyHealth closestEnemy = null; // Variable to store the closest enemy
        float closestDistance = Mathf.Infinity; // Initialize the closest distance to infinity

        foreach (var collider in hitColliders) // Loop through all detected colliders
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>(); // Get the EnemyHealth component
            Enemy enemy = collider.GetComponent<Enemy>(); // Get the Enemy component

            if (enemyHealth != null && enemy.movementType == Enemy.MovementType.Land) // Check if the collider is a land enemy
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);

                if (distanceToEnemy < closestDistance) // If this enemy is closer than the previously found one
                {
                    closestDistance = distanceToEnemy; // Update the closest distance
                    closestEnemy = enemyHealth; // Update the closest enemy reference
                }
            }
        }

        return closestEnemy; // Return the closest enemy found
    }

    private void Shoot(EnemyHealth closestEnemy)
    {
        if (Time.time <= _lastMortarShot + _MortarCooldown) // Check if the cooldown period has not elapsed
        {
            return; // Exit if still on cooldown
        }

        _lastMortarShot = Time.time; // Update the last shot time
        GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation); // Create a projectile
        _audioManager.PlaySFX(_audioManager.MortarshootSFX, 1);
        MortarBullet projScript = projectile.GetComponent<MortarBullet>(); // Get the MortarBullet component from the projectile

        if (projScript != null) // If the projectile has a MortarBullet component
        {
            projScript.SetShooter(this); // Set the shooter reference in the projectile
            projScript.Launch(closestEnemy.transform, _ProjectileSpeed);
            CalculateCooldown(); // Calculate the new cooldown based on upgrades
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius); // Draw a wire sphere to visualize the detection radius
    }

    private void CalculateCooldown() // Method to adjust the cooldown based on town hall upgrades
    {
        _MortarCooldown = _BaseMortarCooldown * _townhallUpgrade.cooldownMultiplier; // Calculate the new cooldown time
    }
}