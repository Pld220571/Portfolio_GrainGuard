using UnityEngine;

// BugHealth class inherits from EnemyHealth, which likely contains shared health functionality for enemies.
public class BugHealth : EnemyHealth
{
    // Private variable to hold a reference to the Tanky component attached to this GameObject.
    private Tanky tanky;

    // Serialized field to allow assignment of a bug splash effect prefab in the Unity Inspector.
    [SerializeField] private GameObject prefabBugSplash;

    // Override the Start method from the base class to perform initialization specific to BugHealth.
    public override void Start()
    {
        // Call the base class's Start method to ensure any initialization in EnemyHealth is executed.
        base.Start();

        // Get the Tanky component attached to the same GameObject and store it in the tanky variable.
        tanky = GetComponent<Tanky>();
    }

    // Override the Kill method from the base class to provide custom behavior when the bug is killed.
    protected override void Kill()
    {
        // Instantiate the bug splash effect prefab at the bug's current position with no rotation.
        Instantiate(prefabBugSplash, transform.position, Quaternion.identity);

        // Call the base class's Kill method to ensure any common kill logic is executed.
        base.Kill();

        // Call the GiveGold method to reward the player with gold based on the Tanky's gainGold value.
        GiveGold(tanky.gainGold);

        // Call the GiveXP method to reward the player with experience points based on the Tanky's gainXP value.
        GiveXP(tanky.gainXP);
    }
}