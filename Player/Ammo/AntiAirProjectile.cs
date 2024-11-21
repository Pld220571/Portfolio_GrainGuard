using UnityEngine;

public class AntiAirProjectile : Projectile
{
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Air Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>(); // Attempt to get the EnemyHealth component from the collided object

            if (enemyHealth != null) // If the EnemyHealth component was found
            {
                enemyHealth.SetShooter(_Shooter); // Set the shooter reference in the enemy's health component
                enemyHealth.ChangeHealth(-_ProjectileDamage); // Deal damage to the enemy by changing its health
            }

            Destroy(gameObject);
        }
    }
}