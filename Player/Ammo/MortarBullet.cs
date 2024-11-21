using System.Collections;
using UnityEngine;

public class MortarBullet : Projectile
{
    public enum ExplosionSoundType
    {
        Lv1AndLv2,
        Lv3
    }

    public float ArcHeight; // Height of the projectile's arc during flight
    public float FlightDuration = 2.0f;

    private Vector3 _startPoint;
    private Vector3 _lastKnownTargetPosition;
    private Transform _target;
    private Collider2D _col;

    [Header("Explosion")]
    [SerializeField] private float _ExpRadius; // Radius of the explosion effect
    [SerializeField] private GameObject _PrefabWaterMelonExplosion;
    [SerializeField] private GameObject _PrefabCrater;
    [SerializeField] private ExplosionSoundType _ExplosionSoundType;

    private AudioManager _audioManager;

    public override void Start()
    {
        base.Start();
        _col = GetComponent<Collider2D>();

        if (_col != null)
        {
            _col.enabled = false;
        }

        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void Launch(Transform targetTransform, float projectileSpeed) // Method to launch the projectile towards a target
    {
        _startPoint = transform.position;
        _target = targetTransform;
        _lastKnownTargetPosition = _target.position; // Store the initial position of the target
        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        float startTime = Time.time; // Record the start time of the movement

        while (true) // Loop until the projectile lands
        {
            float elapsedTime = Time.time - startTime;
            float fractionOfJourney = elapsedTime / FlightDuration; // Calculate how far along the journey the projectile is

            if (_target != null)
            {
                _lastKnownTargetPosition = _target.position; // Update the last known target position
            }

            Vector3 currentPosition = Vector3.Lerp(_startPoint, _lastKnownTargetPosition, fractionOfJourney); // Calculate the current position of the projectile using linear interpolation and arc height
            float height = Mathf.Sin(Mathf.PI * fractionOfJourney) * ArcHeight; // Calculate the height for the arc
            transform.position = new Vector3(currentPosition.x, currentPosition.y + height, currentPosition.z);

            if (fractionOfJourney >= 1f) // If the projectile has reached its destination
            {
                OnLanding(); // Handle landing effects
                yield break;
            }

            yield return null;
        }
    }

    private void OnLanding()
    {
        if (_col != null)
        {
            _col.enabled = true;
        }

        Vector3 landingPosition = transform.position; // Get the landing position
        Instantiate(_PrefabCrater, landingPosition, Quaternion.identity); // Create a crater effect
        Instantiate(_PrefabWaterMelonExplosion, landingPosition, Quaternion.identity); // Create an watermelon explosion effect
        Explosion();
        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Explosion();
        }
        else
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _ExpRadius);

        foreach (Collider2D nearby in colliders)
        {
            EnemyHealth enemyHealth = nearby.GetComponent<EnemyHealth>(); // Attempt to get the EnemyHealth component from the nearby collider

            if (enemyHealth != null) // If an EnemyHealth component was found
            {
                enemyHealth.SetShooter(_Shooter); // Set the shooter reference on the enemy's health component
                enemyHealth.ChangeHealth(-_ProjectileDamage); // Deal damage to the enemy
            }
        }

        Instantiate(_PrefabCrater, transform.position, Quaternion.identity); // Create an crater effect at the projectile's current position
        Instantiate(_PrefabWaterMelonExplosion, transform.position, Quaternion.identity); // Create an explosion effect at the projectile's current position
        Destroy(gameObject);

        switch (_ExplosionSoundType)
        {
            case ExplosionSoundType.Lv1AndLv2:
                _audioManager.PlaySFX(_audioManager.MortarBulletExplosionLv1And2SFX, 1f);
                break;
            case ExplosionSoundType.Lv3:
                _audioManager.PlaySFX(_audioManager.MortarBulletExplosionLv3SFX, 1f);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _ExpRadius); // Draw a wireframe sphere to represent the explosion radius
    }
}