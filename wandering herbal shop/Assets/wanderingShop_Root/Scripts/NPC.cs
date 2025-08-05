using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool hasMug;
    [SerializeField] GameObject mug = null;
    [SerializeField] bool isEvil;
    //[SerializeField] int[] favInfusions;
    [SerializeField] int patience;

    [SerializeField] bool inShop;
    [SerializeField] DrinkPrepared drinkScript;

    [SerializeField] bool served;
    [SerializeField] GameObject dialogueBubble;
    [SerializeField] SpriteRenderer wantedHerb;
    [SerializeField] int herbNumber;

    [SerializeField] bool stay;

    [SerializeField] bool onTheWay;
    [SerializeField] bool arrived;
    [SerializeField] Transform startPos;
    [SerializeField] Transform shopPos;


    [SerializeField] Animator animator;
    //hacer en general arrays de lugares donde ir
    // ? [SerializeField] bool isSpecial;
    private void Update()
    {
        if(!stay)
        {
            if(!arrived && !served)
            {
                float distance = Vector3.Distance(transform.position, shopPos.position);
                if (distance < 0.5)
                {
                    animator.SetBool("isWalking", false);
                    arrived = true;
                }
            }
            else if (served && !arrived)
            {
                float distance = Vector3.Distance(transform.position, startPos.position); //arreglar todo esto para que no lo checkee constantly
                if (distance < 0.5)
                {
                    animator.SetBool("isWalking", false);
                }
            }
            
        }   
        if(inShop&&arrived)
        {
            if(drinkScript.isFinished && drinkScript.isFinished)
            {
                if(herbNumber == drinkScript.ingredient)
                {
                    served = true;
                    drinkScript.mugInTable = false;
                    GameManager.Instance.money++; //suma muchos, como lo controlo? por ahora lo hago en coroutina
                    if(GameManager.Instance.infoUpdatesText != null)
                    {
                        GameManager.Instance.infoUpdatesText.text = "+1$";
                        StartCoroutine(GameManager.Instance.FadeWait());
                    }
                    arrived = false;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (!stay)
        {
            if (!served && !arrived)
            {
                Movement(shopPos.position);
            }
            else if(served && !arrived) Movement(startPos.position);
        }
    }

    void OrderTea()
    {
        dialogueBubble.SetActive(true);
        herbNumber = Random.Range(0, GameManager.Instance.herbs.Length);
        wantedHerb.sprite = GameManager.Instance.herbs[herbNumber];
        Debug.Log($"La hierba {herbNumber} de nombre {GameManager.Instance.herbs[herbNumber]} ha sido elegida.");
    }

    void Movement(Vector3 targetPos)
    {
        rb.position = Vector3.MoveTowards(rb.position, targetPos, moveSpeed*Time.deltaTime);
        if(animator.GetBool("isWalking") == false) animator.SetBool("isWalking", true);
        //look towards targetPos
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("TeaShop"))
        {
            OrderTea();
            drinkScript = other.gameObject.GetComponent<DrinkPrepared>();
            inShop = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TeaShop"))
        {
            dialogueBubble.SetActive(false);
            inShop = false;
        }
    }

    //puedo diseñar un navMesh a través de raycasts, y no por donde sea más eficiente


    //rutina:
    // llega a la tienda
    // di lo que quieres
    // deja taza si tienes
    // empieza paciencia

    //cuando se lo des (en una taza) te paga: cantidad adecuada? depende de si se equivoca (pequeño porcentaje) o si esEvil, que cuando lo confrontes huirá
}
