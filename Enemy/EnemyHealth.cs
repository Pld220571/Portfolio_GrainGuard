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
    private MonoBehaviour _shooter; // Reference to the shooter, which could be a player or another entity that dealt damage

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

    public void SetShooter(MonoBehaviour shooter) // Method to set the shooter reference, allowing the enemy to know who caused its death
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
            _spawner.EnemyDestroyed(); // Notify the spawner that an enemy has been destroyed
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

    protected void GiveXP(int amount) // Method to give experience points to the shooter if they are valid
    {   
        if (_shooter != null) // Check if there is a shooter assigned
        {
            BuildingXP buildingXP = _shooter.GetComponent<BuildingXP>(); // Attempt to get the BuildingXP component from the shooter

            if (buildingXP != null) // If the shooter has a BuildingXP component, grant the experience points
            {
                buildingXP.GainXP(amount);
            }
        }
    }

    protected virtual void GiveGold(int amount) // Method to give gold to the player or relevant entity
    {
        _points.GainGold(amount); // Increases the gold amount in the Punten component
    }

    public void IncreaseHealth(int amount)
    {
        ChangeMaxHealth(amount); // Adjusts the maximum health of the enemy
        ChangeHealth(amount); // Also restores the current health by the same amount
    }
}