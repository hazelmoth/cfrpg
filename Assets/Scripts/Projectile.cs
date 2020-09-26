using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void ProjectileCallback(Projectile projectile, Collider2D other);

    private ProjectileCallback callback;
    private Collider2D ignoredCollider;


    public void Init(ProjectileCallback callback, Collider2D ignore)
    {
	    this.callback = callback;
	    this.ignoredCollider = ignore;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
	    if (other != ignoredCollider && other.GetComponent<DroppedItem>() == null && other.GetComponent<ShadowCullCircle>() == null) // Don't block bullets with items or shadow culler
	    {
		    callback(this, other);
	    }
    }
}
