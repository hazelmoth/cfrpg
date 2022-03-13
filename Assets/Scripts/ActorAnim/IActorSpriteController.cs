using UnityEngine;

namespace ActorAnim
{
    public interface IActorSpriteController
    {
        /// Updates the actor's sprites based on its current state.
        void UpdateSprites();

        void UpdateSprites(Direction forcedDirection);

        Direction CurrentDirection { get; }

        void StartAttackAnim(Vector2 direction);
    }
}
