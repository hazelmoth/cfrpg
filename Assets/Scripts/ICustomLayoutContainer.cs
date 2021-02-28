using System;
using System.Collections.Generic;

public interface ICustomLayoutContainer : IContainer
{
	List<IContainerLayoutElement> GetLayoutElements();
	void SetUpdateListener(Action<IContainer> listener);
}
