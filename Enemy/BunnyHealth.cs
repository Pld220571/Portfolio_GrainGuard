using UnityEngine;

public class BunnyHealth : EnemyHealth
{
    [SerializeField] private GameObject _PrefabBloodEffect;

    private Bunny _bunny;

    public override void Start()
    {
        base.Start();
        _bunny = GetComponent<Bunny>();
    }

    protected override void Kill()
    {
        // Create a blood effect at the Bunny's position upon death
        Instantiate(_PrefabBloodEffect, transform.position, Quaternion.identity);
        base.Kill();

        // Give gold to the player
        GiveGold(_bunny.GainGold);

        // Give experience points to the player
        GiveXP(_bunny.GainXP);
    }
}