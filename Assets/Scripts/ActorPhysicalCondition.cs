using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorPhysicalCondition
{
	public delegate void ActorPhysConditionEvent();
	public event ActorPhysConditionEvent OnDeath;
	
	public float MaxHealth { get; }
	public bool Sleeping { get; private set; }
	public bool IsDead { get; private set; }
	public IBed CurrentBed { get; private set; }
	public float CurrentHealth { get; private set; }

	public ActorPhysicalCondition(float maxHealth, float currentHealth)
	{
		MaxHealth = maxHealth;
		CurrentHealth = currentHealth;
	}

	public void TakeHit(float force)
	{
		CurrentHealth -= force;
		if (!IsDead && CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			Die();
		}
	}

	public void Sleep (IBed bed)
	{
		Sleeping = true;
		CurrentBed = bed;
	}

	public void WakeUp ()
	{
		Sleeping = false;
	}

	public void IntakeNutrition(float nutritionAmount)
	{
		// TODO handle eating
	}

	private void Die()
	{
		IsDead = true;
		if (OnDeath == null)
		{
			Debug.Log("No subscriptions to this death event.");
		}
		OnDeath?.Invoke();
	}
}
