using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _ProjectileDamage;
    [SerializeField] protected float _TimeToDestroy;

    protected MonoBehaviour _shooter;
    private AudioManager _audioManager;
    private TownhallUpgrade _townhallUpgrade;

    public virtual void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        ApplyDamageMultiplier();
        Destroy(gameObject, _TimeToDestroy);
    }

    protected void ApplyDamageMultiplier() // Method to apply damage multipliers based on upgrades
    {
        _ProjectileDamage = _ProjectileDamage * _townhallUpgrade.dmgMultiplier;
    }

    public virtual void DmgSound()
    {
        _audioManager.PlaySFX(_audioManager.enemyDeathSFX, 1);
    }

    public void SetShooter(MonoBehaviour shooter) // Method to set the shooter of the projectile
    {
        _shooter = shooter; // Assign the shooter reference
    }
}