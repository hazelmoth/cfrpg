using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{

	// For displaying items that can't actually be dragged, as in the hotbar
	public class InventoryIcon : MonoBehaviour
	{

		[SerializeField] protected TextMeshProUGUI quantityText;
		protected Image renderer;
		protected bool visible = true;

		private void Start()
		{
			renderer = GetComponent<Image>();
		}

		public void SetVisible(bool visible)
		{
			if (renderer == null)
				renderer = GetComponent<Image>();
			if (visible)
			{
				gameObject.SetActive(true);
				renderer.color = Color.white;
			}
			else
			{
				if (renderer == null)
					return;
				renderer.color = Color.clear;
			}

			this.visible = visible;
		}

		public void SetQuantityText(string text)
		{
			quantityText.text = text;
		}
	}
}