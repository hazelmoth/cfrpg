using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEatingManager : MonoBehaviour
{
    public delegate void EatEvent(Item item);
    public event EatEvent OnItemEaten;

    private ActorPhysicalCondition physCondition;
    // Start is called before the first frame update
    void Start()
    {
        physCondition = this.GetComponent<ActorPhysicalCondition>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool AttemptEat (Item item)
    {
        if (item == null)
        {
            return false;
        }
        Eat(item);
        return true;
    }

    void Eat (Item item)
    {
        if (physCondition == null)
        {
            physCondition = this.GetComponent<ActorPhysicalCondition>();
        }
        if (physCondition == null)
        {
            return;
        }

        physCondition.IntakeNutrition(item.NutritionalValue);
        if (OnItemEaten != null)
        {
            OnItemEaten(item);
        }
    }
}
