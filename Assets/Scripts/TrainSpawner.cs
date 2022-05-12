using System;
using System.Collections.Generic;
using UnityEngine;

/// A GameObject that spawns fucking trains
public class TrainSpawner : MonoBehaviour
{
    [SerializeField] private GameObject trainPrefab;
    [SerializeField] private float trainInterval = 120f;
    [SerializeField] private float maxTravelBeforeDestroy = 100;

    private GameObject currentTrain;
    private float lastTrainDestruction;

    public GameObject CurrentTrain => currentTrain;

    private void Update()
    {
        if (!currentTrain)
        {
            SpawnTrain();
        }

        if (Vector3.Distance(currentTrain.transform.position, transform.position) > maxTravelBeforeDestroy)
            DestroyTrain();

        if (Time.time - lastTrainDestruction > trainInterval) DestroyTrain();
    }

    public void SpawnTrain()
    {
        currentTrain = Instantiate(trainPrefab, this.transform, false);
    }

    public void SpawnStoppingTrain(float stopPosition, float stopDuration)
    {
        if (currentTrain)
        {
            Debug.LogWarning("Trying to spawn a stopping train, but a train already exists.");
            DestroyTrain();
        }
        SpawnTrain();
        currentTrain.GetComponent<Train>().Stop(stopPosition, stopDuration);
    }

    private void DestroyTrain()
    {
        lastTrainDestruction = Time.time;
        Destroy(currentTrain);
        currentTrain = null;
    }
}
