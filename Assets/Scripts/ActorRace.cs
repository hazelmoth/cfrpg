using System.Collections.Generic;
using ActorAnim;
using UnityEngine;

[CreateAssetMenu(menuName = "Race/ActorRace")]
public class ActorRace : BaseActorRace, IActorRace
{
	[SerializeField] private string id;
	[SerializeField] private string name;
	[SerializeField] private float health = 100f;
	[SerializeField] private float speed = 3f; // Units per second
	[SerializeField] private bool humanoid;
	[SerializeField] private bool supportsHair;

	// Whether the hair, shirt, and hat should be lowered on walk frames
	[SerializeField] private bool bounceUpperSprites;
	
	[SerializeField] private CompoundWeightedTable butcherDrops;
	
	[SerializeField] public Vector2 itemPosDown;
	[SerializeField] public Vector2 itemPosUp;
	[SerializeField] public Vector2 itemPosLeft;
	[SerializeField] public Vector2 itemPosRight;

	[SerializeField] private RuntimeAnimatorController animatorController;
	[SerializeField] private Sprite itemSprite;
	[SerializeField] private List<Sprite> bodySprites;
	[SerializeField] private List<Sprite> swooshSprites;
	
	public override string Id => id;
	public override string Name => name;
	public override float Speed => speed;
	public override float MaxHealth => health;
	public override bool Humanoid => humanoid;
	public override bool SupportsHair => supportsHair;
	public bool BounceUpperSprites => bounceUpperSprites;
	public override CompoundWeightedTable ButcherDrops => butcherDrops;
	public override Sprite CorpseItemSprite => itemSprite;
	public List<Sprite> BodySprites => bodySprites;
	public List<Sprite> SwooshSprites => swooshSprites;

	public override Vector2 GetItemPosition(Direction dir)
	{
		return dir switch
		{
			Direction.Down => itemPosDown,
			Direction.Up => itemPosUp,
			Direction.Left => itemPosLeft,
			_ => itemPosRight
		};
	}

	public override IActorSpriteController CreateSpriteController(Actor actor)
	{
		return new ClothedAnimatedSpriteController(actor, animatorController);
	}
}
