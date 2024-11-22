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
        // Create a blood effect at the bear's position.
        Instantiate(_PrefabBloodEffect, transform.position, Quaternion.identity);
        base.Kill();

        // Reward gold based on the Tanky's gainGold property.
        GiveGold(_tanky.gainGold);

        // Reward XP based on the Tanky's gainXP property.
        GiveXP(_tanky.gainXP);
    }
}