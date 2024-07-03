using UnityEngine;

public class BunnyHealth : EnemyHealth
{
    private Bunny bunny;

    [SerializeField] private GameObject prefabBloodEffect;

    public override void Start()
    {
        base.Start();
        bunny = GetComponent<Bunny>();
    }
    protected override void Kill()
    {
        Instantiate(prefabBloodEffect, transform.position, Quaternion.identity);
        base.Kill();
        GiveGold(bunny.gainGold);
        GiveXP(bunny.gainXP);
    }
}
