using UnityEngine;

public class BearHealth : EnemyHealth
{
    [SerializeField] private GameObject _PrefabBloodEffect;

    private Tanky _tanky;

    public override void Start()
    {
        base.Start();
        _tanky = GetComponent<Tanky>();
    }

    protected override void Kill()
    {
        Instantiate(_PrefabBloodEffect, transform.position, Quaternion.identity); // Create a blood effect at the bear's position.
        base.Kill();
        GiveGold(_tanky.gainGold); // Reward gold based on the Tanky's gainGold property.
        GiveXP(_tanky.gainXP);     // Reward XP based on the Tanky's gainXP property.
    }
}