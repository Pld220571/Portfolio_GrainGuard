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
    [HideInInspector] public GameObject _townHall; // The game object that the bear is moving towards if no towers are in range.
    private Animator _animator;
    private Spawner _spawner; // The spawner game object that spawned the bear.
    private PauseHandler _pauseHandler; // The pause handler game object that controls the game's pause state.
    public int gainGold; // The amount of gold earned when this enemy is destroyed.
    public int gainXP; // The amount of XP earned when this enemy is destroyed.

    [Header("Tower Detection")]
    [SerializeField] private float _MaxDistance; // The maximum distance within which the bear can detect towers.

    [Header("Movement")]
    [SerializeField] private float _MovementSpeed;
    private Transform _currentTarget;

    [Header("Attack")]
    [SerializeField] private float _StoppingDistance; // The distance at which the bear stops moving and starts attacking.
    [SerializeField] private float _AdditionalStoppingDistance;
    [SerializeField] private float _EnemyDamage; // The damage dealt to the target when attacked.
    [SerializeField] private float _AnimationSpeed;
    [SerializeField] private TankyAttackType _TankyAttackType;
    private AudioManager _audioManager;
    #endregion

    private void Start()
    {
        _townHall = GameObject.FindGameObjectWithTag("TownHall");
        _animator = GetComponent<Animator>();
        _animator.SetFloat("animationSpeed", _AnimationSpeed); // Sets the animation speed.
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

            // Set the animator's direction based on the enemy's movement direction
            float direction = GetDirection();
            _animator.SetFloat("direction", direction);
        }

        //Debug.Log(_currentTarget.name);
    }

    private void FindClosestTower()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _MaxDistance); // Uses a physics overlap circle to find all colliders within the detection range.
        TowerCheck closestTower = null;
        float closestDistance = Mathf.Infinity;

        // Loops through the colliders and finds the closest tower with a TowerCheck component.
        foreach (var collider in hitColliders)
        {
            TowerCheck towerCheck = collider.GetComponentInChildren<TowerCheck>();
            if (towerCheck != null)
            {
                float distanceToTower = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToTower < closestDistance)
                {
                    closestDistance = distanceToTower;
                    closestTower = towerCheck;
                }
            }
        }
        if (closestTower != null)
        {
            // Updates the _currentTarget variable with the closest tower.
            _currentTarget = closestTower.transform;
        }
    }

    void MoveToTarget()
    {
        if (_currentTarget == null)
        {
            _currentTarget = _townHall.transform;
        }

        float currentDistanceToTarget = Vector3.Distance(_currentTarget.GetComponentInParent<Collider2D>().bounds.center, transform.position); // Calculate the distance to the TownHall.
        currentDistanceToTarget -= _AdditionalStoppingDistance;
        Debug.Log(currentDistanceToTarget);

        if (currentDistanceToTarget >= _StoppingDistance)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, _currentTarget.position, _MovementSpeed * Time.deltaTime);
            _animator.SetBool("canAttack", false);
        }
        else
        {
            // If within stopping distance, it starts attacking the TownHall
            _animator.SetBool("canAttack", true);
        }
    }

    public void HitAnimationEvent()
    {
        if (!_pauseHandler.gameOver)
        {
            if (_currentTarget != null)
            {
                Health health = _currentTarget.GetComponentInChildren<Health>();
                if (health == null)
                {
                    health = _currentTarget.GetComponentInParent<Health>();
                }
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

        //// Play the corresponding attack sound effect based on the enemy type
        //if (enemyType == "Bear")
        //{
        //    _audioManager.PlaySFX(_audioManager.BearChopSFX, 1f);
        //}
        //else if (enemyType == "Gooper")
        //{
        //    _audioManager.PlaySFX(_audioManager.GooperAttackSFX, 1f);
        //}
    }

    private float GetDirection() // Calculates the bear's movement direction
    {
        Vector2 direction;

        // Check if the enemy is targeting a tower
        if (_currentTarget != null)
        {
            // If a target is set, it calculates the vector from the bear to the target.
            direction = (_currentTarget.transform.position - transform.position).normalized;
        }
        else
        {
            // If no target is set, it calculates the vector from the bear to the town hall.
            direction = (_townHall.transform.position - transform.position).normalized;
        }

        // Converts the vector to an angle in degrees and returns it.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }
    //#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw detection range gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _MaxDistance);

        // Draw stopping distance gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _StoppingDistance);
    }
//}
//#endif
}