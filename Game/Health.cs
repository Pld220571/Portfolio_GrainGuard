using UnityEngine;
using System.Collections;

// Defining an abstract class Health that inherits from MonoBehaviour
public abstract class Health : MonoBehaviour
{
    [SerializeField] public float _MaxHealth;
    [SerializeField] public float _CurrentHealth;
    public bool died;

    // Serialized array of colors for flashing the sprite
    [SerializeField] protected Color[] _FlashColors;

    private SpriteRenderer _spriteRenderer;


    public virtual void Start()
    {
        _CurrentHealth = _MaxHealth;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // ChangeHealth method that changes the current health by a given amount and checks if the game object should be killed or hit
    public virtual void ChangeHealth(float amount)
    {
        _CurrentHealth = _CurrentHealth + amount;
        CheckHealth();
    }

    // ChangeMaxHealth method that changes the max health and current health by a given amount
    public virtual void ChangeMaxHealth(float amount)
    {
        _MaxHealth = _MaxHealth + amount;
        CheckHealth();
    }

    // CheckHealth method that checks if the current health is less than or equal to zero. If it is, the game object is killed. Otherwise, it is hit.
    protected virtual void CheckHealth()
    {
        if (_CurrentHealth <= 0)
        {
            _CurrentHealth = 0;
            Kill();
        }
        else
        {
            Hit();
        }
    }

    protected virtual void Kill()
    {
        if (died)
        {
            return;
        }
        died = true;
        Destroy(gameObject);
    }

    // Hit method that starts a coroutine to flash the sprite
    protected virtual void Hit()
    {
        StartCoroutine(FlashSprite(0.1f));
    }

    // FlashSprite coroutine that flashes the sprite by changing its color to each color in the _FlashColors array
    protected virtual IEnumerator FlashSprite(float flashTime)
    {
        for (int i = 0; i < _FlashColors.Length; i++)
        {
            _spriteRenderer.material.color = _FlashColors[i];
            yield return new WaitForSeconds(flashTime);
        }
    }
}
