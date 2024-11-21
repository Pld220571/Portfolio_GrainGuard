using UnityEngine;

public class BunnyHealth : EnemyHealth
{
    private Bunny _bunny;
    [SerializeField] private GameObject _PrefabBloodEffect;

    public override void Start()
    {
        base.Start();
        _bunny = GetComponent<Bunny>();
    }

    protected override void Kill()
    {
        Instantiate(_PrefabBloodEffect, transform.position, Quaternion.identity); // Create a blood effect at the Bunny's position upon death
        base.Kill();
        GiveGold(_bunny.GainGold); // Give gold to the player
        GiveXP(_bunny.GainXP); // Give experience points to the player
    }
}