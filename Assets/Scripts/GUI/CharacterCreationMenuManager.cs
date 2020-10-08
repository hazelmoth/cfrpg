using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace GUI
{
	public class CharacterCreationMenuManager : MonoBehaviour
	{
		[SerializeField] private bool allowNoShirt = true;
		[SerializeField] private bool allowNoPants = true;
		private const string noShirtText = "None";
		private const string noPantsText = "None";

		[SerializeField] private MainMenuManager menuManager;

		[SerializeField] private List<Hair> startHairs;
		[SerializeField] private List<ItemData> startShirts;
		[SerializeField] private List<ItemData> startPants;

		[SerializeField] private Image hairImage;
		[SerializeField] private Image shirtImage;
		[SerializeField] private Image pantsImage;
		[SerializeField] private TextMeshProUGUI hairText;
		[SerializeField] private TextMeshProUGUI shirtText;
		[SerializeField] private TextMeshProUGUI pantsText;
		[SerializeField] private TMP_InputField nameInput;


		private int currentHairIndex = 0;
		private int currentShirtIndex = 0;
		private int currentPantsIndex = 0;


		// Start is called before the first frame update
		private void Start()
		{
			startHairs = ContentLibrary.Instance.Hairs.GetHairs();
			startShirts = ContentLibrary.Instance.Items.GetAll<Shirt>();
			startPants = ContentLibrary.Instance.Items.GetAll<Pants>();

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

		private void FinishCreation()
		{
			string name = nameInput.text;
			if (nameInput.text == null)
			{
				name = "";
			}
			string playerName = nameInput.text;
			string saveId = playerName.Replace(" ", "_").ToLower();
			string hairId = startHairs[currentHairIndex].hairId;
			string raceId = "human_light";
			string personality = "western";
			ActorInventory.InvContents inventory = new ActorInventory.InvContents();

			if (startShirts[currentShirtIndex] != null)
			{
				inventory.equippedShirt = new ItemStack(startShirts[currentShirtIndex]);
			}
			if (startPants[currentPantsIndex] != null)
			{
				inventory.equippedPants = new ItemStack(startPants[currentPantsIndex]);
			}

			// TODO naturally spawn the player somewhere the first time the game is loaded
			Vector2 playerSpawn = new Vector2(50, 50);
			ActorData data = new ActorData(saveId, playerName, personality, raceId, Gender.Male, hairId, new ActorPhysicalCondition(), inventory, 0, new FactionStatus(null), new List<object>());
			GameDataMaster.NewlyCreatedPlayer = data;
		}

		private void UpdateCharacterDisplay()
		{
			Mathf.Clamp(currentHairIndex, 0, startHairs.Count - 1);
			Mathf.Clamp(currentShirtIndex, 0, startShirts.Count - 1);
			Mathf.Clamp(currentPantsIndex, 0, startPants.Count - 1);
			Hair hair = startHairs[currentHairIndex];
			Shirt shirt = startShirts[currentShirtIndex] as Shirt;
			Pants pants = startPants[currentPantsIndex] as Pants;
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
				shirtText.text = shirt.DefaultName;
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
				pantsText.text = pants.DefaultName;
			}
		}
		public void OnFinishButton()
		{
			FinishCreation();
			SceneManager.LoadScene((int)UnityScenes.WorldGeneration, LoadSceneMode.Single);
		}

		public void OnHairForwardButton()
		{
			currentHairIndex++;
			currentHairIndex = currentHairIndex % startHairs.Count;
			UpdateCharacterDisplay();
		}
		public void OnHairBackButton()
		{
			currentHairIndex--;
			if (currentHairIndex < 0)
				currentHairIndex = startHairs.Count - 1;
			UpdateCharacterDisplay();
		}
		public void OnShirtForwardButton()
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
}