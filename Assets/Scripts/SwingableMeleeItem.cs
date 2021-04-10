using UnityEngine;

[CreateAssetMenu(fileName = "new_melee_item", menuName = "Items/Swingable Melee Item", order = 1)]
public class SwingableMeleeItem : SwingableItem
{
	[SerializeField] private float range = 1f;
	[SerializeField] private float force = 20f;
	protected override void OnMidSwing(Actor actor)
	{
		PunchSystem.ExertDirectionalPunch(
			actor,
			TilemapInterface.WorldPosToScenePos(actor.transform.position,
				actor.CurrentScene),
			actor.Direction.ToVector2(),
			range,
			force,
			actor.CurrentScene);
	}
}
