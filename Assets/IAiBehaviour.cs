using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAiBehaviour
{
	void Execute();
	void Cancel();
	bool IsRunning { get; }
}
