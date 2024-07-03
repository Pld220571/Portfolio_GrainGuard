using System.Collections;
using UnityEngine;

public class Tomato : Crops
{
    // The total number of evolution stages the tomato plant will go through
    [SerializeField] private int _EvolutionsAmount;

    // The amount of gold earned each time the tomato plant evolves
    [SerializeField] private int _GoldIncrease;

    // The time (in seconds) between each evolution stage
    [SerializeField] private float _UpgradeCountdown;

    // An array of prefabs, each representing a different evolution stage of the tomato plant
    [SerializeField] private GameObject[] _evolutionSprites;

    // The current evolution stage of the tomato plant (initialized to 0)
    private int _currentEvolution;

    // Called when the script is initialized
    private new void Start()
    {
        // Call the Start() method of the base class (Crops)
        base.Start();

        // Start the evolution coroutine
        StartCoroutine(StartEvolution());
    }

    // Coroutine responsible for evolving the tomato plant over time
    IEnumerator StartEvolution()
    {
        // Loop _EvolutionsAmount times
        for (int i = 0; i < _EvolutionsAmount; i++)
        {
            // Wait for _UpgradeCountdown seconds
            yield return new WaitForSeconds(_UpgradeCountdown);

            // Evolve the tomato plant to the next stage
            Evolve();
        }
    }

    //evolve the current tomato to the next stage
    private void Evolve()
    {
        // Check if the current evolution is not the last one in the array
        if (_currentEvolution < _evolutionSprites.Length - 1)
        {
            // Destroy the current game object's child (i.e., the current evolution)
            Destroy(gameObject.transform.GetChild(0).gameObject);

            // Instantiate the next evolution at the current position and rotation
            GameObject nextEvolution = Instantiate(_evolutionSprites[++_currentEvolution], transform.position, transform.rotation);

            // Set the parent of the next evolution to the current game object
            nextEvolution.transform.parent = transform;

            // Set the local Y position of the new child to 0.82f
            nextEvolution.transform.localPosition = new Vector3(0, 0.82f, 0);

            // Increase the gold by the gold increase amount
            _Gold = _Gold + _GoldIncrease;
        }
    }
}