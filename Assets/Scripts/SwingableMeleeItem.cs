using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "new_melee_item", menuName = "Items/Swingable Melee Item", order = 1)]
public class SwingableMeleeItem : SwingableItem
{
	[SerializeField] private float range = 1f;
	[SerializeField] private float force = 20f;
	[SerializeField] private ImpactInfo.DamageType impactDamageType = ImpactInfo.DamageType.Slash;

	protected override void OnMidSwing(Actor actor)
	{
		ImpactSystem.ExertDirectionalForce(
			actor,
			TilemapInterface.WorldPosToScenePos(actor.transform.position,
				actor.CurrentScene),
			actor.Direction.ToVector2(),
			range,
			force,
			impactDamageType,
			actor.CurrentScene);
	}
}
