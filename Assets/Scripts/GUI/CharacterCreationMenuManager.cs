using System.Collections.Generic;
using System.Linq;
using ContentLibraries;
using Items;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace GUI
{
	public class CharacterCreationMenuManager : MonoBehaviour
	{
		private const bool AllowNoShirt = true;
		private const bool AllowNoPants = true;
		private const string NoShirtText = "None";
		private const string NoPantsText = "None";

		[SerializeField] private MainMenuManager menuManager;

		[SerializeField] private List<Hair> startHairs;
		[SerializeField] private List<ItemData> startShirts;
		[SerializeField] private List<ItemData> startPants;

		[SerializeField] private Image hairImage = null;
		[SerializeField] private Image shirtImage = null;
		[SerializeField] private Image pantsImage = null;
		[SerializeField] private TextMeshProUGUI hairText = null;
		[SerializeField] private TextMeshProUGUI shirtText = null;
		[SerializeField] private TextMeshProUGUI pantsText = null;
		[SerializeField] private TMP_InputField nameInput = null;


		private int currentHairIndex = 0;
		private int currentShirtIndex = 0;
		private int currentPantsIndex = 0;


		// Start is called before the first frame update
		private void Start()
		{
			startHairs = ContentLibrary.Instance.Hairs.GetAll().ToList();
			startShirts = new List<ItemData>();
			startPants = ContentLibrary.Instance.Items.GetAll<Pants>();

			if (AllowNoShirt)
			{
				startShirts.Add(null);
			}
			if (AllowNoPants)
			{
				startPants.Add(null);
			}

			UpdateCharacterDisplay();
		}

		private void FinishCreation()
		{
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
			
			float maxHealth = ContentLibrary.Instance.Races.Get(raceId).MaxHealth;
			ActorData data = new ActorData(saveId,
				playerName,
				personality,
				raceId,
				Gender.Male,
				hairId,
				new ActorHealth(maxHealth, maxHealth),
				inventory,
				0,
				new FactionStatus(null),
				null);
			SaveInfo.NewlyCreatedPlayer = data;
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
				shirtText.text = NoShirtText;
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
				pantsText.text = NoPantsText;
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