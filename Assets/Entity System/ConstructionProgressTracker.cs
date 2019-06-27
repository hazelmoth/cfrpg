using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionProgressTracker : MonoBehaviour
{
	// All the ingredients that this entity's construction requires, whether or not they've been added
	public List<EntityData.CraftingIngredient> totalIngredientList;
	// What ingredients need to be added to this constructable entity before it is finished
	public List<EntityData.CraftingIngredient> remainingIngredientList;

	public bool IsComplete => GetFractionOfCompleteness() >= 1f;

	public bool AddIngredient (string itemId)
	{
		List<int> markForRemove = new List<int>();
		bool ingredientAddedSuccessfully = false;

		for (int i = 0; i < remainingIngredientList.Count; i++)
		{
			EntityData.CraftingIngredient ingredient = remainingIngredientList[i];
			if (ingredient.quantity > 0)
			{
				if (ingredient.itemId == itemId)
				{
					ingredient.quantity--;
					ingredientAddedSuccessfully = true;
					if (ingredient.quantity < 1)
						markForRemove.Add(i);
					break;
				}
			} else {
				markForRemove.Add(i);
			}
		}
		// remove any ingredients with a quantity of 0
		foreach (int i in markForRemove)
		{
			remainingIngredientList.RemoveAt(i);
		}
		return ingredientAddedSuccessfully;
	}

	public float GetFractionOfCompleteness()
	{
		float IngredientCountInList(List<EntityData.CraftingIngredient> list)
		{
			int total = 0;
			foreach (EntityData.CraftingIngredient ingred in list)
			{
				total += ingred.quantity;
			}
			return total;
		}

		return (IngredientCountInList(remainingIngredientList) / IngredientCountInList(totalIngredientList));
		
	}
}
