using UnityEngine;

public abstract class SwingableItem : ItemData
{
	[SerializeField] private Sprite ingameItemSprite;
	[SerializeField] private float swingDuration = 0.5f;


	public void Swing(Actor wieldingActor)
	{
		ItemSwingAnimSystem.Animate(ingameItemSprite, wieldingActor, swingDuration, OnMidSwing);
	}
	protected abstract void OnMidSwing(Actor actor);

}
