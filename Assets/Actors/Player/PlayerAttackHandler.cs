﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
	Actor actor;
	ActorPunchExecutor puncher;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (actor == null)
				actor = GetComponent<Actor>();

			ActorInventory inv = actor.Inventory;

			if (inv.GetEquippedItem() != null)
			{
				SwingableItem equippedSwingable = inv.GetEquippedItem() as SwingableItem;
				if (equippedSwingable != null)
				{
					equippedSwingable.Swing(actor);
					return;
				}
			}
			else
			{
				// If no item is equipped, throw a punch instead
				if (puncher == null)
				{
					puncher = GetComponent<ActorPunchExecutor>();
					if (puncher == null)
						puncher = gameObject.AddComponent<ActorPunchExecutor>();
				}

				puncher.InitiatePunch(actor.Direction.ToVector2());
			}
		}
    }
}
