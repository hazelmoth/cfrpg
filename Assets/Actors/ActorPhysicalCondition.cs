using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorPhysicalCondition
{
	const float NutritionLossPerHour = 0.03f;
	// TODO: consume calories faster while moving around

	// scale of 0 to 1; how well-fed the npc currently is
	public float CurrentNutrition {get; private set;}

	bool hasInited = false;

	public void Init(float currentNutrition)
	{
		if (!hasInited)
			TimeKeeper.OnMinuteChanged += OnMinuteElapsed;

		hasInited = true;

		CurrentNutrition = currentNutrition;

	}
	public void Init()
	{
		Init(1f);
	}

	void OnMinuteElapsed()
	{
		if (!hasInited)
			Init();

		CurrentNutrition -= NutritionLossPerHour / 60f;
		
		if (CurrentNutrition < 0) {
			CurrentNutrition = 0;
		}

		if (CurrentNutrition == 0) {
			// Starvation
		}
	}

    public void IntakeNutrition (float nutritionAmount)
    {
		if (!hasInited)
			Init();

		CurrentNutrition += nutritionAmount;
        //if (CurrentNutrition > 1)
            //CurrentNutrition = 1; // TODO handle overeating
    }

}
