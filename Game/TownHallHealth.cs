using Michsky.MUIP;
using System.Collections;
using UnityEngine;

public class TownHallHealth : Health
{
    private PauseHandler _pauseHandler;
    [SerializeField] private ProgressBar _HealthBar;
    [SerializeField] private SpriteRenderer _spriteRenderer1;
    [SerializeField] private SpriteRenderer _spriteRenderer2;

    public override void Start()
    {
        base.Start();
        _pauseHandler = FindObjectOfType<PauseHandler>();
    }

    public override void ChangeHealth(float amount)
    {
        base.ChangeHealth(amount);
        _HealthBar.ChangeValue(base._CurrentHealth);
    }

    public override void ChangeMaxHealth(float amount)
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

    protected override IEnumerator FlashSprite(float flashTime)
    {
        for (int i = 0; i < _FlashColors.Length; i++)
        {
            _spriteRenderer1.material.color = _FlashColors[i];
            _spriteRenderer2.material.color = _FlashColors[i];
            yield return new WaitForSeconds(flashTime);
        }
    }
}
