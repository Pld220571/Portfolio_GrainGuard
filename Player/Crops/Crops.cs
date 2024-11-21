using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Crops : MonoBehaviour
{
    [HideInInspector] public Punten Points;

    [SerializeField] protected int _Gold; // Amount of gold generated per farming cycle
    [SerializeField] private float _GoldCooldown; // Time interval between gold generation
    [SerializeField] private GameObject _CoinAnimation;
    [SerializeField] private float _CoinAnimationYPosition;

    private PauseHandler _pauseHandler;
    private bool _isFarming;

    protected void Start()
    {
        Points = FindObjectOfType<Punten>();
        _pauseHandler = FindAnyObjectByType<PauseHandler>();
        StartCoroutine(StartFarming());
    }

    protected void GiveGold(int amount) // Method to give gold to the Punten instance
    {
        Points.GainGold(amount); // Call the GainGold method to add gold
    }

    protected IEnumerator StartFarming()
    {
        while (!_pauseHandler.gameOver)
        {
            yield return new WaitForSeconds(_GoldCooldown);
            GiveGold(_Gold); // Generate gold
            Vector3 CoinAnimationPosition = transform.position + new Vector3(0, _CoinAnimationYPosition, 0); // Calculate the position for the coin animation
            Instantiate(_CoinAnimation, CoinAnimationPosition, Quaternion.identity);

            if (_CoinAnimation == null)
            {
                Debug.Log("There is no animation assigned.");
            }
        }
    }
}