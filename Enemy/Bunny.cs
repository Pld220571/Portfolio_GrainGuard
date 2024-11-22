using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bunny : MonoBehaviour
{
    #region variables
    [HideInInspector] public GameObject TownHall;
    public float Speed;

    // Amount of gold gained when the Bunny is killed
    public int GainGold;

    // Amount of experience gained when the Bunny is killed
    public int GainXP;

    [SerializeField] private int _EnemyDamage;
    
    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;

    // The closest tower that the Bunny is targeting
    private TowerCheck _target;

    [SerializeField] private GameObject _PrefabExplosion;

    private bool _isDead;
    private AudioManager _audioManager;

    #endregion

    private void Start()
    {
        TownHall = GameObject.FindGameObjectWithTag("TownHall");
        _animator = GetComponent<Animator>();
        _spawner = FindAnyObjectByType<Spawner>();
        _audioManager = FindObjectOfType<AudioManager>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
    }

    private void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            // Call to find the closest tower to target
            FindClosestTower();

            // Move the Bunny towards the target tower
            MoveToTarget();

            // Get the direction angle towards the target
            float direction = GetDirection();

            // Update animator with the current direction
            _animator.SetFloat("direction", direction);
        }
    }

    public void FindClosestTower()
    {
        // Initialize to maximum possible distance
        float distanceToClosestBuilding = Mathf.Infinity;

        // Variable to store the closest tower found
        TowerCheck closestTower = null;

        // Get all TowerCheck objects in the scene
        TowerCheck[] allTowers = GameObject.FindObjectsOfType<TowerCheck>();

        // Loop through all found towers to determine the closest one
        foreach (TowerCheck currentTower in allTowers)
        {
            // Calculate squared distance
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude;

            // Check if this tower is closer than the previous closest
            if (distanceToTower < distanceToClosestBuilding)
            {
                // Update the closest distance
                distanceToClosestBuilding = distanceToTower;

                // Update the closest tower reference
                closestTower = currentTower;
            }

            // Set the target to the closest tower found
            _target = closestTower;
        }
    }

    private void MoveToTarget()
    {
        if (_target != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _target.transform.position, Speed * Time.deltaTime);
        }
    }

    private float GetDirection()
    {
        // Variable to hold the direction vector
        Vector2 direction;

        if (_target != null)
        {
            // Calculate direction towards the tower, if it exists
            direction = (_target.transform.position - transform.position).normalized;
        }
        else
        {
            // If there is no tower, move towards the TownHall
            direction = (TownHall.transform.position - transform.position).normalized;
        }

        // Convert direction to angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Return the angle for animator
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.CompareTag("Tower"))
        {
            // Create an explosion effect at the Bunny's position
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Apply damage to the Tower's health component
            collision.GetComponent<TowerHealth>().ChangeHealth(-_EnemyDamage);
            _isDead = true;

            // Notify the spawner that this enemy is destroyed
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }

        if (collision.CompareTag("Crops"))
        {
            // Create an explosion effect
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Damage the crops
            collision.GetComponent<CropsHealth>().ChangeHealth(-_EnemyDamage);
            _isDead = true;

            // Notify the spawner
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }

        if (collision.CompareTag("TownHall"))
        {
            // Create an explosion effect at the Bunny's position
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Damage the TownHall 
            collision.GetComponent<TownHallHealth>().ChangeHealth(-_EnemyDamage);           
            _isDead = true;

            // Notify the spawner
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }
    }
}