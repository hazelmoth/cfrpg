using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public static void ExertForce (Vector2 relativeOrigin, Direction direction, string scene) {
		ExertForce(relativeOrigin, direction.ToVector2(), scene);

	}
	public static void ExertForce (Vector2 relativeOrigin, Vector2 direction, string scene) {
		
	}
}
