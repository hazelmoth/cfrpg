using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the NPC does, and keeps track of what it's doing.
public class ActorBehaviour : MonoBehaviour
{
	public enum Activity {
		None,
		Eat,
		Wander
	}

	private ActorPhysicalCondition actorCondition;
	private NPCActivityExecutor executor;
	private bool isActing;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		ExecuteActivity (EvaluateBehaviour());
    }

	Activity EvaluateBehaviour () {
		if (actorCondition == null) {
			actorCondition = GetComponent<ActorPhysicalCondition> ();
		}
		if (executor == null) {
			executor = GetComponent<NPCActivityExecutor> ();
		}
		Activity nextActivity = Activity.None;

		// Start by checking for critical needs
		if (actorCondition != null && actorCondition.CurrentNutrition < 0.3f) {
			nextActivity = Activity.Eat;
		}

		// Wander if we have nothing else to do
		if (nextActivity == Activity.None && executor.CurrentActivity == Activity.None) {
			nextActivity = Activity.Wander;
		}
		return nextActivity;
	}
		
	void ExecuteActivity (Activity activity) {
		if (executor == null) {
			executor = GetComponent<NPCActivityExecutor> ();
		}
		switch (activity) {
		case Activity.Eat:
			executor.Execute_EatSomething ();
			break;
		case Activity.Wander:
			executor.Execute_Wander();
			break;
		// If there's nothing to do, keep doing whatever it is we were doing
		case Activity.None:
		default:
			break;
		}
	}
}
