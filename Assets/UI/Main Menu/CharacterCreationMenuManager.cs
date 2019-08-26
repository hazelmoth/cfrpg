using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterCreationMenuManager : MonoBehaviour
{
	[SerializeField] bool allowNoShirt = true;
	[SerializeField] bool allowNoPants = true;
	const string noShirtText = "None";
	const string noPantsText = "None";

	[SerializeField] MainMenuManager menuManager;

	[SerializeField] List<Hair> startHairs;
	[SerializeField] List<Shirt> startShirts;
	[SerializeField] List<Pants> startPants;

	[SerializeField] Image hairImage;
	[SerializeField] Image shirtImage;
	[SerializeField] Image pantsImage;
	[SerializeField] TextMeshProUGUI hairText;
	[SerializeField] TextMeshProUGUI shirtText;
	[SerializeField] TextMeshProUGUI pantsText;
	[SerializeField] TMP_InputField nameInput;

	

	int currentHairIndex = 0;
	int currentShirtIndex = 0;
	int currentPantsIndex = 0;


	// Start is called before the first frame update
	void Start()
    {
		// We need a way to load the content libraries when the game is launched, not in the main scene
		//startHairs = HairLibrary.GetHairs();

		if (allowNoShirt)
		{
			startShirts.Add(null);
		}
		if (allowNoPants)
		{
			startPants.Add(null);
		}

		UpdateCharacterDisplay();
    }
	void FinishCreation ()
	{
		PlayerCharData data = new PlayerCharData();
		data.playerName = nameInput.text;
		data.saveId = nameInput.text; // TODO create new ID if name is already used
		data.hairId = startHairs[currentHairIndex].hairId;
		data.inventory = new SerializableActorInv();
		data.inventory.shirt = startShirts[currentShirtIndex] != null ? startShirts[currentShirtIndex].ItemId : null;
		data.inventory.pants = startPants[currentPantsIndex] != null ? startPants[currentPantsIndex].ItemId : null;
		// TODO naturally spawn the player somewhere the first time the game is loaded
		GameDataMaster.PlayerToLoad = new SavedPlayerChar(data, Vector2.zero, Direction.Down, SceneObjectManager.WorldSceneId);
	}

	void UpdateCharacterDisplay ()
	{
		Mathf.Clamp(currentHairIndex, 0, startHairs.Count - 1);
		Mathf.Clamp(currentShirtIndex, 0, startShirts.Count - 1);
		Mathf.Clamp(currentPantsIndex, 0, startPants.Count - 1);
		Hair hair = startHairs[currentHairIndex];
		Shirt shirt = startShirts[currentShirtIndex];
		Pants pants = startPants[currentPantsIndex];
		hairImage.sprite = hair.sprites[0];
		hairText.text = hair.hairName;

		if (shirt == null)
		{
			shirtImage.color = Color.clear;
			shirtImage.sprite = null;
			shirtText.text = noShirtText;
		}
		else
		{
			shirtImage.color = Color.white;
			shirtImage.sprite = shirt.GetShirtSprites()[0];
			shirtText.text = shirt.ItemName;
		}
		if (pants == null)
		{
			pantsImage.color = Color.clear;
			pantsImage.sprite = null;
			pantsText.text = noPantsText;
		}
		else
		{
			pantsImage.color = Color.white;
			pantsImage.sprite = pants.GetPantsSprites()[0];
			pantsText.text = pants.ItemName;
		}	
	}
	public void OnFinishButton ()
	{
		FinishCreation();
		SceneManager.LoadScene((int)UnityScenes.WorldGeneration, LoadSceneMode.Single);
}

	public void OnHairForwardButton ()
	{
		currentHairIndex++;
		currentHairIndex = currentHairIndex % startHairs.Count;
		UpdateCharacterDisplay();
	}
	public void OnHairBackButton ()
	{
		currentHairIndex--;
		if (currentHairIndex < 0)
			currentHairIndex = startHairs.Count - 1;
		UpdateCharacterDisplay();
	}
	public void OnShirtForwardButton ()
	{
		currentShirtIndex++;
		currentShirtIndex = currentShirtIndex % startShirts.Count;
		UpdateCharacterDisplay();
	}
	public void OnShirtBackButton()
	{
		currentShirtIndex--;
		if (currentShirtIndex < 0)
			currentShirtIndex = startShirts.Count - 1;
		UpdateCharacterDisplay();
	}
	public void OnPantsForwardButton()
	{
		currentPantsIndex++;
		currentPantsIndex = currentPantsIndex % startPants.Count;
		UpdateCharacterDisplay();
	}
	public void OnPantsBackButton()
	{
		currentPantsIndex--;
		if (currentPantsIndex < 0)
			currentPantsIndex = startPants.Count - 1;
		UpdateCharacterDisplay();
	}
}
