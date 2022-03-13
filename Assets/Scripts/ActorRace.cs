using System.Collections.Generic;
using ActorAnim;
using UnityEngine;

[CreateAssetMenu]
public class ActorRace : ScriptableObject
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
	
	public string Id => id;
	public string Name => name;
	public float Speed => speed;
	public float MaxHealth => health;
	public bool Humanoid => humanoid;
	public bool SupportsHair => supportsHair;
	public bool BounceUpperSprites => bounceUpperSprites;
	public CompoundWeightedTable ButcherDrops => butcherDrops;
	public Sprite ItemSprite => itemSprite;
	public List<Sprite> BodySprites => bodySprites;
	public List<Sprite> SwooshSprites => swooshSprites;

	public Vector2 GetItemPosition(Direction dir)
	{
		switch (dir)
		{
			case Direction.Down:
				return itemPosDown;
			case Direction.Up:
				return itemPosUp;
			case Direction.Left:
				return itemPosLeft;
			default:
				return itemPosRight;
		}
	}

	public IActorSpriteController CreateSpriteController(Actor actor)
	{
		return new ClothedAnimatedSpriteController(actor, animatorController);
	}
}
