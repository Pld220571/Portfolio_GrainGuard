using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _ProjectileDamage;
    [SerializeField] protected float _TimeToDestroy;

    protected MonoBehaviour _Shooter;
    private AudioManager audioManager;
    private TownhallUpgrade townhallUpgrade;

    public virtual void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        townhallUpgrade = FindObjectOfType<TownhallUpgrade>();
        ApplyDamageMultiplier();
        Destroy(gameObject, _TimeToDestroy);
    }

    protected void ApplyDamageMultiplier()
    {
        _ProjectileDamage = _ProjectileDamage * townhallUpgrade.dmgMultiplier;
    }

    public virtual void DmgSound()
    {
        audioManager.PlaySFX(audioManager.enemyDeathSFX, 1);
    }

    public void SetShooter(MonoBehaviour shooter)
    {
        _Shooter = shooter;
    }
}
