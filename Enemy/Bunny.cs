using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bunny : MonoBehaviour
{
    #region variables
    [SerializeField] public float _Speed; // movement speed of the enemy
    [SerializeField] private int _EnemyDamage; // damage dealt to the grains when the enemy collides with them
    public int gainGold;
    public int gainXP;

    [HideInInspector] public GameObject _townHall; // the grains game object that the enemy is moving towards
    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;
    private TowerCheck _target;
    private bool died;

    [SerializeField] private GameObject prefabExplosion;
    private AudioManager _audioManager;

    #endregion

    private void Start()
    {
        _townHall = GameObject.FindGameObjectWithTag("TownHall");
        _animator = GetComponent<Animator>();
        _spawner = FindAnyObjectByType<Spawner>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Update function to find the closest tower and move towards it
    private void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            FindClosestTower();
            MoveToTarget();

            // Set the animator's direction based on the enemy's movement direction
            float direction = GetDirection();
            _animator.SetFloat("direction", direction);
        }
    }

    // Function to find the closest tower to the enemy
    public void FindClosestTower()
    {
        // Find all colliders within the detection radius
        float distanceToClosestBuilding = Mathf.Infinity;
        TowerCheck closestTower = null;
        TowerCheck[] allTowers = GameObject.FindObjectsOfType<TowerCheck>();

        // Iterate through the colliders to find the closest enemy with an EnemyHealth component
        foreach (TowerCheck currentTower in allTowers)
        {
            float distanceToTower = (currentTower.transform.position - transform.position).sqrMagnitude;
            if (distanceToTower < distanceToClosestBuilding)
            {
                distanceToClosestBuilding = distanceToTower;
                closestTower = currentTower;
            }
            _target = closestTower;
        }
    }

    // Function to move the enemy towards the closest tower or grains
    private void MoveToTarget()
    {
        // Move towards the closest tower if it's within range
        if (_target != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _target.transform.position, _Speed * Time.deltaTime);
        }
    }

    // Function to calculate the enemy's movement direction
    private float GetDirection()
    {
        Vector2 direction;

        // Check if the enemy is targeting a tower
        if (_target != null)
        {
            // Calculate the vector from the enemy to the closest tower
            direction = (_target.transform.position - transform.position).normalized;
        }
        else
        {
            // Calculate the vector from the enemy to the grains
            direction = (_townHall.transform.position - transform.position).normalized;
        }

        // Convert the vector to an angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (died)
        {
            return;
        }
        if (collision.CompareTag("Tower"))
        {
            
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Deal damage to the Tower
            collision.GetComponent<TowerHealth>().ChangeHealth(-_EnemyDamage);
            died = true;
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }

        if (collision.CompareTag("Crops"))
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Deal damage to the TownHall
            collision.GetComponent<CropsHealth>().ChangeHealth(-_EnemyDamage);
            died = true;
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }

        if (collision.CompareTag("TownHall"))
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            _audioManager.PlaySFX(_audioManager.explosionSFX, 1);

            // Deal damage to the TownHall
            collision.GetComponent<TownHallHealth>().ChangeHealth(-_EnemyDamage);
            died = true;
            _spawner.EnemyDestroyed();
            Destroy(gameObject);
        }
    }
}