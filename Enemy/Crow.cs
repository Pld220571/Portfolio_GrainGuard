using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Crow : MonoBehaviour
{
    public float Speed;
    public int GainGold; // Amount of gold gained when the Crow is defeated
    public int GainXP; // Amount of experience gained when the Crow is defeated

    [SerializeField] private int _EnemyDamage;
    [SerializeField] private GameObject _PrefabFeatherParticle;

    private CropsCheck _grains; // Reference to the closest crops
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
            FindClosestTower(); // Find the closest crops to move towards
            MoveToGrains(); // Move towards the closest crops or TownHall
            float direction = GetDirection(); // Get the direction to face
            _animator.SetFloat("direction", direction); // Set the animator's direction parameter
        }
    }

    public void FindClosestTower()
    {
        float distanceToClosestBuilding = Mathf.Infinity; // Initialize distance to infinity
        CropsCheck closestTower = null; // Variable to hold the closest crops
        CropsCheck[] allTowers = GameObject.FindObjectsOfType<CropsCheck>(); // Find all CropsCheck instances in the scene

        foreach (CropsCheck currentTower in allTowers) // Loop through each crops
        {
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude; // Calculate squared distance to the current crops

            if (distanceToTower < distanceToClosestBuilding) // Check if this crops is closer than the previous closest
            {
                distanceToClosestBuilding = distanceToTower; // Update the closest distance
                closestTower = currentTower; // Update the closest crops reference
            }

            _grains = closestTower; // Set the grains reference to the closest crops found
        }
    }

    private void MoveToGrains() // Move the Crow towards the closest crops or TownHall
    { 
        if (_grains != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _grains.transform.position, Speed * Time.deltaTime); // Move the Crow towards the closest crops, if it exists
        }
        else 
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _townHall.transform.position, Speed * Time.deltaTime); // If no crops are found, move towards the TownHall
        }
    }

    private float GetDirection()
    {
        Vector2 direction; // Variable to hold the direction vector

        if (_grains != null)
        {
            direction = (_grains.transform.position - transform.position).normalized;
        }
        else
        {
            direction = (_townHall.transform.position - transform.position).normalized;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle in degrees
        return angle; // Return the angle for the animation
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.CompareTag("Crops"))
        {
            Instantiate(_PrefabFeatherParticle, transform.position, Quaternion.identity); // Create a feather particle effect at the Crow's position
            _spawner.EnemyDestroyed(); // Notify the spawner that this enemy is destroyed
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
}