using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActorRace : ScriptableObject
{
	[SerializeField] private string id;
	public string Id => id;

	[SerializeField] private string name;
	public string Name => name;

	[SerializeField] public Vector2 itemPosDown;
	[SerializeField] public Vector2 itemPosUp;
	[SerializeField] public Vector2 itemPosLeft;
	[SerializeField] public Vector2 itemPosRight;

	[SerializeField] private List<Sprite> bodySprites;
	public List<Sprite> BodySprites => bodySprites;

	[SerializeField] private List<Sprite> swooshSprites;
	public List<Sprite> SwooshSprites => swooshSprites;

	public Vector2 GetItemPosition(Direction dir)
	{
		switch (dir)
		{
			case Direction.Down:
				return itemPosDown;
			case Direction.Up:
				return itemPosUp;
			case Direction.Left:
				return itemPosLeft;
			default:
				return itemPosRight;
		}
	}
}
