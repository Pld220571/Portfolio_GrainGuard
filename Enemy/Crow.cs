using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Crow : MonoBehaviour
{
    public float Speed;

    // Amount of gold gained when the Crow is defeated
    public int GainGold;

    // Amount of experience gained when the Crow is defeated
    public int GainXP;

    [SerializeField] private int _EnemyDamage;
    [SerializeField] private GameObject _PrefabFeatherParticle;

    // Reference to the closest crops
    private CropsCheck _grains;
    private TownHallCheck _townHall;
    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;
    private bool _isDead;

    private void Start()
    {
        _townHall = FindObjectOfType<TownHallCheck>();
        _animator = GetComponent<Animator>();
        _spawner = FindObjectOfType<Spawner>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
    }

    void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            FindClosestTower();

            // Move towards the closest crops or TownHall
            MoveToGrains();

            // Get the direction to face
            float direction = GetDirection();

            // Set the animator's direction parameter
            _animator.SetFloat("direction", direction);
        }
    }

    public void FindClosestTower()
    {
        // Initialize distance to infinity
        float distanceToClosestBuilding = Mathf.Infinity;

        // Variable to hold the closest crops
        CropsCheck closestTower = null;

        // Find all CropsCheck instances in the scene
        CropsCheck[] allTowers = GameObject.FindObjectsOfType<CropsCheck>();

        // Loop through each crops
        foreach (CropsCheck currentTower in allTowers)
        {
            // Calculate squared distance to the current crops
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude;

            // Check if this crops is closer than the previous closest
            if (distanceToTower < distanceToClosestBuilding)
            {
                // Update the closest distance
                distanceToClosestBuilding = distanceToTower;

                // Update the closest crops reference
                closestTower = currentTower;
            }

            // Set the grains reference to the closest crops found
            _grains = closestTower;
        }
    }

    // Move the Crow towards the closest crops or TownHall
    private void MoveToGrains()
    { 
        if (_grains != null)
        {
            // Move the Crow towards the closest crops, if it exists
            transform.position = Vector2.MoveTowards(this.transform.position, _grains.transform.position, Speed * Time.deltaTime);
        }
        else 
        {
            // If no crops are found, move towards the TownHall
            transform.position = Vector2.MoveTowards(this.transform.position, _townHall.transform.position, Speed * Time.deltaTime);
        }
    }

    private float GetDirection()
    {

        // Variable to hold the direction vector
        Vector2 direction;

        if (_grains != null)
        {
            direction = (_grains.transform.position - transform.position).normalized;
        }
        else
        {
            direction = (_townHall.transform.position - transform.position).normalized;
        }

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Return the angle for the animation
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.CompareTag("Crops"))
        {
            // Create a feather particle effect at the Crow's position
            Instantiate(_PrefabFeatherParticle, transform.position, Quaternion.identity);

            // Notify the spawner that this enemy is destroyed
            _spawner.EnemyDestroyed();
            _isDead = true;
            collision.GetComponent<CropsHealth>().ChangeHealth(-_EnemyDamage);
            Destroy(gameObject);
        }

        if (collision.CompareTag("TownHall"))
        {
            Instantiate(_PrefabFeatherParticle, transform.position, Quaternion.identity);
            _spawner.EnemyDestroyed();
            _isDead = true;
            collision.GetComponent<TownHallHealth>().ChangeHealth(-_EnemyDamage);
            Destroy(gameObject);
        }

    }