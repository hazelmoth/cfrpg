using System.Collections;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

namespace ActorAnim.Animation
{
	/// Directly controls an actor's Animator.
	internal class ActorAnimController : MonoBehaviour {

		private Animator animator;
		private ClothedAnimatedSpriteController spriteController;
		private static readonly int DirectionAnimProperty = Animator.StringToHash("direction");
		private static readonly int IsWalkingAnimProperty = Animator.StringToHash("isWalking");
		private static readonly int PunchDirectionAnimProperty = Animator.StringToHash("punchDirection");
		public bool InPunchAnim { get; private set; }

		public void Init (ClothedAnimatedSpriteController spriteController, RuntimeAnimatorController animController)
		{
			animator = this.GetOrAddComponent<Animator>();
			animator.runtimeAnimatorController = animController;
			this.spriteController = spriteController;
		}

		public void SetDirection (Direction dir)
		{
			if (animator == null) return;

			switch(dir) {
				case Direction.Down:
					animator.SetInteger (DirectionAnimProperty, 0);
					break;
				case Direction.Right:
					animator.SetInteger (DirectionAnimProperty, 1);
					break;
				case Direction.Up:
					animator.SetInteger (DirectionAnimProperty, 2);
					break;
				case Direction.Left:
					animator.SetInteger (DirectionAnimProperty, 3);
					break;
				default:
					Debug.LogError("Invalid direction: " + dir);
					break;
			}
		}

		public void SetWalking (bool isWalking) {
			animator.SetBool (IsWalkingAnimProperty, isWalking);
		}

		public void PlayPunchAnim (float duration, Direction direction)
		{
			StartCoroutine(PunchCoroutine(duration, direction));
		}

		private IEnumerator PunchCoroutine (float duration, Direction direction)
		{
			switch (direction)
			{
				case Direction.Down:
					animator.SetInteger(PunchDirectionAnimProperty, 0);
					break;
				case Direction.Right:
					animator.SetInteger(PunchDirectionAnimProperty, 1);
					break;
				case Direction.Up:
					animator.SetInteger(PunchDirectionAnimProperty, 2);
					break;
				case Direction.Left:
					animator.SetInteger(PunchDirectionAnimProperty, 3);
					break;
			}
			animator.SetTrigger("startPunch");
			spriteController.ShowPunchSprites();
			InPunchAnim = true;
			yield return new WaitForSeconds(duration);
			animator.SetTrigger("finishPunch");
			InPunchAnim = false;
		}

		/// Called by animation events.
		[UsedImplicitly]
		public void SetFrame (int animFrame)
		{
			spriteController?.SetFrame(animFrame);
		}

		/// Called by animation events... kindly refrain from calling this, I guess
		[UsedImplicitly]
		public void StartPunch() {
			spriteController.ShowPunchSprites();
		}

		public Direction GetDirection () {
			int dir = animator.GetInteger (DirectionAnimProperty);
			switch (dir) {
				case 0:
					return Direction.Down;
				case 1:
					return Direction.Right;
				case 2:
					return Direction.Up;
				default:
					return Direction.Left;
			}
		}
		public Direction GetPunchDirection()
		{
			int dir = animator.GetInteger(PunchDirectionAnimProperty);
			switch (dir)
			{
				case 0:
					return Direction.Down;
				case 1:
					return Direction.Right;
				case 2:
					return Direction.Up;
				default:
					return Direction.Left;
			}
		}

		public Vector2 GetDirectionVector2 ()
		{
			return GetDirection().ToVector2();
		}
	}
}
