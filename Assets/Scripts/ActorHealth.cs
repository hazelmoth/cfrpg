using UnityEngine;

/// Stores and manages the physical condition of an actor.
public class ActorHealth
{
	private const float RegenPerTick = 1/240f; // 4 seconds per unit

	public delegate void ActorPhysConditionEvent();
	public event ActorPhysConditionEvent OnDeath;

	public float MaxHealth { get; }
	public float CurrentHealth { get; private set; }
	public bool Sleeping { get; private set; }
	public IBed CurrentBed { get; private set; }

	public bool IsDead => CurrentHealth == 0;

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

	/// Applies the specified number of ticks' worth of health regen.
	/// (Note: if the Actor is dead this will revive them.)
	public void Regen(int ticks)
	{
		AdjustHealth(RegenPerTick * ticks);
	}
	
	public void TakeHit(float force)
	{
		if (IsDead) return;
		
		AdjustHealth(-force);
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

	/// Adjusts the current health by the specified amount, but going no higher than
	/// MaxHealth. The actor dies if the result is less than zero.
	private void AdjustHealth(float amount)
	{
		float previousHealth = CurrentHealth;
		CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
		if (CurrentHealth > 0) return;

		CurrentHealth = 0;
		if (previousHealth > 0)
			Die();
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
