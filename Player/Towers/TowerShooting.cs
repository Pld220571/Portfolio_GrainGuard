using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    [Header("Tower Info")]
    [SerializeField] private float _MinDistance;
    [SerializeField] private int currentUpgradeLevel = 0;

    [Header("Shoot Enemy")]
    [SerializeField] private GameObject _ProjectilePrefab;
    [SerializeField] private Transform _ProjectileExit;
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private float _BaseProjectileCooldown;
    [SerializeField] private float _ProjectileCooldown;

    private float lastShootTime;

    [Header("Tower sprites")]
    [SerializeField] private SpriteRenderer towerSprites;
    [SerializeField] private SpriteRenderer rankSprites;
    [SerializeField] private Sprite[] baseSprites;
    [SerializeField] private Sprite[] upgrade1Sprites;
    [SerializeField] private Sprite[] upgrade2Sprites;
    private Sprite rank1Sprites;
    [SerializeField] private Sprite rank2Sprites;
    [SerializeField] private Sprite rank3Sprites;

    private AudioManager audioManager;
    private PauseHandler pauseHandler;
    private TownhallUpgrade townhallUpgrades;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        pauseHandler = FindObjectOfType<PauseHandler>();
        townhallUpgrades = FindObjectOfType<TownhallUpgrade>();
    }

    private void Update()
    {
        if (!pauseHandler.gameOver)
        {
            EnemyHealth closestEnemy = LocateClosestEnemy();
            ShootClosestEnemy(closestEnemy);
        }
    }

    private void ShootClosestEnemy(EnemyHealth closestEnemy)
    {
        if (closestEnemy != null)
        {
            if (Time.time > lastShootTime + _ProjectileCooldown)
            {
                CalculateCooldown();
                lastShootTime = Time.time;
                Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
                GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.AddForce(direction * _ProjectileSpeed, ForceMode2D.Impulse);

                // Set the reference to this tower on the projectile
                BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
                if (baseProjectile != null)
                {
                    baseProjectile.SetShooter(this);
                }

                UpdateTowerSprite(direction);
                audioManager.PlaySFX(audioManager.towerShootSFX, 1f);
            }
        }
    }

    private EnemyHealth LocateClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MinDistance);
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

    // Function to draw a gizmo representing the enemy's range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MinDistance);
    }

    private void CalculateCooldown()
    {
        _ProjectileCooldown = _BaseProjectileCooldown * townhallUpgrades.cooldownMultiplier;
        Debug.Log("Cooldowj: " + _ProjectileCooldown);
    }

    public void UpgradeTower()
    {
        currentUpgradeLevel++;

        switch (currentUpgradeLevel)
        {
            case 0:
                rankSprites.sprite = rank1Sprites;
                break;
            case 1:
                rankSprites.sprite = rank2Sprites;
                break;
            case 2:
                rankSprites.sprite = rank3Sprites;
                break;
            default:
                rankSprites.sprite = rank1Sprites;
                break;
        }
    }

    public void UpdateTowerSprite(Vector3 directions)
    {
        Sprite[] sprites;
        switch (currentUpgradeLevel)
        {
            case 0:
                sprites = baseSprites;
                break;
            case 1:
                sprites = upgrade1Sprites;
                break;
            case 2:
                sprites = upgrade2Sprites;
                break;
            default:
                sprites = baseSprites;
                break;
        }

        // Calculate angle in radians
        float angle = Mathf.Atan2(directions.y, directions.x) * Mathf.Rad2Deg;

        // Normalize angle to be between 0 and 360
        if (angle < 0)
            angle += 360;

        // Determine which sprite to use based on angle
        int spriteIndex = Mathf.RoundToInt(angle / 45) % 8; // 360 degrees divided into 8 directions

        // Update the sprite
        if (spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            towerSprites.sprite = sprites[spriteIndex];
        }
    }
}
