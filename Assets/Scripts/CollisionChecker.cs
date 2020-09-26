using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
	public bool Colliding { get; private set; }

	private ISet<Collider2D> collidingObjects;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collidingObjects == null)
		{
			initSet();
		}

		collidingObjects.Add(collider);
		Colliding = true;
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collidingObjects == null)
		{
			initSet();
		}
		else if (collidingObjects.Contains(collider))
		{
			collidingObjects.Remove(collider);
		}

		Colliding = (collidingObjects.Count > 0);
	}

	private void initSet ()
	{
		collidingObjects = new HashSet<Collider2D>();
	}
}
