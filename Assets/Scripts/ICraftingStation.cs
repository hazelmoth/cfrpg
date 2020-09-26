using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICraftingStation
{
	string WorkstationName { get; }
	CraftingEnvironment Environment { get; }
}
