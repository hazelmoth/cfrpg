using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ActorComponents
{
	/// Stores and manages the physical condition of an actor.
	[Serializable]
	public class ActorHealth : IActorComponent
	{
		private const float RegenPerTick = 1/240f; // 4 seconds per unit

		private Location currentBedPos;

		public delegate void ActorPhysConditionEvent();
		public event ActorPhysConditionEvent OnDeath;

		public float MaxHealth { get; }
		public float CurrentHealth { get; private set; }
		public bool Sleeping { get; private set; }

		[JsonIgnore]
		public IBed CurrentBed
		{
			get
			{
				if (!Sleeping) return null;
				return RegionMapManager.GetEntityObjectAtPoint(currentBedPos.Vector2.ToVector2Int(), currentBedPos.scene)
					?.GetComponent<IBed>();
			}
			private set
			{
				if (value == null) currentBedPos = null;

				MonoBehaviour monoBehaviour = value as MonoBehaviour;
				Debug.Assert(monoBehaviour != null, "Bed must be a MonoBehaviour");
				EntityObject entityObject = monoBehaviour.GetComponent<EntityObject>();
				Debug.Assert(entityObject != null, "Bed must be an EntityObject");
				currentBedPos = entityObject.Location;
			}
		}

		public bool Dead => CurrentHealth == 0;

		public ActorHealth(float maxHealth, float currentHealth)
		{
			MaxHealth = maxHealth;
			CurrentHealth = currentHealth;
		}

		/// Resets back to perfect health.
		public void Reset()
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
			if (Dead) return;
		
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

		/// Adjusts the current health by the specified amount, but going no higher than
		/// MaxHealth. The actor dies if the result is less than zero.
		public void AdjustHealth(float amount)
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
}
