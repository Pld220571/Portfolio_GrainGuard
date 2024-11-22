using UnityEngine;

public class BugHealth : EnemyHealth
{
    [SerializeField] private GameObject _PrefabBugSplash;

    private Tanky _tanky;

    public override void Start()
    {
        base.Start();
        _tanky = GetComponent<Tanky>();
    }

    protected override void Kill()
    {
        Instantiate(_PrefabBugSplash, transform.position, Quaternion.identity); // Create a splash effect at the bug's position.
        base.Kill();
        GiveGold(_tanky.gainGold); // Reward gold based on the Tanky's gainGold property.
        GiveXP(_tanky.gainXP);     // Reward XP based on the Tanky's gainXP property.
    }
}