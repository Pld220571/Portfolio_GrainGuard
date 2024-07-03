using System.Collections;
using UnityEngine;

public class Crops : MonoBehaviour
{
    [HideInInspector] public Punten Punten;
    [SerializeField] protected int _Gold;
    [SerializeField] private float _GoldCooldown;
    [SerializeField] private GameObject _CoinAnimation;
    [SerializeField] private float _CoinAnimationYPosition;
    private PauseHandler _pauseHandler;
    private bool _isFarming;


    protected void Start()
    {
        Punten = FindObjectOfType<Punten>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
        StartCoroutine(StartFarming());
    }

    protected void GiveGold(int amount)
    {
        Punten.GainGold(amount);
    }

    protected IEnumerator StartFarming()
    {
        while (!_pauseHandler.gameOver)
        {
            yield return new WaitForSeconds(_GoldCooldown);
            GiveGold(_Gold);
            Vector3 CoinAnimationPosition = transform.position + new Vector3(0, _CoinAnimationYPosition, 0);
            Instantiate(_CoinAnimation, CoinAnimationPosition, Quaternion.identity);

            if (_CoinAnimation == null)
            {
                Debug.Log("There is no animation assigned.");
            }
        }
    }
}