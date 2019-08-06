using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates physical geometry to cast shadows on the tilemap
public class ShadowCaster : MonoBehaviour
{
	float height = 2f;
	List<GameObject> shadowBlocks;

	// Unity might not include these components if they aren't explicitly referenced somewhere
	private readonly BoxCollider makeSureBoxIsIncludedInBuild;
	private readonly MeshFilter makeSureMeshFilterIncludedInBuild;
	private readonly MeshRenderer makeSureMeshRendererIncludedInBuild;

	[SerializeField] ShadowShape shape;
	[SerializeField] float diameter = 1;

	enum ShadowShape
	{
		Square,
		Round
	}

    // Start is called before the first frame update
    void Start()
    {
		SetUpShadows();
    }
	void SetUpShadows ()
	{
		shadowBlocks = new List<GameObject>();
		List<Vector2Int> relativeShadowLocations;

		// Check for an entity tag so we can cover the entity's entire base
		EntityTag entityTag = GetComponent<EntityTag>();

		if (entityTag != null)
		{
			EntityData entityData = EntityLibrary.GetEntityFromID(entityTag.entityId);
			relativeShadowLocations = entityData.baseShape;
		}
		else
		{
			// If no entity tag was found, assume that this isn't an entity and only cover the origin
			relativeShadowLocations = new List<Vector2Int> { new Vector2Int(0, 0) };
		}

		foreach (Vector2Int pos in relativeShadowLocations)
		{
			GameObject block;
			if (shape == ShadowShape.Square)
			{
				block = GameObject.CreatePrimitive(PrimitiveType.Cube);
				block.transform.position = transform.position;
				block.transform.SetParent(transform);
				block.transform.Translate(new Vector3(pos.x, pos.y, -height / 2));
				block.transform.localScale = new Vector3(diameter, diameter, height);
			}
			else
			{
				block = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				block.transform.position = transform.position;
				block.transform.SetParent(transform);
				block.transform.Rotate(90f, 0f, 0f);
				block.transform.Translate(new Vector3(pos.x, -height, -pos.y));
				block.transform.localScale = new Vector3(diameter, height, diameter);
			}
			block.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			shadowBlocks.Add(block);
		}
	}
    
}
