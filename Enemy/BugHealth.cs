using UnityEngine;

public class BugHealth : EnemyHealth
{
    private Tanky tanky;

    [SerializeField] private GameObject prefabBugSplash;
    private AudioManager audioManager;

    public override void Start()
    {
        base.Start();
        tanky = GetComponent<Tanky>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    protected override void Kill()
    {
        Instantiate(prefabBugSplash, transform.position, Quaternion.identity);
        //audioManager.PlaySFX(audioManager.bugdeathSFX, 1);
        base.Kill();
        GiveGold(tanky.gainGold);
        GiveXP(tanky.gainXP);
    }
}
