using UnityEngine;

namespace ActorAnim
{
    /// An IActorSpriteController is responsible for rendering and updating an actor's sprites.
    public interface IActorSpriteController
    {
        /// Updates the actor's sprites based on its current state.
        void UpdateSprites();

        /// Updates the actor's sprites based on its current state, facing towards the
        /// given direction if possible.
        void UpdateSprites(Direction forcedDirection);

        /// The direction that the actor is currently facing.
        Direction CurrentDirection { get; }

        /// Initiates the attack animation for the actor, attacking in the given direction.
        void StartAttackAnim(Vector2 direction);
    }
}
