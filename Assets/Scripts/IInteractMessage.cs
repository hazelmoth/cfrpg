using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents objects that offer a message to display when interaction with them is available.
public interface IInteractMessage
{
    string GetInteractMessage();
}
