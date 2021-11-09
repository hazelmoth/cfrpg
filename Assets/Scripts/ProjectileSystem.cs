using System.Collections.Generic;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
	private const float MaxProjectileLifetime = 30;
	
	private static ProjectileSystem instance;
	private IDictionary<int, ProjectileData> projectiles;
	private GameObject projectileParent;

	private struct ProjectileData
	{
		public ProjectileData(Projectile projectileObject, Vector2 velocity, float force, float range, Actor source = null)
		{
			this.projectileObject = projectileObject;
			this.velocity = velocity;
			this.force = force;
			this.startTIme = Time.time;
			this.maxLifetime = Mathf.Min(MaxProjectileLifetime, range / this.velocity.magnitude);
			this.actor = source;
		}
		public readonly Projectile projectileObject;
		public readonly Vector2 velocity;
		public readonly float force;
		public readonly float startTIme;
		public readonly float maxLifetime;
		public readonly Actor actor;
	}

    // Start is called before the first frame update
    private void Start()
    {
	    instance = this;
	    instance.projectileParent = new GameObject("Projectiles");
	    projectiles = new Dictionary<int, ProjectileData>();
	}

    // Update is called once per frame
    private void Update()
    {
	    IList<int> removeList = new List<int>(32);

	    foreach (int key in projectiles.Keys)
	    {
		    ProjectileData projectile = projectiles[key];
		    if (projectile.projectileObject == null || Time.time - projectile.startTIme > projectile.maxLifetime)
		    {
				removeList.Add(key);
		    }
		    else
		    {
			    projectile.projectileObject.transform.Translate(projectile.velocity * Time.deltaTime, Space.World);
		    }
	    }

	    for (int i = 0; i < removeList.Count; i++)
	    {
		    Projectile projObject = projectiles[removeList[i]].projectileObject;
		    if (projObject != null)
		    {
			    Destroy(projObject.gameObject);
		    }

		    projectiles.Remove(removeList[i]);
	    }
    }

    public static void LaunchProjectile(
	    GameObject prefab,
	    Vector2 origin,
	    float angle,
	    float speed,
	    float force,
	    float maxRange,
	    float colliderRadius,
	    Collider2D ignoredCollider,
	    bool flipProjectileSprite,
	    Actor attacker = null)
    {
	    if (instance.projectileParent == null)
	    {
			instance.projectileParent = new GameObject("Projectiles");
	    }

		GameObject projectile = Instantiate(prefab);
		projectile.name = "Projectile";
		Projectile projectileComp = projectile.GetComponent<Projectile>();
		
		projectile.transform.SetParent(instance.projectileParent.transform);
		projectile.transform.position = origin;
		projectile.transform.Rotate(Vector3.forward, angle);

		CircleCollider2D collider = projectile.GetComponent<CircleCollider2D>();
		collider.isTrigger = true;
		collider.radius = colliderRadius;
		
		SpriteRenderer spriteRenderer = projectile.GetComponentInChildren<SpriteRenderer>();
		if (flipProjectileSprite)
		{
			spriteRenderer.flipY = true;
		}

		Vector2 velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed);
		ProjectileData projectileData = new ProjectileData(projectileComp, velocity, force, maxRange, attacker);

		instance.projectiles.Add(projectile.GetInstanceID(), projectileData);
		projectileComp.Init(OnProjectileCollide, ignoredCollider);
	}

    private static void OnProjectileCollide(Projectile projectile, Collider2D other)
    {
	    ProjectileData data;

	    if (instance.projectiles.ContainsKey(projectile.gameObject.GetInstanceID()))
	    {
		    data = instance.projectiles[projectile.gameObject.GetInstanceID()];
		    instance.projectiles.Remove(projectile.gameObject.GetInstanceID());

		    IImpactReceiver[] receivers = other.GetComponents<IImpactReceiver>();
		    foreach (IImpactReceiver receiver in receivers)
		    {
			    Vector2 force = data.velocity.normalized * data.force;
			    receiver?.OnImpact(new ImpactInfo(ImpactInfo.DamageType.Gunshot, data.actor, force));
		    }
	    }
	    Destroy(projectile.gameObject);
    }

}
