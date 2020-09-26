using GUI;
using UnityEngine;

public class DialogueOptionButtonHandler : MonoBehaviour {

	// Use this for initialization
	private void Start () {
		
	}
	
	// Update is called once per frame
	private void Update () {
		
	}

	public void OnClick () {
		FindObjectOfType<DialogueUIManager> ().OnDialogueOptionButton (this.gameObject);
	}
}
