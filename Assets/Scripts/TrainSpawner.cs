using System;
using System.Collections.Generic;
using UnityEngine;

/// A GameObject that spawns fucking trains
public class TrainSpawner : MonoBehaviour
{
    [SerializeField] private GameObject trainPrefab;
    [SerializeField] private float trainInterval = 10f;
    [SerializeField] private float maxTravelBeforeDestroy = 100;

    private GameObject currentTrain;
    private float lastTrainDestruction;

    private void Update()
    {
        if (!currentTrain)
        {
            currentTrain = Instantiate(trainPrefab, this.transform, false);
        }

        if (Vector3.Distance(currentTrain.transform.position, transform.position) > maxTravelBeforeDestroy)
            DestroyTrain();

        if (Time.time - lastTrainDestruction > trainInterval) DestroyTrain();
    }

    private void DestroyTrain()
    {
        lastTrainDestruction = Time.time;
        Destroy(currentTrain);
        currentTrain = null;
    }
}
