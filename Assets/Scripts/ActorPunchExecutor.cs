using UnityEngine;

public class ActorPunchExecutor : MonoBehaviour
{
	private Actor actor;
	private ActorAnimController animController;
	private float lastPunchTime = 0f;

	private const float PunchDuration = 0.2f;
	private const float PunchDamage = 5f;
	
	// Start is called before the first frame update
	private void Start()
    {
		actor = GetComponent<Actor>();
		animController = GetComponent<ActorAnimController>();
	}

	public void InitiatePunch (Vector2 direction)
	{
		InitiatePunch(PunchDamage, 1.0f, direction);
	}
	private void InitiatePunch (float strength, float range, Vector2 direction)
	{
		// Don't punch if we're still in the midst of a punch
		if (Time.time - lastPunchTime < PunchDuration && lastPunchTime != 0f)
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
			animController = GetComponent<ActorAnimController>();
		if (animController != null)
		{
			animController.AnimatePunch(PunchDuration, direction.ToDirection());
		}
		Vector2 posInScene = TilemapInterface.WorldPosToScenePos(transform.position, actor.CurrentScene);

		// Exert the punch force
		PunchSystem.ExertDirectionalPunch(actor, posInScene, direction, range, strength, actor.CurrentScene);
	}
}