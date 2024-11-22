using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _MinDistance; // Minimum distance from the scarecrow to detect enemies
    [SerializeField] private GameObject _ProjectilePrefab;
    [SerializeField] private Transform _ProjectileExit;
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private float _AnimationSpeed;

    private Animator _animator;
    private EnemyHealth _currentTarget;
    private TownhallUpgrade _townhallUpgrade;
    private float _lastShootTime;
    private AudioManager _audioManager;
    #endregion

    private void Start()
    {
        _townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("animarionSpeed", _AnimationSpeed); // Set animation speed
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (_currentTarget == null)
        {
            LocateClosestEnemy();
            _animator.SetInteger("scarecrowState", 0); // Set scarecrow state to idle

            return;
        }

        float shootingDirection = GetDirection(); // Calculate shooting direction
        _animator.SetFloat("direction", shootingDirection); // Set animator direction and state
        _animator.SetInteger("scarecrowState", 1);
    }

    private void LocateClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MinDistance);
        EnemyHealth closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>(); // Check if the collider has an EnemyHealth component
            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemyHealth != null && enemy.movementType == Enemy.MovementType.air) // Check if the enemy is an air enemy
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                
                if (distanceToEnemy < closestDistance) // Update closest enemy if this one is closer
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemyHealth;
                }

                _currentTarget = closestEnemy; // Set the current target to the closest enemy
            }
        }
    }
    
    void OnDrawGizmosSelected() // Draw a gizmo to visualize the minimum distance
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MinDistance);
    }

    private void ShootClosestEnemy()
    {
        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.transform.position - transform.position).normalized; // Calculate direction to the target
            CooldownMultiplier(); // Apply cooldown multiplier to animation speed
            GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation); // Create a projectile
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * _ProjectileSpeed, ForceMode2D.Impulse);
            AntiAirProjectile proj = projectile.GetComponent<AntiAirProjectile>(); // Set the projectile's shooter

            if (proj != null)
            {
                proj.SetShooter(this);
            }

            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; // Rotate the projectile to face the target
            rb.transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            _audioManager.PlaySFX(_audioManager.scarecrowthrowSFX, 1f);
        }
    }

    private float GetDirection() // Calculate the direction to the target
    {
        Vector2 spriteDirection = (_currentTarget.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg;

        return angle;
    }

    private void CooldownMultiplier()
    {
        _AnimationSpeed = 1f / _townhallUpgrade.cooldownMultiplier; // Calculate the new animation speed based on the cooldown multiplier from the townhall upgrade
        _animator.SetFloat("animarionSpeed", _AnimationSpeed); // Update the animator's parameter to reflect the new animation speed
    }
}