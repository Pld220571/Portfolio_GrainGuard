using System.Collections;
using UnityEngine;

public class Tomato : Crops
{
    [SerializeField] private int _EvolutionsAmount; // Number of times the tomato can evolve
    [SerializeField] private int _GoldIncrease; // Amount of gold increase per evolution
    [SerializeField] private float _UpgradeCountdown; // Time interval between evolutions
    [SerializeField] private GameObject[] _evolutionSprites; // Array of sprites for each evolution stage

    private int _currentEvolution; // Tracks the current evolution stage

    private new void Start()
    {
        base.Start();
        StartCoroutine(StartEvolution());
    }

    IEnumerator StartEvolution() // Coroutine to manage the evolution process
    {
        for (int i = 0; i < _EvolutionsAmount; i++) // Loop through the number of evolutions
        {
            yield return new WaitForSeconds(_UpgradeCountdown);
            Evolve(); // Call the Evolve method to evolve the tomato
        }
    }

    private void Evolve() // Method to handle the evolution of the tomato
    {
        if (_currentEvolution < _evolutionSprites.Length - 1) // Check if there are more evolutions available
        {
            Destroy(gameObject.transform.GetChild(0).gameObject); // Destroy the current evolution sprite
            GameObject nextEvolution = Instantiate(_evolutionSprites[++_currentEvolution], transform.position, transform.rotation); // Create the next evolution sprite
            nextEvolution.transform.parent = transform; // Set the parent of the new sprite to the tomato
            nextEvolution.transform.localPosition = new Vector3(0, 0.82f, 0); // Position the new sprite slightly above the current position
            _Gold = _Gold + _GoldIncrease; // Increase the gold value after evolution
        }
    }
}