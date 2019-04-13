using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorPhysicalCondition : MonoBehaviour
{
	// scale of 0 to 1; how well-fed the npc currently is
	public float CurrentNutrition {get; private set;}
	bool hasInited = false;

	const float NutritionLossPerHour = 0.03f;
	// TODO: consume calories faster while moving around

	void Start() {
		if (!hasInited)
			Init ();
	}
	public void Init(float currentNutrition) {
		hasInited = true;

		TimeKeeper.OnSecondChanged += OnSecondElapsed;
		CurrentNutrition = currentNutrition;
	}
	public void Init() {
		Init(1f);

	}
	void OnSecondElapsed() {
		CurrentNutrition -= NutritionLossPerHour / 3600f;
		if (CurrentNutrition < 0) {
			CurrentNutrition = 0;
		}
		if (CurrentNutrition == 0) {
			Debug.Log (this.GetComponent<NPCCharacter> ().NPCName + " is starving.");
			// Starvation
		}
	}

    public void IntakeNutrition (float nutritionAmount)
    {
        CurrentNutrition += nutritionAmount;
        if (nutritionAmount > 1)
            nutritionAmount = 1; // TODO handle overeating
    }

}
