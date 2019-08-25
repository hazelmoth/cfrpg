﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedNpc
{
	public string scene;
	public Vector2 location;
	public Direction direction;
	public SerializableNpcData data;

	public SavedNpc (NPC sourceNpc)
	{
		scene = sourceNpc.ActorCurrentScene;
		location = TilemapInterface.WorldPosToScenePos(sourceNpc.transform.position, sourceNpc.ActorCurrentScene);
		direction = sourceNpc.Direction;
		data = new SerializableNpcData(NPCDataMaster.GetNpcFromId(sourceNpc.NpcId), sourceNpc.Inventory.GetContents());
	}
}