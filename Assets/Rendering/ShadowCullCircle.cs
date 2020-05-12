using System;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Disables shadow casters far from the player
public class ShadowCullCircle : MonoBehaviour
{
	private const float checkRadius = 19f;
    private const float ShadowCullRadius = 18f;
    private CircleCollider2D checkCollider;

    void Start()
    {
	    checkCollider = GetComponent<CircleCollider2D>();
	    if (checkCollider == null)
	    {
		    checkCollider = gameObject.AddComponent<CircleCollider2D>();
		    checkCollider.radius = checkRadius;
		    checkCollider.isTrigger = true;
		    Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
		    rb.isKinematic = true;
	    }
    }

    private void Update()
    {
	    transform.position = ActorRegistry.Get(PlayerController.PlayerActorId).gameObject.transform.position;
    }


	[UsedImplicitly]
    void OnTriggerStay2D(Collider2D other)
    {
	    ShadowCaster2D caster = other.GetComponent<ShadowCaster2D>();
	    if (caster != null)
	    {
		    caster.enabled = InRange(other.transform.position);
	    }
    }

    private bool InRange(Vector2 pos)
    {
	    return Mathf.Abs(transform.position.x - pos.x) < checkRadius ||
	           Mathf.Abs(transform.position.y - pos.y) < checkRadius;

    }
}
