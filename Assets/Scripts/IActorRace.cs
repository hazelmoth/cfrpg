using System.Collections.Generic;
using ActorAnim;
using UnityEngine;

public interface IActorRace
{
    public string Id { get; }
    public string Name { get; }
    public float Speed { get; }
    public float MaxHealth { get; }
    public bool Humanoid { get; }
    public bool SupportsHair { get; }
    public CompoundWeightedTable ButcherDrops{ get; }

    /// The sprite of this actor's corpse, as an item
    public Sprite CorpseItemSprite { get; }

    /// The position, relative to the actor, where held items should be rendered.
    public Vector2 GetItemPosition(Direction dir);

    /// Returns a new sprite controller for the given actor.
    public IActorSpriteController CreateSpriteController(Actor actor);
}
