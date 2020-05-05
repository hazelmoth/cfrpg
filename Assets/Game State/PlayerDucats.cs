using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the player's ducat balance
public class PlayerDucats : MonoBehaviour {

	public delegate void PlayerDucatEvent (int ducats);
	public static event PlayerDucatEvent BalanceChanged;

	private void OnDestroy ()
	{
		BalanceChanged = null;
	}

	private static int ducatBalance;
	public static int DucatBalance {
		get {
			return ducatBalance;
		}
	}
	public static void AddDucats (int amount) {
		ducatBalance += amount;
		if (BalanceChanged != null)
			BalanceChanged (ducatBalance);
	}
	public static void SetDucatBalance (int amount) {
		ducatBalance = amount;
		if (BalanceChanged != null)
			BalanceChanged (ducatBalance);
	}
}
