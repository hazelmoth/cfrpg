using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWorldMenuManager : MonoBehaviour
{
	[SerializeField] private GameObject listItemPrefab = null;
    [SerializeField] private GameObject worldListContent = null;

    public static LoadWorldMenuManager instance;

    private WorldListItem currentSelected;

    private void Start()
    {
        instance = this;
    }
    public void PopulateWorldList ()
    {
        List<WorldSave> saves = SaveReader.GetAllSaves();
		PopulateWorldList(saves);
	}
	public void PopulateWorldList (List<WorldSave> saves)
	{
		currentSelected = null;
		ClearWorldList();
		foreach (WorldSave save in saves)
		{
			GameObject listItemObject = Instantiate(listItemPrefab, worldListContent.transform, false);
			WorldListItem listItem = listItemObject.GetComponent<WorldListItem>();
			string worldName = save.worldName;
			string subText = "this sure is a save";
			listItem.save = save; // TODO use save IDs
			listItem.SetText(worldName, subText);
			listItem.SetHighlighted(false);
		}
	}
	public void ClearWorldList ()
	{
		currentSelected = null;
		foreach (Transform child in worldListContent.transform)
		{
			Destroy(child.gameObject);
		}
	}
	public void OnLoadWorldButton ()
	{
		if (currentSelected == null)
		{
			return;
		}
		SaveInfo.SaveFileId = currentSelected.save.saveFileId;
		SaveInfo.SaveToLoad = currentSelected.save;

		SceneManager.LoadScene((int)UnityScenes.Main, LoadSceneMode.Single);
	}
	public void OnDeleteButton ()
	{

	}
	public void OnExitButton ()
	{

	}

	public void OnListItemSelected (WorldListItem item)
	{
		currentSelected?.SetHighlighted(false);
		currentSelected = item;
		currentSelected.SetHighlighted(true);
	}
}
