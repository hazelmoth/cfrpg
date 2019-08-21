using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadWorldMenuManager : MonoBehaviour
{
	[SerializeField] GameObject listItemPrefab;
	[SerializeField] GameObject worldListContent;

	WorldListItem currentSelected;

	public void PopulateWorldList ()
	{
		// TODO get all available saves
		List<WorldSave> saves = new List<WorldSave>();
		PopulateWorldList(saves);
	}
	public void PopulateWorldList (List<WorldSave> saves)
	{
		currentSelected = null;
		ClearWorldList();
		foreach (WorldSave save in saves)
		{
			GameObject listItemObject = Instantiate(listItemPrefab);
			WorldListItem listItem = listItemObject.GetComponent<WorldListItem>();
			string worldName = save.ToString();
			string subText = "this sure is a save";
			listItem.save = save; // TODO save IDs
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
		//TODO actual world loading
		SceneManager.LoadScene(2, LoadSceneMode.Single);
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
