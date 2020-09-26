using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Disables shadow casters far from the player
public class ShadowCullCircle : MonoBehaviour
{
	private const float CheckDistance = 19f;
    private const float ShadowCullRadius = 18f;
    private BoxCollider2D checkCollider;

    void Start()
    {
	    checkCollider = GetComponent<BoxCollider2D>();
	    if (checkCollider == null)
	    {
		    checkCollider = gameObject.AddComponent<BoxCollider2D>();
		    checkCollider.size = new Vector2(CheckDistance, CheckDistance);
		    checkCollider.isTrigger = true;
		    Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
		    rb.isKinematic = true;
	    }
    }

    private void Update()
    {
	    transform.position = ActorRegistry.Get(PlayerController.PlayerActorId).actorObject.transform.position;
    }


	[UsedImplicitly]
    void OnTriggerStay2D(Collider2D other)
    {
	    if (other.TryGetComponent(out ShadowCaster2D caster))
	    {
		    caster.enabled = InRange(other.transform.position);
	    }
    }

    private bool InRange(Vector2 pos)
    {
	    return Mathf.Abs(transform.position.x - pos.x) < ShadowCullRadius &&
	           Mathf.Abs(transform.position.y - pos.y) < ShadowCullRadius;
    }
}
