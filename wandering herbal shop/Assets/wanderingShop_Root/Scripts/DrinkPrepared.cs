using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrinkPrepared : MonoBehaviour
{
    //public List<int> ingredients = new List<int>();
    public int ingredient = -1;
    public bool isFinished;
    [SerializeField] bool isEmpty;
    [SerializeField] GameObject[] mugsInShop = new GameObject[5];
    [SerializeField] int mugsAvailable;
    [SerializeField] int mugsInBox;
    [SerializeField] GameObject mugBox = null; //añadir tmb otras tazas
    [SerializeField] TMP_Text replenishText; //añadir tmb otras tazas
    [SerializeField] bool extraMugs;//hacer que sea int y las puedas poner poco a poco?

    private void Start()
    {
        mugsAvailable = 0;
        for (int i = 0; i < mugsInShop.Length; i++)
        {
            if (mugsInShop[i].activeSelf) mugsAvailable++;
        }
        if(mugsAvailable == 0)
        {
            replenishText.text = "touch to replenish\r\n(if you have mugs)";
        }
        else replenishText.text = "";
    }
    public void NewIngredient(int ingredientNumber)
    {
        //if hay una taza fuera
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
    public void UseMugs()
    {
        Debug.Log("You are pressing the button");
        mugsAvailable = 0;
        for (int i = 0; i < mugsInShop.Length; i++)
        {
            if (mugsInShop[i].activeSelf) mugsAvailable++;
        }
        
        if(mugsAvailable <= 0)
        {
            if (mugBox != null)
            {
                //actualizar esto para que se puedan poner de una en una, pero mientras:
                for (int i = 0; i < mugsInShop.Length; i++)
                {
                    mugsInShop[i].SetActive(true);
                }
                extraMugs = false;
                mugBox.SetActive(false);
                replenishText.text = "";
                mugBox = null;
            }
            else Debug.Log("Consigue más tazas!");
            
        }
        else
        {
            for (int i = 0; i < mugsInShop.Length; i++)
            {
                if (mugsInShop[i].activeSelf)
                {
                    mugsAvailable--;
                    mugsInShop[i].SetActive(false);
                    //ponerla en position 0.147 , 0.254 , 0.057; rotacion Y 181.65; solo dejar que saque una taza mientras haya una fuera
                    break;
                }
            }
            Debug.Log("Mugs now available: " + mugsAvailable);
        }
        if(mugsAvailable == 0)
        {
            replenishText.text = "touch to replenish\r\n(if you have mugs)";
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "mugBox")
        {
            mugBox = other.gameObject;
            extraMugs = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "mugBox")
        {
            mugBox = null;
            extraMugs = false;
        }
    }
}
