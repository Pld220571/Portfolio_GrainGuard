using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bunny : MonoBehaviour
{
    #region variables
    [HideInInspector] public GameObject TownHall;
    public float Speed;
    public int GainGold; // Amount of gold gained when the Bunny is killed
    public int GainXP; // Amount of experience gained when the Bunny is killed

    [SerializeField] private int _EnemyDamage;
    
    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;
    private TowerCheck _target; // The closest tower that the Bunny is targeting

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
            FindClosestTower(); // Call to find the closest tower to target
            MoveToTarget(); // Move the Bunny towards the target tower
            float direction = GetDirection(); // Get the direction angle towards the target
            _animator.SetFloat("direction", direction); // Update animator with the current direction
        }
    }

    public void FindClosestTower()
    {
        float distanceToClosestBuilding = Mathf.Infinity; // Initialize to maximum possible distance
        TowerCheck closestTower = null; // Variable to store the closest tower found
        TowerCheck[] allTowers = GameObject.FindObjectsOfType<TowerCheck>(); // Get all TowerCheck objects in the scene

        foreach (TowerCheck currentTower in allTowers)// Loop through all found towers to determine the closest one
        {
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude; // Calculate squared distance

            if (distanceToTower < distanceToClosestBuilding) // Check if this tower is closer than the previous closest
            {
                distanceToClosestBuilding = distanceToTower; // Update the closest distance
                closestTower = currentTower; // Update the closest tower reference
            }

            _target = closestTower; // Set the target to the closest tower found
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
        Vector2 direction; // Variable to hold the direction vector
        if (_target != null)
        {
            direction = (_target.transform.position - transform.position).normalized; // Calculate direction towards the tower, if it exists
        }
        else
        {
            direction = (TownHall.transform.position - transform.position).normalized; // If there is no tower, move towards the TownHall
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert direction to angle in degrees
        return angle; // Return the angle for animator
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.CompareTag("Tower"))
        {
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity); // Create an explosion effect at the Bunny's position
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);
            collision.GetComponent<TowerHealth>().ChangeHealth(-_EnemyDamage); // Apply damage to the Tower's health component
            _isDead = true;
            _spawner.EnemyDestroyed(); // Notify the spawner that this enemy is destroyed
            Destroy(gameObject);
        }

        if (collision.CompareTag("Crops"))
        {
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity); // Create an explosion effect
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1); // Play sound effect
            collision.GetComponent<CropsHealth>().ChangeHealth(-_EnemyDamage); // Damage the crops
            _isDead = true; // Mark the Bunny as dead
            _spawner.EnemyDestroyed(); // Notify the spawner
            Destroy(gameObject); // Remove the Bunny
        }

        if (collision.CompareTag("TownHall"))
        {
            Instantiate(_PrefabExplosion, transform.position, Quaternion.identity); // Create an explosion effect at the Bunny's position
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);
            collision.GetComponent<TownHallHealth>().ChangeHealth(-_EnemyDamage); // Damage the TownHall            
            _isDead = true;
            _spawner.EnemyDestroyed(); // Notify the spawner
            Destroy(gameObject);
        }
    }
}