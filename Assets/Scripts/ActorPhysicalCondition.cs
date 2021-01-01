using UnityEngine;

// Stores and manages the physical condition of a human or creature.
public class ActorPhysicalCondition
{
	private bool hasInited = false;

	private const float NutritionLossPerHour = 0f; // Nutrition disabled for now

	public delegate void ActorPhysConditionEvent();
	public event ActorPhysConditionEvent OnDeath;

	// scale of 0 to 100; how well-fed the Actor currently is
	public float CurrentNutrition {get; private set;}
	public float CurrentHealth { get; private set; }
	public bool Sleeping { get; private set; }
	public bool IsDead { get; private set; }
	public IBed CurrentBed { get; private set; }
	public bool CanWalk => !IsDead && !Sleeping;

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
		Init(100f, 100f);
	}

	public void TakeHit(float force)
	{
		CurrentHealth -= force;
		if (!IsDead && CurrentHealth <= 0)
		{
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
		if (!hasInited)
			Init();

		CurrentNutrition += nutritionAmount;
		// TODO handle overeating
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

	private void OnMinuteElapsed()
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
			Die();
		}
	}
}
