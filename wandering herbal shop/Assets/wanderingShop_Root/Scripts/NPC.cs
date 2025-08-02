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
    [SerializeField] int[] favInfusions;
    [SerializeField] int patience;

    [SerializeField] bool served;

    [SerializeField] bool onTheWay;
    [SerializeField] bool arrived;
    [SerializeField] Transform startPos;
    [SerializeField] Transform shopPos;


    [SerializeField] Animator animator;
    //hacer en general arrays de lugares donde ir
    // ? [SerializeField] bool isSpecial;
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, shopPos.position);
        if (distance < 0.8)
        {
            arrived = true; 
            animator.SetBool("isWalking", false);
        }
           
    }
    private void FixedUpdate()
    {
        if(!served)
        {
            Movement(shopPos.position);
        }
        else Movement(startPos.position);
    }

    void ChooseDrink()
    {

    }

    void Movement(Vector3 targetPos)
    {
        rb.position = Vector3.MoveTowards(rb.position, targetPos, moveSpeed*Time.deltaTime);
        if(animator.GetBool("isWalking") == false) animator.SetBool("isWalking", true);
        //look towards targetPos
    }

    //puedo diseñar un navMesh a través de raycasts, y no por donde sea más eficiente


    //rutina:
    // llega a la tienda
    // di lo que quieres
    // deja taza si tienes
    // empieza paciencia
    
    //cuando se lo des (en una taza) te paga: cantidad adecuada? depende de si se equivoca (pequeño porcentaje) o si esEvil, que cuando lo confrontes huirá
}
