using UnityEngine;
using System.Collections;

public abstract class Health : MonoBehaviour
{
    public float MaxHealth;
    public float CurrentHealth;
    public bool IsDead;

    [SerializeField] protected Color[] _FlashColors; // Array of colors used for flashing effect on hit

    private SpriteRenderer _spriteRenderer;

    public virtual void Start()
    {
        CurrentHealth = MaxHealth;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void ChangeHealth(float amount) // Method to change the current health by a specified amount
    {
        CurrentHealth = CurrentHealth + amount; // Update current health
        CheckHealth(); // Check if health is below zero after the change
    }

    public virtual void ChangeMaxHealth(float amount) // Method to change the maximum health of the entity
    {
        MaxHealth = MaxHealth + amount; // Update maximum health
        CheckHealth(); // Check health status after changing max health
    }

    protected virtual void CheckHealth()
    {
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Kill();
        }
        else
        {
            Hit();
        }
    }

    protected virtual void Kill()
    {
        if (IsDead)
        {
            return;
        }
        IsDead = true;
        Destroy(gameObject);
    }

    protected virtual void Hit()
    {
        StartCoroutine(FlashSprite(0.1f)); // Start the coroutine to flash the sprite
    }

    protected virtual IEnumerator FlashSprite(float flashTime) // Coroutine to flash the sprite colors
    {
        for (int i = 0; i < _FlashColors.Length; i++) // Loop through each color in the flash colors array
        {
            _spriteRenderer.material.color = _FlashColors[i]; // Change the sprite color to the current flash color
            yield return new WaitForSeconds(flashTime); // Wait for the specified flash time before changing to the next color
        }
    }
}