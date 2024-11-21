using UnityEngine;

// BearHealth class inherits from EnemyHealth, which handles health functionality for enemies.
public class BearHealth : EnemyHealth
{
    // Private variable to hold a reference to the Tanky component attached to this GameObject.
    private Tanky _tanky;

    // Assignment of a blood effect prefab in the Unity Inspector.
    [SerializeField] private GameObject _PrefabBloodEffect;

    // Override the Start method from the base class to initialize the BearHealth.
    public override void Start()
    {
        // Call the base class's Start method to ensure any initialization in EnemyHealth is executed.
        base.Start();
        
        // Get the Tanky component attached to the same GameObject and store it in the _tanky variable.
        _tanky = GetComponent<Tanky>();
    }

    // Override the Kill method from the base class to provide custom behavior when the bear is killed.
    protected override void Kill()
    {
        // Instantiate the blood effect prefab at the bear's current position with no rotation.
        Instantiate(_PrefabBloodEffect, transform.position, Quaternion.identity);
        
        // Call the base class's Kill method to ensure any common kill logic is executed.
        base.Kill();
        
        // Call the GiveGold method to reward the player with gold based on the Tanky's gainGold value.
        GiveGold(_tanky.gainGold);
        
        // Call the GiveXP method to reward the player with experience points based on the Tanky's gainXP value.
        GiveXP(_tanky.gainXP);
    }
}