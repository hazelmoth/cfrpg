using System;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [Serializable]
    public class Wheel
    {
        public GameObject wheelObject;
        public float radius;
    }

    [SerializeField] private float speed = -6f;
    [SerializeField] private float timeToAccel = 12f;
    [SerializeField] private List<Wheel> wheels;

    private float currentSpeed;
    private bool stopping;
    private float stoppingTime;
    private float timeWhenFullyStopped;
    private float stopDuration;

    public bool Stopped => currentSpeed == 0f;

    public void Stop(float stoppingDistance, float stopDuration)
    {
        stopping = true;
        this.stopDuration = stopDuration;
        stoppingDistance = Mathf.Abs(stoppingDistance);
        stoppingDistance *= Mathf.Sign(speed);
        stoppingTime = stoppingDistance / (speed / 2);
        Debug.Assert(stoppingTime > 0f, "Stopping time isn't positive");
    }

    private void Start()
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        // If the train is stopping, slow the current speed such that it goes from speed
        // to 0 over a total distance of StoppingDistance.
        if (stopping && currentSpeed != 0)
        {
            float speedChangePerSecond = -speed / stoppingTime;
            currentSpeed += speedChangePerSecond * Time.deltaTime;
            currentSpeed = speed > 0
                ? Mathf.Clamp(currentSpeed, 0, speed)
                : Mathf.Clamp(currentSpeed, speed, 0);
            if (currentSpeed == 0)
            {
                timeWhenFullyStopped = Time.time;
            }
        }
        else if (stopping && currentSpeed == 0f)
        {
            if (Time.time - timeWhenFullyStopped > stopDuration)
            {
                stopping = false;
            }
        }
        else
        {
            // If the train isn't stopping, accelerate to normal speed.
            float accel = speed / timeToAccel;
            currentSpeed += accel * Time.deltaTime;
            currentSpeed = speed > 0
                ? Mathf.Clamp(currentSpeed, 0, speed)
                : Mathf.Clamp(currentSpeed, speed, 0);
        }

        // Rotate the wheels based on the current speed.
        float linearDist = currentSpeed * Time.deltaTime;
        wheels.ForEach(
            wheel =>
            {
                float angularDist = Mathf.Rad2Deg * (linearDist / wheel.radius);
                wheel.wheelObject.transform.Rotate(Vector3.forward, -angularDist, Space.Self);
            });

        // Move the train forward.
        transform.Translate(Vector3.right * linearDist);
    }
}
