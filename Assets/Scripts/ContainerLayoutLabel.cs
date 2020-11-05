using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerLayoutLabel : IContainerLayoutElement
{
    public string text;

    public ContainerLayoutLabel(string text)
    {
        this.text = text;
    }
}
