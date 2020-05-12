﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A component for detecting when things get PUNCHED!.
public interface IPunchReceiver
{

	void OnPunch(float strength, Vector2 direction);
}