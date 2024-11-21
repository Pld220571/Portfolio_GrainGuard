using UnityEngine;

// BunnyHealth class inherits from EnemyHealth, which likely provides shared health functionality for enemies.
public class BunnyHealth : EnemyHealth
{
    // Private variable to hold a reference to the Bunny component attached to this GameObject.
    private Bunny bunny;

    // Serialized field to allow assignment of a blood effect prefab in the Unity Inspector.
    [SerializeField] private GameObject prefabBloodEffect;

    // Override the Start method from the base class to perform initialization specific to BunnyHealth.
    public override void Start()
    {
        // Call the base class's Start method to ensure any initialization in EnemyHealth is executed.
        base.Start();
        
        // Get the Bunny component attached to the same GameObject and store it in the bunny variable.
        bunny = GetComponent<Bunny>();
    }

    // Override the Kill method from the base class to provide custom behavior when the bunny is killed.
    protected override void Kill()
    {
        // Instantiate the blood effect prefab at the bunny's current position with no rotation.
        Instantiate(prefabBloodEffect, transform.position, Quaternion.identity);
        
        // Call the base class's Kill method to ensure any common kill logic is executed.
        base.Kill();
        
        // Call the GiveGold method to reward the player with gold based on the Bunny's gainGold value.
        GiveGold(bunny.gainGold);
        
        // Call the GiveXP method to reward the player with experience points based on the Bunny's gainXP value.
        GiveXP(bunny.gainXP);
    }
}