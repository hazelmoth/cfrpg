using System.Collections;
using UnityEngine;

namespace ActorAnim
{
    public class BouncySpriteController : IActorSpriteController
    {
        /// Which sorting layer actor sprites are on
        private const string BodySortingLayer = "Entities";

        private const float BounceDuration = 0.25f;

        private readonly Actor actor;
        private readonly SpriteRenderer bodyRenderer;
        private readonly GameObject spriteParent;
        private readonly GameObject bodySpriteObj;
        private readonly Sprite spriteDown;
        private readonly Sprite spriteUp;
        private readonly Sprite spriteLeft;
        private readonly Sprite spriteRight;

        private Direction currentDirection = Direction.Down;
        private float lastBounceTime;

        public BouncySpriteController(Actor actor, Sprite spriteDown, Sprite spriteUp, Sprite spriteLeft, Sprite spriteRight)
        {
            this.actor = actor;
            this.spriteDown = spriteDown;
            this.spriteUp = spriteUp;
            this.spriteLeft = spriteLeft;
            this.spriteRight = spriteRight;

            this.actor = actor;

            // Create clothing sprite gameobjects
            spriteParent = new GameObject("Sprites");
            spriteParent.transform.SetParent(actor.transform);
            spriteParent.transform.localPosition = Vector3.zero;

            bodySpriteObj = new("Body");
            bodySpriteObj.transform.SetParent(spriteParent.transform);
            bodySpriteObj.transform.localPosition = Vector3.zero;
            bodyRenderer = bodySpriteObj.AddComponent<SpriteRenderer>();
            bodyRenderer.sortingLayerName = BodySortingLayer;
        }

        public void UpdateSprites()
        {
            UpdateSprites(
                actor.WalkVector.magnitude > 0
                ? actor.WalkVector.ToDirection()
                : currentDirection);
        }

        public void UpdateSprites(Direction forcedDirection)
        {
            currentDirection = forcedDirection;
            bodyRenderer.sprite = GetSprite(forcedDirection);

            if (actor.WalkVector.magnitude > 0.1f && Time.time - lastBounceTime > BounceDuration)
            {
                lastBounceTime = Time.time;
                actor.StartCoroutine(BounceAnimCoroutine());
            }
        }

        public Direction CurrentDirection => currentDirection;

        public void StartAttackAnim(Vector2 direction)
        {
            // Animate a lunge in the direction of the attack.
            actor.StartCoroutine(LungeAnimCoroutine(direction));
        }

        /// Briefly moves the actor's sprite in the given direction.
        private IEnumerator LungeAnimCoroutine(Vector2 direction)
        {
            const float distance = 0.3f;
            const float duration = 0.1f;

            Vector2 startPos = spriteParent.transform.localPosition;
            Vector2 endPos = startPos + direction * distance;

            float startTime = Time.time;

            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                spriteParent.transform.localPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            // Move back to the original position.
            startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                spriteParent.transform.localPosition = Vector2.Lerp(endPos, startPos, t);
                yield return null;
            }
        }

        /// Bounces the actor's sprite up and down once.
        private IEnumerator BounceAnimCoroutine()
        {
            const float distance = 0.25f;
            const float duration = BounceDuration / 2;

            bodySpriteObj.transform.localPosition = Vector3.zero;
            Vector2 startPos = bodySpriteObj.transform.localPosition;
            Vector2 endPos = startPos + Vector2.up * distance;

            float startTime = Time.time;

            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                bodySpriteObj.transform.localPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }

            startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                bodySpriteObj.transform.localPosition = Vector2.Lerp(endPos, startPos, t);
                yield return null;
            }
        }

        private Sprite GetSprite(Direction dir)
        {
            return dir switch
            {
                Direction.Down => spriteDown,
                Direction.Up => spriteUp,
                Direction.Left => spriteLeft,
                Direction.Right => spriteRight,
                _ => spriteDown
            };
        }
    }
}
