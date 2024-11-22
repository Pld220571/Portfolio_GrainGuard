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

    // Amount of gold gained
    public int GainGold;

    // Amount of experience gained
    public int GainXP;

    private Animator _animator;
    private Spawner _spawner;
    private PauseHandler _pauseHandler;

    [Header("Tower Detection")]
    [SerializeField] private float _MaxDistance;

    [Header("Movement")]
    [SerializeField] private float _MovementSpeed;
    private Transform _currentTarget;

    [Header("Attack")]
    // Distance at which the Tanky stops moving towards the target
    [SerializeField] private float _StoppingDistance;

    // Additional distance to consider for stopping
    [SerializeField] private float _AdditionalStoppingDistance;
    [SerializeField] private float _EnemyDamage;
    [SerializeField] private float _AnimationSpeed;
    [SerializeField] private TankyAttackType _TankyAttackType;
    private AudioManager _audioManager;
    #endregion

    private void Start()
    {
        TownHall = GameObject.FindGameObjectWithTag("TownHall");
        _animator = GetComponent<Animator>();

        // Set the animator's speed parameter based on _AnimationSpeed
        _animator.SetFloat("animationSpeed", _AnimationSpeed);
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

            // Get the direction towards the target
            float direction = GetDirection();

            // Set the animator's direction parameter based on the calculated direction
            _animator.SetFloat("direction", direction);
        }
    }

    private void FindClosestTower()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MaxDistance);

        // Variable to hold the closest tower found
        TowerCheck closestTower = null;

        // Initialize closest distance to infinity
        float closestDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            TowerCheck towerCheck = collider.GetComponentInChildren<TowerCheck>();

            if (towerCheck != null)
            {
                float distanceToTower = Vector2.Distance(transform.position, collider.transform.position);

                // If this tower is closer than the previous closest
                if (distanceToTower < closestDistance)
                {
                    // Update closest distance
                    closestDistance = distanceToTower;

                    // Update closest tower
                    closestTower = towerCheck;
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
            
            // Set animator to not attack
            _animator.SetBool("canAttack", false);
        }
        else
        {
            // Set animator to attack
            _animator.SetBool("canAttack", true);
        }
    }

    public void HitAnimationEvent()
    {
        if (!_pauseHandler.gameOver)
        {
            if (_currentTarget != null)
            {
                // Get the Health component from the target
                Health health = _currentTarget.GetComponentInChildren<Health>();

                // If no Health component found in children
                if (health == null)
                {
                    // Try to get it from the parent
                    health = _currentTarget.GetComponentInParent<Health>();
                }

                // Apply damage to the target
                health.ChangeHealth(-_EnemyDamage);
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

    // Method to get the direction towards the target
    private float GetDirection()
    {
        // Variable to hold the direction vector
        Vector2 direction;

        if (_currentTarget != null)
        {
            direction = (_currentTarget.transform.position - transform.position).normalized;
        }
        else
        {
            direction = (TownHall.transform.position - transform.position).normalized;
        }

        // Calculate the angle in degrees from the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Return the calculated angle
        return angle;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MaxDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _StoppingDistance);
    }