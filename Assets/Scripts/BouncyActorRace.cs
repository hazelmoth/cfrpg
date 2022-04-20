using System;
using ActorAnim;
using UnityEngine;

/// An ActorRace that uses a simple bouncing animation instead of any sprite animation,
/// and doesn't support clothes or hair.
[CreateAssetMenu(menuName = "Race/BouncyActorRace")]
public class BouncyActorRace : BaseActorRace, IActorRace
{
    [SerializeField] private string id;
    [SerializeField][InspectorName("name")] private string raceName;
    [SerializeField] private float speed;
    [SerializeField] private float maxHealth;
    [SerializeField] private bool humanoid;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteUp;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteRight;
    [SerializeField] public Vector2 itemPosDown = new(-0.25f, 1);
    [SerializeField] public Vector2 itemPosUp = new(0.25f, 1.25f);
    [SerializeField] public Vector2 itemPosLeft = new(-0.25f, 1.25f);
    [SerializeField] public Vector2 itemPosRight = new(0.25f, 1);
    [SerializeField] private Sprite corpseItemSprite;
    [SerializeField] private CompoundWeightedTable butcherDrops;

    public override string Id => id;
    public override string Name => raceName;
    public override float Speed => speed;
    public override float MaxHealth => maxHealth;
    public override bool Humanoid => humanoid;
    public override Sprite CorpseItemSprite => corpseItemSprite;
    public override CompoundWeightedTable ButcherDrops => butcherDrops;
    public override bool SupportsHair => false;

    public override Vector2 GetItemPosition(Direction dir)
    {
        return dir switch
        {
            Direction.Up => itemPosUp,
            Direction.Left => itemPosLeft,
            Direction.Right => itemPosRight,
            _ => itemPosDown
        };
    }

    public override IActorSpriteController CreateSpriteController(Actor actor)
    {
        return new BouncySpriteController(actor, spriteDown, spriteUp, spriteLeft, spriteRight);
    }
}
