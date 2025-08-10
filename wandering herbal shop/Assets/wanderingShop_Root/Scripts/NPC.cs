using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class NPC : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed = 400;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool hasMug;
    //[SerializeField] GameObject mug = null;
    [SerializeField] bool isEvil;
    //[SerializeField] int[] favInfusions;
    [SerializeField] int patience;

    [SerializeField] bool inShop;
    [SerializeField] DrinkPrepared drinkScript;

    public bool served;
    [SerializeField] Image wantedHerb = null;
    [SerializeField] int herbNumber;

    [SerializeField] bool followingPath = true;
    [SerializeField] bool stay;
    [SerializeField] bool canArrive;
    [SerializeField] bool justChecked;
    public bool canBeServed = true;//y ahi cambio de sprite a enfadao y que tarde más el siguiente cliente

    [SerializeField] bool onTheWay;
    public bool arrived;
    [SerializeField] Vector3 startPos;
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
                float distance = Vector3.Distance(transform.position, startPos); //arreglar todo esto para que no lo checkee constantly
                if (distance < 0.1) //a 0.35 va bien pero se queda muy cerca
                {
                    animator.SetBool("isWalking", false);
                }
            }
            if(!canBeServed)
            {
                float distance = Vector3.Distance(transform.position, startPos); //arreglar todo esto para que no lo checkee constantly
                if (distance < 0.1) //a 0.35 va bien pero se queda muy cerca
                {
                    animator.SetBool("isWalking", false);
                }
            }
            
        }   
        if(inShop&&arrived)
        {
            if(drinkScript.isFinished)
            {
                if(herbNumber == drinkScript.ingredient)
                {
                    served = true;
                    drinkScript.orderFinished = true;
                    //drinkScript.mugInTable = false;
                    GameManager.Instance.money++;
                    if(GameManager.Instance.infoUpdatesText != null)
                    {
                        GameManager.Instance.infoUpdatesText.text = "+1$";
                        StopCoroutine(GameManager.Instance.FadeWait()); //necesario??
                        StartCoroutine(GameManager.Instance.FadeWait());
                    }
                    arrived = false;
                }
                else drinkScript.isFinished = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if(!justChecked)
        {
            //FALTA AÑADIR LA ROTACIÓN SEGÚN CADA DIRECCIÓN Y HACER QUE SIGA LA DIRECCIÓN INDICADA, ADEMÁS QUE ESTO NO LO HAGA CUANDO ESTÉ A LA COLA
            //AHÍ QUE HAGA EL COMPORTAMIENTO MÁS PRIMITIVO QUE TENÍAMOS ANTES
            //TAMBIEN FALTA AÑADIR AQUÍ QUE LO CHECKEE CADA MEDIO SEGUNDO QUIZÁS, SI NO ESTÁ QUIETO EN ALGÚN SITIO, SI LO ESTÁ, QUE NO HAGA NADA
            RaycastHit[] hit = new RaycastHit[14];
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward) * 0.5f, Color.yellow); //1
            /*
            Debug.DrawRay(transform.position, transform.TransformDirection(-8*Vector3.forward+ Vector3.right) * 0.07f, Color.white); //2
            Debug.DrawRay(transform.position, transform.TransformDirection(-8*Vector3.forward - Vector3.right) * 0.07f, Color.white); //3
            
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.3f, Color.blue); //4
            Vector3 leftRaysStart = transform.position + transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.3f;
            Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward) * 0.5f, Color.green);// 6
            Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.3f, Color.green);// 7
            Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward + Vector3.right + 4*Vector3.right) * 0.1f, Color.green);// 8
            //12
            //13

            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.3f, Color.red); //5
            Vector3 rightRaysStart = transform.position + transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.3f;
            Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward) * 0.5f, Color.magenta); // 9
            Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.3f, Color.magenta); // 10
            Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right - 4*Vector3.right) * 0.1f, Color.magenta); // 11
            //14
            //15*/
            
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit[0], 0.5f)) //añadir 3 raycast/5 por si uno se queda quieto que el otro pueda rodearlo
            {
                Debug.Log("Paso por 1 ocupado, procede a siguiente fase");
                bool hit1 = Physics.Raycast(transform.position, (-8 * Vector3.forward + Vector3.right), out hit[1], 0.7f);
                bool hit2 = Physics.Raycast(transform.position, (-8 * Vector3.forward - Vector3.right), out hit[2], 0.7f);
                int pathToTake = 0;

                Debug.DrawRay(transform.position, transform.TransformDirection(-8 * Vector3.forward + Vector3.right) * 0.7f, Color.white); //2
                Debug.DrawRay(transform.position, transform.TransformDirection(-8 * Vector3.forward - Vector3.right) * 0.7f, Color.white); //3

                if (hit1 && hit2)
                {
                    Debug.Log("Paso por 2 y 3 ocupado, procede a siguiente fase");
                    Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.7f, Color.blue); //4
                    Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.7f, Color.red); //5
                    Vector3 leftRaysStart = transform.position + transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.7f;
                    bool hit3 = Physics.Raycast(transform.position, (-Vector3.forward + Vector3.right), out hit[3], 0.7f); //4
                    Vector3 rightRaysStart = transform.position + transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.7f;
                    bool hit4 = Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward - Vector3.right), out hit[4], 0.7f); //5

                    if (hit3 && hit4)
                    {
                        Debug.Log("Paso por 4 y 5 ocupado, procede a siguiente fase");
                    }
                    else if (!hit3 && hit4)
                    {
                        Debug.Log("Paso por 5 libre, por 4 no");
                        pathToTake = 5;
                    }
                    else if (hit3 && !hit4)
                    {
                        Debug.Log("Paso por 4 libre, 5 no");
                        pathToTake = 4;
                    }
                    else
                    {
                        int hitsA=0;
                        int hitsB=0;
                        Debug.Log("Paso por 4 y 5 libre, vamos a especificar");
                        bool hit5 = Physics.Raycast(leftRaysStart, (-Vector3.forward), out hit[5], 0.5f);// 6
                        bool hit6 = Physics.Raycast(leftRaysStart, (-Vector3.forward + Vector3.right), out hit[6], 0.5f);// 7
                        bool hit7 = Physics.Raycast(leftRaysStart, (-Vector3.forward + Vector3.right + 4 * Vector3.right), out hit[7], 0.5f);// 8
                        bool hit8 = Physics.Raycast(rightRaysStart, transform.TransformDirection(-Vector3.forward), out hit[8], 0.5f); // 9
                        bool hit9 = Physics.Raycast(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right), out hit[9], 0.5f); // 10
                        bool hit10 = Physics.Raycast(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right - 4 * Vector3.right), out hit[10], 0.5f); // 11

                        Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward) * 0.5f, Color.green);// 6
                        Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward + Vector3.right) * 0.3f, Color.green);// 7
                        Debug.DrawRay(leftRaysStart, transform.TransformDirection(-Vector3.forward + Vector3.right + 4 * Vector3.right) * 0.1f, Color.green);// 8
                        Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward) * 0.5f, Color.magenta); // 9
                        Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right) * 0.3f, Color.magenta); // 10
                        Debug.DrawRay(rightRaysStart, transform.TransformDirection(-Vector3.forward - Vector3.right - 4 * Vector3.right) * 0.1f, Color.magenta); // 11

                        if (hit5)
                        {
                            hitsA = 1;
                            if(hit6)
                            {
                                hitsA = 2;
                                if (hit7)hitsA = 3;
                            }
                        }
                        else
                        {
                            if (hit6)
                            {
                                hitsA = 1;
                                if (hit7)hitsA = 2;
                            }
                            else
                            {
                                if (hit7)hitsA = 1;
                            }
                        }
                        if (hit8)
                        {
                            hitsB = 1;
                            if (hit9)
                            {
                                hitsB = 2;
                                if (hit10) hitsB = 3;
                            }
                        }
                        else
                        {
                            if (hit9)
                            {
                                hitsB = 1;
                                if (hit10) hitsB = 2;
                            }
                            else
                            {
                                if (hit10) hitsB = 1;
                            }
                        }
                        if (hitsA > hitsB)
                        {
                            Debug.Log("Se elige ir por el A");
                            if (hit7) pathToTake = 8;
                            else
                            {
                                if(hit6) pathToTake = 7;
                                else
                                {
                                    if (hit5) pathToTake = 6;
                                }
                            }
                        }
                        else if (hitsA < hitsB)
                        {
                            Debug.Log("Se elige ir por el B");
                            if (hit10) pathToTake = 11;
                            else
                            {
                                if (hit9) pathToTake = 10;
                                else
                                {
                                    if (hit8) pathToTake = 9;
                                }
                            }
                        }
                        else if (hitsA == hitsB && hitsB < 3)
                        {
                            Debug.Log("Se pueden coger ambos");
                            pathToTake = Random.Range(4, 5);//??
                        }
                        else Debug.Log("No se puede coger ninguno, da media vuelta mejor"); pathToTake = -1;
                    }
                }
                else if (!hit1 && hit2)
                {
                    Debug.Log("Paso por 3 libre, por 2 no");
                    pathToTake = 3;
                }
                else if (hit1 && !hit2)
                {
                    Debug.Log("Paso por 2 libre, 3 no");
                    pathToTake = 2;
                }
                else
                {
                    Debug.Log("Paso por 2 y 3 libre");
                    pathToTake = Random.Range(2, 3);
                }
                Debug.Log("El camino a tomar es el número " + pathToTake);
            }
            else
            {
                Debug.Log("Puedes seguir recto");
            }
            
            /*
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, 0.5f)) //añadir 3 raycast/5 por si uno se queda quieto que el otro pueda rodearlo
            {
                if(!hit.collider.CompareTag("TeaShop"))
                {
                    Debug.Log("En el centro ha dado!");
                    StopCoroutine(CheckAgain());
                    if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward + Vector3.right), out hit, 0.3f))
                    {
                        followingPath = false;
                        Debug.Log("se puede salir por lado izq");
                        Vector3 targetPosition = new Vector3(2, rb.position.y, -2);
                        Quaternion toRotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y - 45f, 0f);

                        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));

                        rb.position = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        Debug.Log("En la izq ha dado!");
                        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward - Vector3.right), out hit, 0.3f))
                        {
                            followingPath = false;
                            Debug.Log("se puede salir por derecha");
                            Vector3 targetPosition = new Vector3(-2, rb.position.y, -2);
                            Quaternion toRotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + 45f, 0f);

                            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));

                            rb.position = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);

                        }
                        else Debug.Log("es imposible salir, en derecha tmb ha dado");
                    }
                    StartCoroutine(CheckAgain());
                }
               */

            /*
                if (hit.collider.CompareTag("NPC"))
            {
                Debug.Log("Did hit NPC");
                stay = true;
                animator.SetBool("isWalking", false);
                StartCoroutine(CheckAgain());
            }
            else
            {
                followingPath = false;
                if (animator.GetBool("isWalking")) animator.SetBool("isWalking", true);
            }
        }
        else
        {
            followingPath = true;
            StopCoroutine(CheckAgain());
            stay = false;
            if (animator.GetBool("isWalking")) animator.SetBool("isWalking", true);
        }
            */
            justChecked = true;
        }
        
        if (!stay)
        {
            if (!served && !arrived)
            {
                Movement(shopPos.position, false);
            }
            else if(served && !arrived) Movement(startPos, true);
            if(!canBeServed)
            {
                Movement(startPos, true);
            }
        }
    }
    IEnumerator RayManager()
    {

        yield break;
    }
    IEnumerator CheckAgain()
    {
        //Debug.Log("Waiting to check again, i am "+ gameObject.name);
        justChecked = true;
        yield return new WaitForSeconds(2f);
        justChecked = false;
        yield break;
    }
    void OrderTea()
    {
        //dialogueBubble.SetActive(true);
        herbNumber = Random.Range(0, GameManager.Instance.herbs.Length);
        wantedHerb.sprite = GameManager.Instance.herbs[herbNumber];
        //Debug.Log($"La hierba {herbNumber} de nombre {GameManager.Instance.herbs[herbNumber]} ha sido elegida.");
    }

    void Movement(Vector3 targetPos, bool isServed)
    {
        Vector3 targetPosWithoutY = new Vector3 (targetPos.x,rb.position.y, targetPos.z);
        Vector3 moveInput;
        if(followingPath)
        {
            rb.position = Vector3.MoveTowards(rb.position, targetPosWithoutY, moveSpeed * Time.deltaTime);
            moveInput = targetPosWithoutY - rb.position;
            Quaternion toRotation = Quaternion.LookRotation(-moveInput, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));
        }
        /*
        else // si eso cambiar todo esto por 3 raycast constantes arriba y que vaya por alguno que no colisione con nada
        {
            Quaternion toRotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + 45f, 0f);

            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));
            RaycastHit hit2;
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit2, 0.75f))
            {
                followingPath = true;
            }
        }
        */
        

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
            //dialogueBubble.SetActive(false);
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
