using System.Collections.Generic;
using UnityEngine;

public class SortOrderController : MonoBehaviour {
	private SpriteRenderer renderer;
	private List<SpriteRenderer> childRenderers = new List<SpriteRenderer> ();
	[SerializeField] private bool raiseByHalfMeter;
	[SerializeField] private List<GameObject> childObjects = new List<GameObject> ();

	// Use this for initialization
	private void Start () {
		renderer = GetComponent<SpriteRenderer> ();
		foreach (GameObject child in childObjects) {
			childRenderers.Add (child.GetComponent<SpriteRenderer> ());
		}
	}
	
	// Update is called once per frame
	private void Update () {
		if (raiseByHalfMeter)
			renderer.sortingOrder = Mathf.RoundToInt ((transform.position.y + 0.5f) * 10) * -1;
		else
			renderer.sortingOrder = Mathf.RoundToInt (transform.position.y * 10) * -1;
		foreach (SpriteRenderer childRenderer in childRenderers) {
			childRenderer.sortingOrder = renderer.sortingOrder;
		}
	}
}
