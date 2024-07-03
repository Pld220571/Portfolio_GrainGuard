using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Crow : MonoBehaviour
{
    [SerializeField] public float _Speed; // movement speed of the enemy
    [SerializeField] private int _EnemyDamage; // damage dealt to the grains when the enemy collides with them

    public int gainGold;
    public int gainXP;

    private CropsCheck _grains;
    private TownHallCheck _townHall;
    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;
    private bool died;

    [SerializeField] private GameObject prefabFeatherParticle;

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
            // Move the enemy towards the grains
            MoveToGrains();
            // Get the direction from the enemy to the grains
            float direction = GetDirection();
            // Set the direction parameter on the animator to control the enemy's animation
            _animator.SetFloat("direction", direction);
        }
    }

    public void FindClosestTower()
    {
        // Find all colliders within the detection radius
        float distanceToClosestBuilding = Mathf.Infinity;
        CropsCheck closestTower = null;
        CropsCheck[] allTowers = GameObject.FindObjectsOfType<CropsCheck>();

        // Iterate through the colliders to find the closest enemy with an EnemyHealth component
        foreach (CropsCheck currentTower in allTowers)
        {
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude;
            if (distanceToTower < distanceToClosestBuilding)
            {
                distanceToClosestBuilding = distanceToTower;
                closestTower = currentTower;
            }
            _grains = closestTower;
        }
    }

    // Move the enemy towards the grains
    private void MoveToGrains()
    {
        if (_grains != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _grains.transform.position, _Speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _townHall.transform.position, _Speed * Time.deltaTime);
        }
    }

    // Calculate the direction from the enemy to the grains
    private float GetDirection()
    {
        Vector2 direction;

        if (_grains != null)
        {
            // Calculate the vector from the enemy to the closest tower
            direction = (_grains.transform.position - transform.position).normalized;
        }
        else
        {
            // Calculate the vector from the enemy to the grains
            direction = (_townHall.transform.position - transform.position).normalized;
        }
        // Calculate the vector from the enemy to the grains
        // Convert the vector to an angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (died)
        { return;
        }
        if (collision.CompareTag("Crops"))
        {
            Instantiate(prefabFeatherParticle, transform.position, Quaternion.identity);
            _spawner.EnemyDestroyed();
            died = true;
            collision.GetComponent<CropsHealth>().ChangeHealth(-_EnemyDamage);
            Destroy(gameObject);
        }
        if (collision.CompareTag("TownHall"))
        {
            Instantiate(prefabFeatherParticle, transform.position, Quaternion.identity);
            _spawner.EnemyDestroyed();
            died = true;
            collision.GetComponent<TownHallHealth>().ChangeHealth(-_EnemyDamage);
            Destroy(gameObject);
        }

    }
}