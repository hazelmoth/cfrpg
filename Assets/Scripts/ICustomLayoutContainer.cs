using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomLayoutContainer : IContainer
{
	List<IContainerLayoutElement> GetLayoutElements();
}
