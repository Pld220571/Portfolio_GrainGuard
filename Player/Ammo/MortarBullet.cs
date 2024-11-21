using System.Collections;
using UnityEngine;

public class MortarBullet : Projectile // Class representing a mortar bullet projectile
{
    public enum ExplosionSoundType // Enum to define different explosion sound types
    {
        Lv1AndLv2, // Sound for levels 1 and 2
        Lv3 // Sound for level 3
    }

    public float ArcHeight; // Height of the projectile's arc during flight
    public float FlightDuration = 2.0f; // Duration of the projectile's flight

    private Vector3 _startPoint; // Starting position of the projectile
    private Vector3 _lastKnownTargetPosition; // Last known position of the target
    private Transform _target; // Transform of the target the projectile is aimed at
    private Collider2D _col; // Reference to the Collider2D component

    [Header("Explosion")] // Header for explosion-related variables in the inspector
    [SerializeField] private float _ExpRadius; // Radius of the explosion effect
    [SerializeField] private GameObject _PrefabWaterMelonExplosion; // Prefab for the watermelon explosion effect
    [SerializeField] private GameObject _PrefabCrater; // Prefab for the crater effect
    [SerializeField] private ExplosionSoundType _ExplosionSoundType; // Type of explosion sound to play

    private AudioManager _audioManager; // Reference to the AudioManager for playing sounds

    public override void Start() // Override the Start method from the base Projectile class
    {
        base.Start(); // Call the base class Start method
        _col = GetComponent<Collider2D>(); // Get the Collider2D component

        if (_col != null) // If the collider is found
        {
            _col.enabled = false; // Disable the collider initially
        }

        _audioManager = FindObjectOfType<AudioManager>(); // Find the AudioManager in the scene
    }

    public void Launch(Transform targetTransform, float projectileSpeed) // Method to launch the projectile towards a target
    {
        _startPoint = transform.position; // Set the starting point of the projectile
        _target = targetTransform; // Set the target transform
        _lastKnownTargetPosition = _target.position; // Store the initial position of the target
        StartCoroutine(MoveProjectile()); // Start the coroutine to move the projectile
    }

    private IEnumerator MoveProjectile() // Coroutine to handle the projectile's movement
    {
        float startTime = Time.time; // Record the start time of the movement

        while (true) // Loop until the projectile lands
        {
            float elapsedTime = Time.time - startTime; // Calculate elapsed time
            float fractionOfJourney = elapsedTime / FlightDuration; // Calculate how far along the journey the projectile is

            if (_target != null) // If there is a target
            {
                _lastKnownTargetPosition = _target.position; // Update the last known target position
            }

            // Calculate the current position of the projectile using linear interpolation and arc height
            Vector3 currentPosition = Vector3.Lerp(_startPoint, _lastKnownTargetPosition, fractionOfJourney);
            float height = Mathf.Sin(Mathf.PI * fractionOfJourney) * ArcHeight; // Calculate the height for the arc
            transform.position = new Vector3(currentPosition.x, currentPosition.y + height, currentPosition.z); // Set the projectile's position

            if (fractionOfJourney >= 1f) // If the projectile has reached its destination
            {
                OnLanding(); // Handle landing effects
                yield break; // Exit the coroutine
            }

            yield return null; // Wait for the next frame
        }
    }

    private void OnLanding() // Method to handle actions when the projectile lands
    {
        if (_col != null) // If the collider is found
        {
            _col.enabled = true; // Enable the collider for the explosion detection
        }

        Vector3 landingPosition = transform.position; // Get the landing position
        Instantiate(_PrefabCrater, landingPosition, Quaternion.identity); // Instantiate the crater effect
        Instantiate(_PrefabWaterMelonExplosion, landingPosition, Quaternion.identity); // Instantiate the explosion effect
        Explosion(); // Call the explosion method
        Destroy(gameObject); // Destroy the projectile
    }

    protected void OnTriggerEnter2D(Collider2D collision) // Method called when the projectile collides with another collider
    {
        if (collision.CompareTag("Enemy")) // Check if the collided object has the tag "Enemy"
        {
            Explosion(); // Call the explosion method
        }
        else
        {
            Explosion(); // Call the explosion method for other collisions as well
        }
    }

    private void Explosion() // Method to handle the explosion logic
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _ExpRadius);

        foreach (Collider2D nearby in colliders)
        {
            EnemyHealth enemyHealth = nearby.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.SetShooter(_Shooter);
                enemyHealth.ChangeHealth(-_ProjectileDamage);
            }
        }

        Instantiate(_PrefabCrater, transform.position, Quaternion.identity);
        Instantiate(_PrefabWaterMelonExplosion, transform.position, Quaternion.identity);
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
        Gizmos.DrawWireSphere(transform.position, _ExpRadius);
    }
}