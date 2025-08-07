using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Unity.VisualScripting;

public class DrinkPrepared : MonoBehaviour
{
    //camara mirar a la cara rotacion en x: 11


    //public List<int> ingredients = new List<int>();
    public int ingredient = -1;
    public int waterAdded = 0;
    public bool isFinished;
    [SerializeField] bool isEmpty;
    [SerializeField] GameObject[] mugsInShop = new GameObject[5];
    [SerializeField] int mugsAvailable;
    [SerializeField] int mugsInBox;
    [SerializeField] GameObject mugBox = null; //añadir tmb otras tazas
    [SerializeField] GameObject mugPrefab; //añadir tmb otras tazas
    [SerializeField] GameObject actualMug = null; //añadir tmb otras tazas
    [SerializeField] GameObject actualNPC = null; //añadir tmb otras tazas
    [SerializeField] Transform mugSpawner; //añadir tmb otras tazas
    public bool mugInTable; //añadir tmb otras tazas
    CameraChange cameraScript;
    

    SpriteRenderer[] childrenSprites;
    [SerializeField] TMP_Text replenishText; //añadir tmb otras tazas
    [SerializeField] bool extraMugs;//hacer que sea int y las puedas poner poco a poco?

    [SerializeField] Button[] herbsButton = new Button[9]; //en total: menta, dandelion, manzanilla, tomillo, hinojo, colaCaballo, lavanda, tila,  te, frutosBosque
    [SerializeField] int[] herbActualOrder = {-1, -1, -1, -1, -1, -1, -1, -1, -1}; //en el orden en el que lleguen a botones/cuales estén libres SI ESTAN VACIOS -1
    //refe a cada boton: cada uno accede a sus hijos imagen y les activa el componente imagen con el sprite correspondiente
    //cada vez que empieces nuevo día o entres al juego, que sse mantengan los que estaban

