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


    private Spawner spawner;
    private Punten punten;
    private MonoBehaviour _Shooter;

    [Header("Audio")]
    [SerializeField] private DeathSoundType _DeathSoundType;
    private AudioManager _audioManager;

    private void Awake()
    {
        base.Start();
        spawner = FindObjectOfType<Spawner>();
        punten = FindObjectOfType<Punten>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    public void SetShooter(MonoBehaviour shooter)
    {
        _Shooter = shooter;
    }

    protected override void CheckHealth()
    {
        base.CheckHealth();
    }

    protected override void Kill()
    {
        if (!died)
        {
            spawner.EnemyDestroyed();
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

    protected void GiveXP(int amount)
    {
        // Notify the shooter tower
        if (_Shooter != null)
        {
            BuildingXP buildingXP = _Shooter.GetComponent<BuildingXP>();
            if (buildingXP != null)
            {
                buildingXP.GainXP(amount); // Adjust XP value as needed
            }
        }
    }

    // Method to increase gold
    protected virtual void GiveGold(int amount)
    {
        punten.GainGold(amount);
    }

    // Method to increase health
    public void IncreaseHealth(int amount)
    {
        ChangeMaxHealth(amount);
        ChangeHealth(amount);
    }
}
