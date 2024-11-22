using UnityEngine;

public class CrowHealth : EnemyHealth
{
    [SerializeField] private GameObject _PrefabFeatherParticle;

    private Crow _crow;

    public override void Start()
    {
        base.Start();
        _crow = GetComponent<Crow>();
    }

    protected override void Kill()
    {
        // Create a feather particle effect at the Crow's position upon death
        Instantiate(_PrefabFeatherParticle, transform.position, Quaternion.identity);
        base.Kill();

        // Give gold to the player
        GiveGold(_crow.GainGold);

        // Give experience points to the player
        GiveXP(_crow.GainXP);
    }
}