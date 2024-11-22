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
        Instantiate(_PrefabFeatherParticle, transform.position, Quaternion.identity); // Create a feather particle effect at the Crow's position upon death
        base.Kill();
        GiveGold(_crow.GainGold); // Give gold to the player
        GiveXP(_crow.GainXP); // Give experience points to the player
    }
}