    //32.15 rotacion x ingredientes preparar, 5 rotacion x to hear out
    private void Start()
    {
        cameraScript = GetComponent<CameraChange>();
        for (int i = 0; i < herbsButton.Length; i++) // cambiar esto para que checkee en los datos de guardado cuantas hierbas hay ed cada, y no simmplemente restockee las 5
        {
            int herbIndex = herbActualOrder[i];
            if (herbsButton[i].GetComponent<herbsInButton>().spritesHerbs != null)
            { //poner aqui otro bucle para cada image??
                childrenSprites = herbsButton[i].GetComponent<herbsInButton>().spritesHerbs;// cambiar esto a modelos en 3d para que quede mejor al mover camara
                foreach (SpriteRenderer sprite in childrenSprites )
                {
                    if (herbActualOrder[i] != -1) // si no está vacío
                    {
                        
                        sprite.enabled = true;
                        sprite.sprite = GameManager.Instance.herbs[herbActualOrder[i]];
                       
                    }
                }
                herbsButton[i].onClick.RemoveAllListeners();
                herbsButton[i].onClick.AddListener(() => NewIngredient(herbIndex));
            }
        }
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
        if (mugInTable)
        {
            if(herbActualOrder.Contains(ingredientNumber)) //esto si solo puedes poner 1 vez cada planta a la VEZ
            {
                int buttonNumber = System.Array.IndexOf(herbActualOrder, ingredientNumber);
                childrenSprites = null;
                herbsInButton herbButtonControl = herbsButton[buttonNumber].GetComponent<herbsInButton>();
                childrenSprites = herbsButton[buttonNumber].GetComponent<herbsInButton>().spritesHerbs; // cambiar esto a modelos en 3d para que quede mejor al mover camara
                int enabledHerbs = 0;
                for (int i = 0;i < childrenSprites.Length;i++)
                {
                    if (childrenSprites[i].gameObject != herbsButton[buttonNumber].gameObject) //para que no se coja a si mismo
                    {
                        if (childrenSprites[i].enabled)
                        {
                            enabledHerbs++;
                        }
                    }
                }
                Debug.Log("Quedan todavía " + enabledHerbs + " trozos de la planta id " + ingredientNumber);
                for (int i = 0; i < childrenSprites.Length; i++)
                {
                    if (childrenSprites[i].gameObject != herbsButton[buttonNumber].gameObject)
                    {
                        if (childrenSprites[i].enabled)
                        {
                            ingredient = ingredientNumber;
                            isEmpty = false;
                            Debug.Log("Ingredient added!");
                            if (enabledHerbs == 1)
                            {
                                herbsButton[buttonNumber].onClick.RemoveAllListeners();
                                herbActualOrder[buttonNumber] = -1;
                                Debug.Log("Botón apagado por falta de hierbas!");
                            }
                            //StartCoroutine(AddIngredientAnim(childrenSprites[i]));
                            //childrenSprites[i].enabled = false;
                            childrenSprites[i].GetComponent<herbSelectedAnim>().ingredientAnimNow = true;
                            break;
                        }
                    }
                }
            }
            /*
            1. buscar ingredientNumber en el orden de los ingredientes en los botones
            2. acceder a ese botón y a sus hijos con componente image
            3. SI NO ESTÁN TODOS APAGADOS: apagar un hijo que esté activado (a ser posible el que esté visiblemente más arriba)
            4. EN EL MOMENTO EN EL QUE SOLO QUEDE 1 Y LO GASTES, EL NÚMERO DE HERBACTUALORDER DE ESE BOTÓN PASA A -1 Y SE APAGA EL BOTÓN HASTA QUE PONGAS MÁS HIERBAS
            */
        }
        else Debug.Log("You need a mug!");
        
    }
    public void AddWater()
    {
        //si el aparato no está roto,tiene batería y tiene suficiente agua
        if (mugInTable)
        {
            waterAdded++;
            actualMug.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void FinishDrink()
    {
        GameObject lastNPC = actualNPC;
        if (!isEmpty && waterAdded > 0)
        {
            isFinished = true;
            Debug.Log("Drink finished!");
            if (actualMug != null)
            {
                actualMug.transform.parent = lastNPC.transform;//si vienen muy juntos, se queda la taza el otro
                                                               //reset:
                StartCoroutine(ResetIngredients());
            }
            cameraScript.rotate = true;
            //la camara vuelve a rotar
        }
        else isFinished = false;
    }
    IEnumerator ResetIngredients()
    {
        yield return new WaitForSeconds(0.5f);
        mugInTable = false;
        isEmpty = true;
        isFinished = false;
        waterAdded = 0;
        
        Debug.Log("Reset acabado");
        StopAllCoroutines();
    }
    public void UseMugs()
    {
        Debug.Log("You are pressing the button");
        //cuenta las tazas que hay
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
                replenishText.text = "";
                for (int i = 0; i < mugsInShop.Length; i++)
                {
                    mugsInShop[i].SetActive(true);
                }
                extraMugs = false;
                mugBox.SetActive(false);
                replenishText.text = "";
                mugBox = null;
                mugsAvailable = 5;
            }
            else Debug.Log("Consigue más tazas!");
            
        }
        else
        {
            if (!mugInTable)
            {
                for (int i = 0; i < mugsInShop.Length; i++)
                {
                    if (mugsInShop[i].activeSelf)
                    {
                        mugsAvailable--;
                        mugsInShop[i].SetActive(false);
                        actualMug = Instantiate(mugPrefab);
                        actualMug.transform.position = mugSpawner.position;
                        actualMug.transform.rotation = mugSpawner.rotation;
                        mugInTable = true;
                        //ponerla en position 0.147 , 0.254 , 0.057; rotacion Y 181.65; solo dejar que saque una taza mientras haya una fuera
                        break;
                    }
                }
                Debug.Log("Mugs now available: " + mugsAvailable);
            }
            else Debug.Log("Look! You already have a mug!");
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
        if(other.CompareTag("NPC"))
        {
            actualNPC = other.gameObject;
            TimeManager.Instance.npcAtShop++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "mugBox")
        {
            mugBox = null;
            extraMugs = false;
        }
        if (other.CompareTag("NPC"))
        {
            actualNPC = null;
            TimeManager.Instance.npcAtShop -= 1;
            // no va :( , lo pongo en reset TimeManager.Instance.npcAtShop -= 1; //no va??
            //TimeManager.Instance.firstNPC = true;
        }
    }
}
