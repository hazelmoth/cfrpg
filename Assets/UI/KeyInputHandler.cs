using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputHandler : MonoBehaviour {

	public delegate void KeyInputEvent ();
	public delegate void NumInputEvent (int num);
	public static event KeyInputEvent OnInventoryButton;
	public static event KeyInputEvent OnInteractButton;
	public static event KeyInputEvent OnPauseButton;
    public static event KeyInputEvent OnBuildMenuButton;
    public static event NumInputEvent OnHotbarSelect;
  
	const KeyCode PauseButton = KeyCode.Escape; // TODO this for all of them

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (OnInventoryButton != null)
				OnInventoryButton ();
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (OnInteractButton != null)
				OnInteractButton ();
		}
		if (Input.GetKeyDown(PauseButton)) {
			if (OnPauseButton != null)
				OnPauseButton ();
		}
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (OnBuildMenuButton != null)
            {
                OnBuildMenuButton ();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
			if (OnHotbarSelect != null)
				OnHotbarSelect (1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			if (OnHotbarSelect != null)
				OnHotbarSelect (2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			if (OnHotbarSelect != null)
			OnHotbarSelect (3);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			if (OnHotbarSelect != null)
				OnHotbarSelect (4);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			if (OnHotbarSelect != null)
				OnHotbarSelect (5);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6)) {
			if (OnHotbarSelect != null)
				OnHotbarSelect (6);
		}
	}
}
