using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tanky : MonoBehaviour
{
    public enum TankyAttackType
    {
        Bear,
        Gooper,
    }

    #region Variables
    [Header("Components and Objects")]
    [HideInInspector] public GameObject TownHall;
    public int GainGold; // Amount of gold gained
    public int GainXP; // Amount of experience gained

    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;

    [Header("Tower Detection")]
    [SerializeField] private float _MaxDistance;

    [Header("Movement")]
    [SerializeField] private float _MovementSpeed;
    private Transform _currentTarget;

    [Header("Attack")]
    [SerializeField] private float _StoppingDistance; // Distance at which the Tanky stops moving towards the target
    [SerializeField] private float _AdditionalStoppingDistance; // Additional distance to consider for stopping
    [SerializeField] private float _EnemyDamage;
    [SerializeField] private float _AnimationSpeed;
    [SerializeField] private TankyAttackType _TankyAttackType;
    private AudioManager _audioManager;
    #endregion

    private void Start()
    {
        TownHall = GameObject.FindGameObjectWithTag("TownHall");
        _animator = GetComponent<Animator>();
        _animator.SetFloat("animationSpeed", _AnimationSpeed); // Set the animator's speed parameter based on _AnimationSpeed
        _spawner = FindObjectOfType<Spawner>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
        _audioManager = FindObjectOfType<AudioManager>();

        if (_TankyAttackType == TankyAttackType.Bear)
        {
            _audioManager.PlaySFX(_audioManager.bearintroductionSFX, 1f);
        }
    }

    void Update()
    {
        if (!_pauseHandler.gameOver)
        {
            FindClosestTower();
            MoveToTarget();
            float direction = GetDirection(); // Get the direction towards the target
            _animator.SetFloat("direction", direction); // Set the animator's direction parameter based on the calculated direction
        }
    }

    private void FindClosestTower()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MaxDistance);
        TowerCheck closestTower = null; // Variable to hold the closest tower found
        float closestDistance = Mathf.Infinity; // Initialize closest distance to infinity

        foreach (var collider in hitColliders)
        {
            TowerCheck towerCheck = collider.GetComponentInChildren<TowerCheck>();

            if (towerCheck != null)
            {
                float distanceToTower = Vector2.Distance(transform.position, collider.transform.position);

                if (distanceToTower < closestDistance) // If this tower is closer than the previous closest
                {
                    closestDistance = distanceToTower; // Update closest distance
                    closestTower = towerCheck; // Update closest tower
                }
            }
        }

        if (closestTower != null)
        {
            _currentTarget = closestTower.transform;
        }
    }

    void MoveToTarget()
    {
        if (_currentTarget == null)
        {
            _currentTarget = TownHall.transform;
        }

        float currentDistanceToTarget = Vector3.Distance(_currentTarget.GetComponentInParent<Collider2D>().bounds.center, transform.position);
        currentDistanceToTarget -= _AdditionalStoppingDistance;

        if (currentDistanceToTarget >= _StoppingDistance)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _currentTarget.position, _MovementSpeed * Time.deltaTime);
            _animator.SetBool("canAttack", false); // Set animator to not attack
        }
        else
        {
            _animator.SetBool("canAttack", true); // Set animator to attack
        }
    }

    public void HitAnimationEvent()
    {
        if (!_pauseHandler.gameOver)
        {
            if (_currentTarget != null)
            {
                Health health = _currentTarget.GetComponentInChildren<Health>(); // Get the Health component from the target

                if (health == null) // If no Health component found in children
                {
                    health = _currentTarget.GetComponentInParent<Health>(); // Try to get it from the parent
                }

                health.ChangeHealth(-_EnemyDamage); // Apply damage to the target
            }

            switch (_TankyAttackType)
            {
                case TankyAttackType.Bear:
                    _audioManager.PlaySFX(_audioManager.BearChopSFX, 1f);
                    break;
                case TankyAttackType.Gooper:
                    _audioManager.PlaySFX(_audioManager.GooperAttackSFX, 1f);
                    break;
            }
        }
    }

    private float GetDirection() // Method to get the direction towards the target
    {
        Vector2 direction; // Variable to hold the direction vector

        if (_currentTarget != null)
        {
            direction = (_currentTarget.transform.position - transform.position).normalized;
        }
        else
        {
            direction = (TownHall.transform.position - transform.position).normalized;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle in degrees from the direction vector
        return angle; // Return the calculated angle
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MaxDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _StoppingDistance);
    }
}