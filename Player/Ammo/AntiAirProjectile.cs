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
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.SetShooter(_Shooter); // Set the shooter reference on the enemy
                enemyHealth.ChangeHealth(-_ProjectileDamage);
            }
            Destroy(gameObject);
        }
    }
}
