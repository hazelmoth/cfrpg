using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorPunchExecutor : MonoBehaviour
{
	private Actor actor;
	private HumanAnimController animController;
	float lastPunchTime = 0f;

	const float punchDuration = 0.17f;
	// Start is called before the first frame update
	void Start()
    {
		actor = GetComponent<Actor>();
		animController = GetComponent<HumanAnimController>();
	}

	public void InitiatePunch (Vector2 direction)
	{
		InitiatePunch(1.0f, 1.0f, direction);
	}
	public void InitiatePunch (float strength, float range, Vector2 direction)
	{
		// Don't punch if we're still in the midst of a punch
		if (Time.time - lastPunchTime < punchDuration && lastPunchTime != 0f)
		{
			return;
		}
		lastPunchTime = Time.time;
		if (actor == null)
		{
			actor = GetComponent<Actor>();
			if (actor == null)
				return;
		}
		if (animController == null)
			animController = GetComponent<HumanAnimController>();
		if (animController != null)
		{
			animController.AnimatePunch(punchDuration, direction.ToDirection());
		}
		Vector2 posInScene = TilemapInterface.WorldPosToScenePos(transform.position, actor.CurrentScene);
		// Exert the punch force
		PunchSystem.ExertDirectionalPunch(posInScene, direction, range, strength, actor.CurrentScene);
	}

	public bool ObjectIsInRange (GameObject gameObject)
	{
		return false;
	}
}
