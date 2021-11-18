using System;
using System.Collections.Generic;
using GUI;
using GUI.ContainerLayoutElements;

public interface ICustomLayoutContainer : IContainer
{
	List<IContainerLayoutElement> GetLayoutElements();

	/// Registers the given listener to be notified when the container's contents change.
	void SetUpdateListener(Action<IContainer> listener);
}
