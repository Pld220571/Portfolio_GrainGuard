using Michsky.MUIP;
using System.Collections;
using UnityEngine;

public class TownHallHealth : Health
{
    [SerializeField] private ProgressBar _HealthBar;
    [SerializeField] private SpriteRenderer _SpriteRenderer1;
    [SerializeField] private SpriteRenderer _SpriteRenderer2;

    private PauseHandler _pauseHandler;

    public override void Start()
    {
        base.Start();
        _pauseHandler = FindObjectOfType<PauseHandler>();
    }

    public override void ChangeHealth(float amount) // Update health and the health bar
    {
        base.ChangeHealth(amount);
        _HealthBar.ChangeValue(base._CurrentHealth); // Update the health bar value based on current health
    }

    public override void ChangeMaxHealth(float amount) // Update the maximum health if needed
    {
        base.ChangeMaxHealth(amount);
    }

    protected override void CheckHealth()
    {
        base.CheckHealth();
    }

    protected override void Kill()
    {
        base.Kill();
        _pauseHandler.GameLost();
    }

    protected override void Hit()
    {
        base.Hit();
    }
    
    protected override IEnumerator FlashSprite(float flashTime) // Override the FlashSprite method to visually indicate damage
    {
        for (int i = 0; i < _FlashColors.Length; i++) // Loop through the flash colors and apply them to the sprite renderers
        {
            _SpriteRenderer1.material.color = _FlashColors[i];
            _SpriteRenderer2.material.color = _FlashColors[i];
            yield return new WaitForSeconds(flashTime);
        }
    }
}