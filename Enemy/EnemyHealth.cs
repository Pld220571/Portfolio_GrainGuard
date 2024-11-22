using UnityEngine;

public class EnemyHealth : Health
{
    public enum DeathSoundType
    {
        Bunny,
        Crow,
        Bear,
        Gooper
    }

    private Spawner _spawner;
    private Punten _points;

    // Reference to the shooter, which could be a player or another entity that dealt damage
    private MonoBehaviour _shooter;

    [Header("Audio")]
    [SerializeField] private DeathSoundType _DeathSoundType;

    private AudioManager _audioManager;

    private void Awake()
    {
        base.Start();
        _spawner = FindObjectOfType<Spawner>();
        _points = FindObjectOfType<Punten>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Method to set the shooter reference, allowing the enemy to know who caused its death
    public void SetShooter(MonoBehaviour shooter)
    {
        _shooter = shooter;
    }

    protected override void CheckHealth()
    {
        base.CheckHealth();
    }

    protected override void Kill()
    {
        if (!died)
        {
            // Notify the spawner that an enemy has been destroyed
            _spawner.EnemyDestroyed();
            base.Kill();

            switch (_DeathSoundType)
            {
                case DeathSoundType.Bunny:
                    _audioManager.PlaySFX(_audioManager.enemyDeathSFX, 1f);
                    break;
                case DeathSoundType.Crow:
                    _audioManager.PlaySFX(_audioManager.CrowDeathSFX, 1f);
                    break;
                case DeathSoundType.Bear:
                    _audioManager.PlaySFX(_audioManager.beardeathSFX, 1f);
                    break;
                case DeathSoundType.Gooper:
                    _audioManager.PlaySFX(_audioManager.bugdeathSFX, 1f);
                    break;
            }
        }
    }

    // Method to give experience points to the shooter if they are valid
    protected void GiveXP(int amount)
    {
        if (_shooter != null)
        {
            // Attempt to get the BuildingXP component from the shooter
            BuildingXP buildingXP = _shooter.GetComponent<BuildingXP>();

            if (buildingXP != null)
            {
                // If the shooter has a BuildingXP component, grant the experience points
                buildingXP.GainXP(amount);
            }
        }
    }

    // Method to give gold to the player or relevant entity
    protected virtual void GiveGold(int amount)
    {
        // Increases the gold amount in the Punten component
        _points.GainGold(amount);
    }

    public void IncreaseHealth(int amount)
    {
        // Adjusts the maximum health of the enemy
        ChangeMaxHealth(amount);

        // Also restores the current health by the same amount
        ChangeHealth(amount);
    }
}