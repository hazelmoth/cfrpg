using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_melee_item", menuName = "Items/Swingable Melee Item", order = 1)]
public class SwingableMeleeItem : SwingableItem
{
	[SerializeField] float range = 1f;
	[SerializeField] float force = 20f;
	protected override void OnMidSwing(Actor actor)
	{
		PunchSystem.ExertDirectionalPunch(TilemapInterface.WorldPosToScenePos(actor.transform.position, actor.CurrentScene), actor.Direction, range, force, actor.CurrentScene);
	}
}
