using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

// A CollisionChecker keeps track of all objects its game object is currently
// trigger-colliding with.
public class CollisionChecker : MonoBehaviour
{
	private ISet<Collider2D> collidingObjects;

	/// Whether this checker is colliding with anything.
	public bool Colliding() => collidingObjects.Count != 0;

	/// Returns whether this checker is colliding, ignoring collisions with the
	/// given object.
	public bool Colliding(ISet<Collider2D> exclude)
	{
		if (collidingObjects == null) return false;
		if (exclude.IsNullOrEmpty()) return Colliding();

		return collidingObjects.Any(colliding => !exclude.Contains(colliding));
	}
	
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collidingObjects == null)
		{
			InitSet();
		}

		collidingObjects.Add(collider);
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collidingObjects == null)
		{
			InitSet();
		}
		else if (collidingObjects.Contains(collider))
		{
			collidingObjects.Remove(collider);
		}
	}

	private void InitSet ()
	{
		collidingObjects = new HashSet<Collider2D>();
	}
}
