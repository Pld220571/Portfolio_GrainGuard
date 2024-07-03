using UnityEngine;

public class BearHealth : EnemyHealth
{
    private Tanky _tanky;

    [SerializeField] private GameObject prefabBloodEffect;

    public override void Start()
    {
        base.Start();
        _tanky = GetComponent<Tanky>();
    }
    protected override void Kill()
    {
        Instantiate(prefabBloodEffect, transform.position, Quaternion.identity);
        base.Kill();
        GiveGold(_tanky.gainGold);
        GiveXP(_tanky.gainXP);
    }
}
