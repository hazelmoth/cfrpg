using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_hair", menuName = "Hair Asset")]
public class Hair : ScriptableObject
{
    public string hairId = "new_hair";
    public string hairName = "New Hair";
    public Sprite[] sprites;
}
