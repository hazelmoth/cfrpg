using ContentLibraries;
using UnityEngine;

[CreateAssetMenu(fileName = "new_hair", menuName = "Hair Asset")]
public class Hair : ScriptableObject, IContentItem
{
    public string hairId = "new_hair";
    public string hairName = "New Hair";
    public Sprite[] sprites;

    public string Id => hairId;
}
