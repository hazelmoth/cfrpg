using UnityEngine;

/*
 * An IContainerLayoutElement is responsible for rendering a particular container
 * layout element, such as a label.
 */
public interface IContainerLayoutElement
{
	public GameObject Create(out float pivotDelta);
}