using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorHealth
{
	public delegate void ActorPhysConditionEvent();
	public event ActorPhysConditionEvent OnDeath;
	
	public float MaxHealth { get; }
	public bool Sleeping { get; private set; }
	public bool IsDead => CurrentHealth == 0;
	public IBed CurrentBed { get; private set; }
	public float CurrentHealth { get; private set; }

	public ActorHealth(float maxHealth, float currentHealth)
	{
		MaxHealth = maxHealth;
		CurrentHealth = currentHealth;
	}

	/// Resets this condition back to perfect health.
	public void ResetHealth()
	{
		CurrentHealth = MaxHealth;
	}
	
	public void TakeHit(float force)
	{
		if (IsDead) return;
		
		CurrentHealth -= force;
		if (CurrentHealth <= 0)
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
		if (OnDeath == null)
		{
			Debug.Log("No subscriptions to this death event.");
		}
		OnDeath?.Invoke();
	}
}
