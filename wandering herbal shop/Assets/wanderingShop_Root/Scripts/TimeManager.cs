using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] bool timeCanpass;
    [SerializeField] float seconds;
    public int minutes;
    public int hours; //despiertas 9/10, trabajas 11h a 20h, duermes a 21h SOLO PUEDES GUARDAR DE DIA EN DIA, SI TE VAS A MITAD DE UN DÍA NO SE GUARDARÁ EL PROGRESO
    //QUIZÁ PODRÍA HACER LOS DÍAS COMO 3 VECES MÁS LARGOS QUE EN GOOD PIZZA GREAT PIZZA Y PONER BREAKS PARA COMER Y VER A ALGÚN NPC POR AHÍ (EN VERSIÓN MÁS EXTENDIDA!)
    public int day = 1;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text dateText;

    [Header("NPC Parameters")]
    public int npcAtShop; // int?
    public bool firstNPC;
    [SerializeField] GameObject[] npcPrefabs;
    [SerializeField] List<NPC> npcs = new List<NPC>();
    [SerializeField] bool npcSent;
    [SerializeField] int index;
    [SerializeField] float timeBetweenCustomers = 5;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    void Update()
    {
        //Gestión TIEMPO
        if(timeCanpass)
        {
            seconds += Time.deltaTime * 154.3f;
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes); //hacer como en great pizza, que solo salen los minutos de 15 en 15 o quizá de 5 en 5
            dateText.text = string.Format("day " + day);
            if (seconds > 60)
            {
                minutes++;
                seconds -= 60;
            }
            if (minutes > 60)
            {
                hours++;
                minutes -= 60;
            }
            if (hours >= 21)
            {
                hours = 21;
                timeText.text = string.Format("{0:00}:{1:00}", 21, 00);
                timeCanpass = false;
            }
        }



        //Gestión NPCs
        if (npcAtShop < 2 && firstNPC && index < npcs.Count)
        {
            StartCoroutine (NewNPC());
        }
        if(npcAtShop < 1 && !npcSent) //ahora mismo va mal
        {
            if (npcs[npcs.Count - 1].served || !npcs[npcs.Count-1].canBeServed)
            {
                StartCoroutine(ResendNPC());
            }
        }
    }
    public void NewDay()
    {
        day++;
        hours = 10;
        minutes = 0;
        seconds = 0;
        timeCanpass = true; //pero cuando acaben todas las animaciones y eso
        Debug.Log("New day started");
    }
    IEnumerator NewNPC()
    {
        firstNPC = false;
        //GameObject npc = Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)]); //o si eso por orden para que no haya dos iguales; nada más se instancia, necesita ir a la tienda
        npcs[index].gameObject.SetActive(true);
        index++;
        //npcAtShop++;
        yield return new WaitForSeconds(timeBetweenCustomers);
        firstNPC = true;
    }
    IEnumerator ResendNPC()
    {
        int randomNPC;
        firstNPC = false;
        randomNPC = Random.Range(0, npcs.Count);
        npcs[randomNPC].served = false;
        npcs[randomNPC].arrived = false;
        Debug.Log("El NPC ya ha salido de nuevo, su numero es " + randomNPC);
        npcSent = true;
        yield return new WaitForSeconds(20);
        npcSent = false;
        yield break;
    }
}
