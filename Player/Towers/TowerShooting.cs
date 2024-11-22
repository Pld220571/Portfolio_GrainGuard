using System.Diagnostics;
using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    [Header("Tower Info")]
    [SerializeField] private float _MinDistance; // Minimum distance to detect enemies
    [SerializeField] private int _CurrentUpgradeLevel = 0;

    [Header("Shoot Enemy")]
    [SerializeField] private GameObject _ProjectilePrefab;
    [SerializeField] private Transform _ProjectileExit;
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private float _BaseProjectileCooldown;
    [SerializeField] private float _ProjectileCooldown;

    private float _lastShootTime; // Timestamp of the last shot fired

    [Header("Tower sprites")]
    [SerializeField] private SpriteRenderer _TowerSprites;
    [SerializeField] private SpriteRenderer _RankSprites;
    [SerializeField] private Sprite[] _BaseSprites;
    [SerializeField] private Sprite[] _Upgrade1Sprites;
    [SerializeField] private Sprite[] _Upgrade2Sprites;
    [SerializeField] private Sprite _Rank2Sprites;
    [SerializeField] private Sprite _Rank3Sprites;

    private Sprite _rank1Sprites; // Placeholder for rank 1 sprites
    private AudioManager _audioManager;
    private PauseHandler _pauseHandler;
    private TownhallUpgrade _townhallUpgrades;

    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _pauseHandler = FindObjectOfType<PauseHandler>();
        _townhallUpgrades = FindObjectOfType<TownhallUpgrade>();
    }

    private void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            EnemyHealth closestEnemy = LocateClosestEnemy();
            ShootClosestEnemy(closestEnemy);
        }
    }

    private void ShootClosestEnemy(EnemyHealth closestEnemy)
    {
        if (closestEnemy != null)
        {
            if (Time.time > _lastShootTime + _ProjectileCooldown)
            {
                CalculateCooldown(); // Update the projectile cooldown based on upgrades
                _lastShootTime = Time.time; // Record the time of the shot
                Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
                GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.AddForce(direction * _ProjectileSpeed, ForceMode2D.Impulse);
                BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>(); // Set the shooter for the projectile if it has a BaseProjectile component

                if (baseProjectile != null)
                {
                    baseProjectile.SetShooter(this);
                }

                UpdateTowerSprite(direction); // Update the tower sprite based on the direction of the shot
                _audioManager.PlaySFX(_audioManager.towerShootSFX, 1f);
            }
        }
    }

    private EnemyHealth LocateClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MinDistance); // Use a circle overlap to detect enemies within the minimum distance specified
        EnemyHealth closestEnemy = null; // Variable to store the closest enemy found
        float closestDistance = Mathf.Infinity; // Initialize closest distance with infinity for comparison

        foreach (var collider in hitColliders) // Loop through detected colliders to find the closest enemy
        {
            EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>(); // Get the EnemyHealth and Enemy components from the collider
            Enemy enemy = collider.GetComponent<Enemy>();
            
            if (enemyHealth != null && enemy.movementType == Enemy.MovementType.Land) // Check if the collider is an enemy and if it's of the land movement type
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);

                if (distanceToEnemy < closestDistance) // If this enemy is closer than the previously found closest enemy, update closestEnemy
                {
                    closestDistance = distanceToEnemy; // Update closest distance
                    closestEnemy = enemyHealth; // Update closest enemy reference
                }
            }
        }

        return closestEnemy;
    }

    void OnDrawGizmosSelected() // Draw a visual representation of the detection range in the editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MinDistance); // Draw a wire sphere to represent the detection range
    }
    
    private void CalculateCooldown() // Calculate the projectile cooldown based on upgrades
    {
        _ProjectileCooldown = _BaseProjectileCooldown * _townhallUpgrades.cooldownMultiplier; // Update the projectile cooldown based on the base cooldown and townhall upgrade multiplier
    }

    public void UpgradeTower()
    {
        _CurrentUpgradeLevel++;

        switch (_CurrentUpgradeLevel)
        {
            case 0:
                _RankSprites.sprite = _rank1Sprites;
                break;
            case 1:
                _RankSprites.sprite = _Rank2Sprites;
                break;
            case 2:
                _RankSprites.sprite = _Rank3Sprites;
                break;
            default:
                _RankSprites.sprite = _rank1Sprites;
                break;
        }
    }

    public void UpdateTowerSprite(Vector3 directions) // Update the tower sprite based on the shooting direction and upgrade level
    {
        Sprite[] sprites; // Array to hold the appropriate sprites based on the upgrade level

        switch (_CurrentUpgradeLevel) // Determine which sprite array to use based on the current upgrade level
        {
            case 0:
                sprites = _BaseSprites;
                break;
            case 1:
                sprites = _Upgrade1Sprites;
                break;
            case 2:
                sprites = _Upgrade2Sprites;
                break;
            default:
                sprites = _BaseSprites;
                break;
        }
        
        float angle = Mathf.Atan2(directions.y, directions.x) * Mathf.Rad2Deg; // Calculate the angle based on the direction vector
        
        if (angle < 0) 
            angle += 360; // Normalize angle to be within 0-360 degrees

        int spriteIndex = Mathf.RoundToInt(angle / 45) % 8; // Determine the sprite index based on the angle (8 directions)

        if (spriteIndex >= 0 && spriteIndex < sprites.Length) // Update the tower sprite if the index is valid
        {
            _TowerSprites.sprite = sprites[spriteIndex]; // Set the sprite based on the calculated index
        }
    }
}