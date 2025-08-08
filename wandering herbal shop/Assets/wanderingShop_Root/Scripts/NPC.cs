using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed = 400;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool hasMug;
    [SerializeField] GameObject mug = null;
    [SerializeField] bool isEvil;
    //[SerializeField] int[] favInfusions;
    [SerializeField] int patience;

    [SerializeField] bool inShop;
    [SerializeField] DrinkPrepared drinkScript;

    public bool served;
    [SerializeField] GameObject dialogueBubble = null;
    [SerializeField] Image wantedHerb = null;
    [SerializeField] int herbNumber;

    [SerializeField] bool stay;
    [SerializeField] bool canArrive;
    [SerializeField] bool justChecked;
    public bool canBeServed = true;//y ahi cambio de sprite a enfadao y que tarde más el siguiente cliente

    [SerializeField] bool onTheWay;
    public bool arrived;
    [SerializeField] Transform startPos;
    [SerializeField] Transform shopPos;

    [SerializeField] Transform spawnPos;

    //esto para npcs más especiales/cuando elabore más el juego, el resto o algunos random llegan a tu tienda y después se van y desaparecen o dan vueltas más sin sentido
    [SerializeField] Transform[] routePos; //por ejemplo, 0 tienda, 1 bajo arbol, 2 parque, 3 hablar en parque
    [SerializeField] int indexRoute;
    [SerializeField] int[] scheduleHours; //por ejemplo, 12 bebida, 14 comer, 17 parque -> si son de repente las 17 y no está en el parque, va hacia el parque y se queda ahí


    [SerializeField] Animator animator;
    // ? [SerializeField] bool isSpecial;
    private void OnEnable()
    {
        //esto por ahora
        shopPos = GameObject.Find("customerPos").transform;
        startPos = GameObject.Find("NPCStartPos").transform;
        spawnPos = GameObject.Find("NPCSpawnPos").transform;
        transform.position = spawnPos.position;
    }
    private void Update()
    {//hacer enum CustomerState { GoingToShop, AtShop, Leaving, Done }???
        if (!stay && canArrive)
        {
            Vector2 ownPosNoY = new Vector2 (transform.position.x, transform.position.z);
            Vector2 shopPosNoY = new Vector2 (shopPos.position.x, shopPos.position.z);
            if(!arrived && !served)
            {
                float distance = Vector3.Distance(ownPosNoY, shopPosNoY); //ahora no va
                //Debug.Log("esta a esta distancia del carro: "+ distance);
                if (distance < 0.1)
                {
                    animator.SetBool("isWalking", false);
                    arrived = true;
                }
            }
            else if ((served && !arrived))
            {
                float distance = Vector3.Distance(transform.position, startPos.position); //arreglar todo esto para que no lo checkee constantly
                if (distance < 0.1) //a 0.35 va bien pero se queda muy cerca
                {
                    animator.SetBool("isWalking", false);
                }
            }
            if(!canBeServed)
            {
                float distance = Vector3.Distance(transform.position, startPos.position); //arreglar todo esto para que no lo checkee constantly
                if (distance < 0.1) //a 0.35 va bien pero se queda muy cerca
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
        if(!justChecked)
        {
            
            RaycastHit hit;//hacer todo esto si esta a x distancia de la tienda y va hacia ella sin estar served, si eso puedo gestionar en un script del carro quien va en que orden para evitar bugs 
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward) * 0.75f, Color.yellow); //cambiar esto para que no lo checkee cada frame aunque sea cada medio segundo
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, 0.75f)) //añadir 3 raycast/5 por si uno se queda quieto que el otro pueda rodearlo
            {
                StopAllCoroutines();
                if (hit.collider.CompareTag("NPC"))
                {
                    Debug.Log("Did hit NPC");
                    stay = true;
                    animator.SetBool("isWalking", false);
                    StartCoroutine(CheckAgain());
                }
                else
                {
                    stay = false;
                    if (animator.GetBool("isWalking")) animator.SetBool("isWalking", true);
                }
            }
            else
            {
                StopAllCoroutines();
                stay = false;
                if (animator.GetBool("isWalking")) animator.SetBool("isWalking", true);
            }

        }
        
        if (!stay)
        {
            if (!served && !arrived)
            {
                Movement(shopPos.position, false);
            }
            else if(served && !arrived) Movement(startPos.position, true);
            if(!canBeServed)
            {
                Movement(startPos.position, true);
            }
        }
    }

    IEnumerator CheckAgain()
    {
        Debug.Log("Waiting to check again, i am "+ gameObject.name);
        justChecked = true;
        yield return new WaitForSeconds(2f);
        justChecked = false;
    }
    void OrderTea()
    {
        dialogueBubble.SetActive(true);
        herbNumber = Random.Range(0, GameManager.Instance.herbs.Length);
        wantedHerb.sprite = GameManager.Instance.herbs[herbNumber];
        Debug.Log($"La hierba {herbNumber} de nombre {GameManager.Instance.herbs[herbNumber]} ha sido elegida.");
    }

    void Movement(Vector3 targetPos, bool isServed)
    {
        Vector3 targetPosWithoutY = new Vector3 (targetPos.x,rb.position.y, targetPos.z);
        Vector3 moveInput;
        rb.position = Vector3.MoveTowards(rb.position, targetPosWithoutY, moveSpeed * Time.deltaTime);
        moveInput = targetPosWithoutY - rb.position;
        Quaternion toRotation = Quaternion.LookRotation(-moveInput, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));

        if (animator.GetBool("isWalking") == false) animator.SetBool("isWalking", true);
        /*
        if(isServed)
        {
            int rotationToAdd = 0;
            RaycastHit hit;//hacer que al estar served, vaya rollo aspiradora 
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward) * 0.75f, Color.blue); //cambiar esto para que no lo checkee cada frame aunque sea cada medio segundo
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, 0.75f))
            {
                stay = true;
                animator.SetBool("isWalking", false);
                Debug.Log("Did hit something");
                transform.rotation = Quaternion.LookRotation(transform.rotation,
            }
            else
            {
                stay = false;
                if (animator.GetBool("isWalking")) animator.SetBool("isWalking", true);
            }
        }
        */

        //quitar que rote en algo mas que y
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("TeaShop"))
        {
            other.GetComponent<CameraChange>().orderTaken = false;
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("NPC")) //hacerlo con raycast para ver al que está delante
        {
            canArrive = false;
            animator.SetBool("isWalking", false);
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
