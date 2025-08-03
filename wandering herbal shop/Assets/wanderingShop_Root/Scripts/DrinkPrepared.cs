using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkPrepared : MonoBehaviour
{
    //public List<int> ingredients = new List<int>();
    public int ingredient = -1;
    public bool isFinished;
    [SerializeField] bool isEmpty;
    
    public void NewIngredient(int ingredientNumber)
    {
        ingredient = ingredientNumber;
        isEmpty = false;
        Debug.Log("New inredient!");
    }
    public void FinishDrink()
    {
        if(!isEmpty) isFinished = true;
        else isFinished = false;
        Debug.Log("Drink finished!");
    }
}
