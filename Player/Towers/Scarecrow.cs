using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _MinDistance;
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
        _animator.SetFloat("animarionSpeed", _AnimationSpeed);
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (_currentTarget == null)
        {
            LocateClosestEnemy();
            _animator.SetInteger("scarecrowState", 0);

            return;
        }

        float shootingDirection = GetDirection();
        _animator.SetFloat("direction", shootingDirection);
        _animator.SetInteger("scarecrowState", 1);
    }

    private void LocateClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MinDistance);
        EnemyHealth closestEnemy = null;
        float closestDistance = Mathf.Infinity;

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

                _currentTarget = closestEnemy;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MinDistance);
    }

    private void ShootClosestEnemy()
    {
        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;
            CooldownMultiplier();
            GameObject projectile = Instantiate(_ProjectilePrefab, _ProjectileExit.position, _ProjectileExit.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * _ProjectileSpeed, ForceMode2D.Impulse);
            AntiAirProjectile proj = projectile.GetComponent<AntiAirProjectile>();

            if (proj != null)
            {
                proj.SetShooter(this);
            }

            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            rb.transform.rotation = Quaternion.Euler(0, 0, rot + 90);
            _audioManager.PlaySFX(_audioManager.scarecrowthrowSFX, 1f);
        }
    }

    private float GetDirection()
    {
        Vector2 spriteDirection = (_currentTarget.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg;

        return angle;
    }

    private void CooldownMultiplier()
    {
        _AnimationSpeed = 1f / _townhallUpgrade.cooldownMultiplier;

        _animator.SetFloat("animarionSpeed", _AnimationSpeed);
    }
}