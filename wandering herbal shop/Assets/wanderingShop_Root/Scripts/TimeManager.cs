using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("TimeManager is null!");
            }
            return instance;
        }
    }
    
    [Header("Time Parameters")]
    [SerializeField] float seconds;
    public int minutes;
    public int hours; //despiertas 9/10, trabajas 11h a 20h, duermes a 21h SOLO PUEDES GUARDAR DE DIA EN DIA, SI TE VAS A MITAD DE UN DÍA NO SE GUARDARÁ EL PROGRESO
    //QUIZÁ PODRÍA HACER LOS DÍAS COMO 3 VECES MÁS LARGOS QUE EN GOOD PIZZA GREAT PIZZA Y PONER BREAKS PARA COMER Y VER A ALGÚN NPC POR AHÍ (EN VERSIÓN MÁS EXTENDIDA!)
    public int day;

    [Header("NPC Parameters")]
    public int npcAtShop; // int?
    public bool firstNPC;
    [SerializeField] GameObject[] npcPrefabs;
    [SerializeField] List<NPC> npcs = new List<NPC>();
    [SerializeField] bool npcSent;
    [SerializeField] int index;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    void Update()
    {
        ///*
        if(npcAtShop < 2 && firstNPC)
        {
            StartCoroutine (NewNPC());
        }
        if(index== 9 && npcAtShop < 1 && !npcSent) //ahora mismo va mal
        {
            int randomNPC;
            firstNPC = false;
             randomNPC = Random.Range(0, npcs.Count);
            npcs[randomNPC].served = false;
            npcs[randomNPC].arrived = false;
        }

        //*/

    }
    IEnumerator NewNPC()
    {
        firstNPC = false;
        //GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)]); //o si eso por orden para que no haya dos iguales; nada más se instancia, necesita ir a la tienda
        npcs[index].gameObject.SetActive(true);
        index++;
        //npcAtShop++;
        yield return new WaitForSeconds(15);
        firstNPC = true;
    }
    IEnumerator ResendNPC()
    {
        npcSent = true;
        int randomNPC;
        firstNPC = false;
        randomNPC = Random.Range(0, npcs.Count);
        npcs[randomNPC].served = false;
        npcs[randomNPC].arrived = false;
        yield return new WaitForSeconds(20);
        npcSent = false;
    }
}
