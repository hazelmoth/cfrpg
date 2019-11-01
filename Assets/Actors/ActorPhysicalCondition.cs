using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorPhysicalCondition
{
	bool hasInited = false;

	const float NutritionLossPerHour = 0.03f;

	public delegate void ActorPhysConditionEvent();
	public event ActorPhysConditionEvent OnDeath;

	// scale of 0 to 1; how well-fed the npc currently is
	public float CurrentNutrition {get; private set;}
	public float CurrentHealth { get; private set; }
	public bool IsDead { get; private set; }

	public void Init(float currentNutrition, float currentHealth)
	{
		if (!hasInited)
			TimeKeeper.OnMinuteChanged += OnMinuteElapsed;

		hasInited = true;

		CurrentNutrition = currentNutrition;
		CurrentHealth = currentHealth;
	}
	public void Init()
	{
		Init(1f, 1f);
	}
	public void TakeHit(float force)
	{
		CurrentHealth -= force;
		if (!IsDead && CurrentHealth <= 0)
		{
			Die();
		}
	}
	public void IntakeNutrition(float nutritionAmount)
	{
		if (!hasInited)
			Init();

		CurrentNutrition += nutritionAmount;
		//if (CurrentNutrition > 1)
		//CurrentNutrition = 1; // TODO handle overeating
	}

	void Die()
	{
		IsDead = true;
		if (OnDeath == null)
		{
			Debug.Log("No subscriptions to this death event.");
		}
		OnDeath?.Invoke();
	}
	void OnMinuteElapsed()
	{
		if (!hasInited)
			Init();

		// TODO: consume calories faster while moving around
		CurrentNutrition -= NutritionLossPerHour / 60f;
		
		if (CurrentNutrition < 0) {
			CurrentNutrition = 0;
		}

		if (CurrentNutrition == 0) {
			// Starvation
		}
	}


}
