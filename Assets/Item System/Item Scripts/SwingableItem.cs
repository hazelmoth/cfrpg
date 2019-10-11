using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SwingableItem : Item
{
	[SerializeField] Sprite ingameItemSprite;
	[SerializeField] float swingDuration = 0.5f;


	public void Swing(Actor wieldingActor)
	{
		ItemSwingAnimSystem.Animate(ingameItemSprite, wieldingActor, swingDuration, OnMidSwing);
	}
	protected abstract void OnMidSwing(Actor actor);

}
