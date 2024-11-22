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
        // Create a splash effect at the bug's position
        Instantiate(_PrefabBugSplash, transform.position, Quaternion.identity);
        base.Kill();

        // Reward gold based on the Tanky's gainGold property
        GiveGold(_tanky.gainGold);

        // Reward XP based on the Tanky's gainXP property
        GiveXP(_tanky.gainXP);
    }
}