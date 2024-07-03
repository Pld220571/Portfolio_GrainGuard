using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _MinDistance; // minimum distance to consider an enemy close
    [SerializeField] private GameObject _ProjectilePrefab;
    [SerializeField] private Transform _ProjectileExit; // transform where the projectile will exit
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private float _AnimationSpeed;

    private Animator _animator;
    private EnemyHealth _currentTarget; // current target enemy
    private TownhallUpgrade townhallUpgrade;
    private float lastShootTime; // Add this if you need a shooting cooldown
    //[SerializeField] private float _ProjectileCooldown; // Set the cooldown duration
    private AudioManager audioManager;

    #endregion

    private void Start()
    {
        townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("animarionSpeed", _AnimationSpeed);
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        // If no target is set, find the closest enemy and set it as the target
        if (_currentTarget == null)
        {
            LocateClosestEnemy();
            // Set the animator state to "Idle"
            _animator.SetInteger("scarecrowState", 0);
            return;
        }

        // Update the animator according to the direction that the Scarecrow is shooting at
        float shootingDirection = GetDirection();
        _animator.SetFloat("direction", shootingDirection);

        // Set the animator state to "Shooting"
        _animator.SetInteger("scarecrowState", 1);

        //// Call the ShootClosestEnemy method to shoot at the current target
        //if (Time.time > lastShootTime + _ProjectileCooldown)
        //{
        //    ShootClosestEnemy();
        //    lastShootTime = Time.time; // Update the last shoot time
        //}
    }

    // Method to find the closest enemy within the minimum distance
    private void LocateClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MinDistance); // Uses a physics overlap circle to find all colliders within the detection range.
        EnemyHealth closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // Loops through the colliders and finds the closest tower with a TowerCheck component.
        foreach (var collider in hitColliders)
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemyHealth != null && enemy.movementType == Enemy.MovementType.air)
            {
                float distanceToTower = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToTower < closestDistance)
                {
                    closestDistance = distanceToTower;
                    closestEnemy = enemyHealth;
                }
                _currentTarget = closestEnemy; // Updates the _currentTarget variable with the closest tower.
            }
        }
    }

    // Function to draw a gizmo representing the enemy's range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MinDistance);
    }

    // Method to shoot a projectile at the closest enemy (ONLY CALLED FROM THE ANIMATION)
    private void ShootClosestEnemy()
    {
        // Only shoot if there is a closest enemy
        if (_currentTarget != null)
        {
            // Calculate the direction between the Scarecrow and the closest enemy
            Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;

            // Set the cooldown speed
            CooldownMultiplier();

            // Instantiate a new projectile at the projectile exit position
            GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation);

            // Add force to the projectile
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * _ProjectileSpeed, ForceMode2D.Impulse);

            // Set the reference to this scarecrow on the projectile
            AntiAirProjectile proj = projectile.GetComponent<AntiAirProjectile>();
            if (proj != null)
            {
                proj.SetShooter(this);
            }

            // Calculate the rotation for the projectile
            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            rb.transform.rotation = Quaternion.Euler(0, 0, rot + 90);

            // Play shooting sound if applicable
            audioManager.PlaySFX(audioManager.scarecrowthrowSFX, 1f);
        }
    }

    // Method to calculate the direction to face based on the target enemy
    private float GetDirection()
    {
        // Calculate the direction from the Scarecrow to the target enemy
        Vector2 spriteDirection = (_currentTarget.transform.position - transform.position).normalized;

        // Convert the direction to an angle in degrees
        float angle = Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void CooldownMultiplier()
    {
        _AnimationSpeed = 1f / townhallUpgrade.cooldownMultiplier;

        _animator.SetFloat("animarionSpeed", _AnimationSpeed);
    }
}
