using Dialogue;
using UnityEngine;

/// An object which a non-player actor can stand behind, and a player can interact with,
/// triggering dialogue with the actor.
public class DialogueDesk : NonPlayerWorkstation, IInteractable
{
    private DialogueManager dialogueManager;

    public void OnInteract()
    {
        if (!Occupied) return;
        if (dialogueManager == null) dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null) Debug.LogError("DialogueManager not found in scene.");
        else dialogueManager.InitiateDialogue(CurrentOccupier);
    }
}
