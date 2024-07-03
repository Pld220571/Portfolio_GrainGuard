using UnityEngine;
public class CrowHealth : EnemyHealth
{
    private Crow crow;

    [SerializeField] private GameObject prefabFeatherParticle;

    public override void Start()
    {
        base.Start();
        crow = GetComponent<Crow>();
    }
    protected override void Kill()
    {
        Instantiate(prefabFeatherParticle, transform.position, Quaternion.identity);
        base.Kill();
        GiveGold(crow.gainGold);
        GiveXP(crow.gainXP);
    }
}
