using ContentLibraries;
using UnityEngine;

namespace ActorAnim
{
    /// A MonoBehaviour that controls the appearance of an actor.
    public class ActorSpriteController : MonoBehaviour
    {
        private IActorSpriteController spriteController;
        private Direction? forcedDirection;
        private bool unlockDirectionNextFrame;

        private bool FaceTowardsMouse { get; set; }

        public Direction CurrentDirection => spriteController?.CurrentDirection ?? Direction.Down;

        private void Start ()
        {
            Actor actor = GetComponent<Actor>();
            if (actor == null)
            {
                Debug.LogError("ActorSpriteController requires an Actor component.");
                return;
            }

            spriteController = ContentLibrary.Instance.Races.Get(actor.GetData().RaceId).CreateSpriteController(actor);
        }

        private void Update()
        {
            if (PauseManager.Paused) return;

            if (forcedDirection != null)
                spriteController.UpdateSprites(forcedDirection.Value);
            else if (FaceTowardsMouse)
                spriteController.UpdateSprites(DirectionTowardsMouse());
            else
                spriteController.UpdateSprites();

            if (unlockDirectionNextFrame)
            {
                forcedDirection = null;
                unlockDirectionNextFrame = false;
            }
        }

        public void StartAttackAnim(Vector2 direction)
        {
            spriteController.StartAttackAnim(direction);
        }

        /// Holds the actor's sprites in the given direction for a single frame.
        public void ForceDirection(Direction direction)
        {
            forcedDirection = direction;
            unlockDirectionNextFrame = true;
        }

        /// Begins holding the actor's sprites in the given direction until unlocked.
        public void LockDirection(Direction dir)
        {
            forcedDirection = dir;
        }

        /// Stops holding the actor's sprites in the given direction.
        public void UnlockDirection()
        {
            forcedDirection = null;
        }

        private Direction DirectionTowardsMouse()
        {
            Vector2 vector = MousePositionHelper.GetMouseWorldPos() - (Vector2)transform.position;
            return vector.ToDirection();
        }
    }
}
