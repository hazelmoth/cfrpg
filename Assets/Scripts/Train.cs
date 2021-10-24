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
    [SerializeField] private float speed = -13f;
    [SerializeField] private List<Wheel> wheels;

    private void Update()
    {
        float linearDist = speed * Time.deltaTime;
        wheels.ForEach(
            wheel =>
            {
                float angularDist = Mathf.Rad2Deg * (linearDist / wheel.radius);
                wheel.wheelObject.transform.Rotate(Vector3.forward, -angularDist, Space.Self);
            });
        transform.Translate(Vector3.right * linearDist);
    }
}
