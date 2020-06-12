﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
	[CreateAssetMenu(fileName = "NewGun", menuName = "Items/Gun", order = 1)]
	public class Gun : ItemData, IGun, IPointable
	{
		[SerializeField] private Sprite gunSprite;
		[SerializeField] private Direction gunSpritePointDirection = Direction.Right;
		[SerializeField] private Sprite projectile;
		[SerializeField] private Vector2 projectileOffset;
		[SerializeField] private float damage = 10;
		[SerializeField] private float velocity = 10;
		[SerializeField] private float range = 10;
		[SerializeField] private float fireRate = 3;
		[SerializeField] private float spread = 10;
		[SerializeField] private float projectileRadius = 0.03f;
		[SerializeField] private bool automatic = false;

		Sprite IGun.Projectile => projectile;
		Vector2 IGun.ProjectileOffset => projectileOffset;
		float IGun.Damage => damage;
		float IGun.Velocity => velocity;
		float IGun.Range => range;
		float IGun.FireRate => fireRate;
		float IGun.Spread => spread;
		float IGun.ProjectileRadius => projectileRadius;
		bool IGun.Automatic => automatic;
		Sprite IPointable.heldItemSprite => gunSprite;
		Direction IPointable.pointDirection => gunSpritePointDirection;
	}
}