using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Items/Gun", order = 1)]
public class Gun : ItemData
{
	public Sprite gunSprite;
	public Sprite projectile;
	public Vector2 projectileOffset;
	public float damage = 10;
	public float velocity = 10;
	public float range = 10;
	public float fireRate = 3;
	public float spread = 10;
	public float projectileRadius = 0.03f;
	public bool automatic = false;
}
