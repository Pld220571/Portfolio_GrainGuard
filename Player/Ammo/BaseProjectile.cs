using UnityEngine;

public class BaseProjectile : Projectile
{
    public override void Start()
    {
        base.Start();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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