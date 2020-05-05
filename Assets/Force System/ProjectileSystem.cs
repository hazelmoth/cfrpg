using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
	private static ProjectileSystem instance;
	private IDictionary<int, ProjectileData> projectiles;
	private GameObject projectileParent;
	private const float MaxProjectileLifetime = 30;

	private struct ProjectileData
	{
		public ProjectileData(Projectile projectileObject, Vector2 velocity, float force, float range)
		{
			this.projectileObject = projectileObject;
			this.velocity = velocity;
			this.force = force;
			this.startTIme = Time.time;
			this.maxLifetime = Mathf.Min(MaxProjectileLifetime, range / this.velocity.magnitude);
		}
		public readonly Projectile projectileObject;
		public readonly Vector2 velocity;
		public readonly float force;
		public readonly float startTIme;
		public readonly float maxLifetime;
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

    public static void LaunchProjectile(Sprite sprite, Vector2 origin, float angle, float speed, float force, float maxRange, float colliderRadius, Collider2D ignoredCollider, bool flipProjectileSprite)
    {
	    if (instance.projectileParent == null)
	    {
			instance.projectileParent = new GameObject("Projectiles");
	    }

		GameObject projectile = new GameObject("Projectile");
		Projectile projectileComp = projectile.AddComponent<Projectile>();
		
		projectile.transform.SetParent(instance.projectileParent.transform);
		projectile.transform.position = origin;
		projectile.transform.Rotate(Vector3.forward, angle);

		SpriteRenderer spriteRenderer = projectile.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		if (flipProjectileSprite)
		{
			spriteRenderer.flipY = true;
		}

		CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
		collider.isTrigger = true;
		collider.radius = colliderRadius;

		Rigidbody2D rigidbody = projectile.AddComponent<Rigidbody2D>();
		rigidbody.isKinematic = true;

		Vector2 velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * speed, Mathf.Sin(angle * Mathf.Deg2Rad) * speed);
		ProjectileData projectileData = new ProjectileData(projectileComp, velocity, force, maxRange);

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

		    IPunchReceiver[] receivers = other.GetComponents<IPunchReceiver>();
		    foreach (IPunchReceiver receiver in receivers)
		    {
			    receiver?.OnPunch(data.force, data.velocity);
		    }
	    }
	    Destroy(projectile.gameObject);
    }

}
