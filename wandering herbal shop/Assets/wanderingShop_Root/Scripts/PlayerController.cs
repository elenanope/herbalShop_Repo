using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    [SerializeField] float moveSpeed;
    [SerializeField] float slowSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float jumpForce;

    Vector3 movDirection;
    Rigidbody rb;
    PlayerInput input;
    Animator anim;
    [SerializeField] MeshRenderer[] renderers;

    [SerializeField] bool readyToTalk;
    public bool talkOrder;

    [SerializeField] GameObject heldObject;
    [SerializeField] Transform heldPlace;
    [SerializeField] bool readyToPick;
    [SerializeField] bool carryingSth;
    public bool grabSth;

    public bool canMove = true;
    [SerializeField] int camChangeStep;
    CameraChange camChange;

    [Header("GroundCheck Parameters")]
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] GameObject panelPausa;

    [Header("Step Climb Parameters")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        if (panelPausa != null) panelPausa.SetActive(false);
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (InputManager.instance.MenuToggle)
        {
            if (canMove)
            {
                input.SwitchCurrentActionMap("UI");
                canMove = false;
                panelPausa.SetActive(true);
                Debug.Log("Menu abierto");
            }
            else
            {
                input.SwitchCurrentActionMap("Gameplay");
                panelPausa.SetActive(false);
                canMove = true;
                Debug.Log("Menu cerrado");
            }
        }


        //poner que no puedas saltar si estás en alguna animacion, o hablando con lo del !canMove, el interact y speak ya veremos,
        //pero habrá que cambiar a mapas de UI y a lo mejor algún switch
        //quiza lo de a continuacion
        if (InputManager.instance.JumpJustPressed && isGrounded)
        {
            Debug.Log("Salto");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        if (InputManager.instance.InteractInput)
        {
            Debug.Log("Interactuando");
            if (!carryingSth)
            {
                if(camChangeStep == 1)
                {
                    
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].enabled = false;
                        Debug.Log("Disabling: " + renderers[i].gameObject.name);

                    }
                    canMove = false;
                    camChange.changeTrigger = true;
                    camChangeStep = 2;
                }
                else if(camChangeStep == 2)
                {
                    
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].enabled = true;
                    }
                    camChange.changeTrigger = true;
                    canMove = true;
                    camChangeStep = 1;
                }
                if (canMove && heldObject != null)
                {
                    Debug.Log("You are interacting with a " + heldObject);
                    Animator objectAnimator = heldObject.GetComponent<Animator>();
                    if (objectAnimator.parameters.Any(p => p.name == "openOrClose") && objectAnimator != null)
                    {
                        //objectAnimator.SetBool("isOpen", !objectAnimator.GetBool("isOpen"));
                        objectAnimator.SetBool("isOpen", !objectAnimator.GetBool("isOpen"));
                        if (objectAnimator.GetBool("isOpen")) objectAnimator.SetTrigger("openOrClose");
                    }
                }
            }
        }

        if (InputManager.instance.MoveObject)
        {
            if (carryingSth)
            {
                Debug.Log("You left a " + heldObject);
                heldObject.transform.parent = null;
                heldObject.transform.position = new Vector3(heldObject.transform.position.x, 0, heldObject.transform.position.z);
                carryingSth = false;
                heldObject = null;
                anim.SetBool("isHolding", false);
            }
            else
            {
                if (readyToPick && !carryingSth)
                {
                    Debug.Log("You are holding a " + heldObject);
                    heldObject.transform.parent = transform;
                    anim.SetTrigger("groundPick");
                    anim.SetBool("isHolding", true);
                    heldObject.transform.position = heldPlace.position;
                    //anim recoger y subir objeto
                    carryingSth = true;
                }
            }
        }
    }

    void GroundCheck()
    {
        if (Physics.OverlapSphere(groundCheck.transform.position, groundCheckRadius, groundLayer).Length > 0 && rb.useGravity) isGrounded = true;
        else isGrounded = false;
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            if (isGrounded) Movement();
            StepClimb();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            readyToTalk = true;
        }
        if (other.CompareTag("House"))
        {
            readyToTalk = true;
        }
        if (other.CompareTag("Pickable"))
        {
            readyToPick = true;
            if (heldObject==null) heldObject = other.gameObject;
        }
        if (other.CompareTag("TeaShop"))
        {
            if(other.GetComponent<CameraChange>() != null)
            {
                camChange = other.GetComponent<CameraChange>();
                camChangeStep = 1;
                Debug.Log("camera change");
            }
        }

        //cuando se acerque al carro y le de a interactuar, la camara que le sigue (guardandose sus transform) cambia a la posicion de prepping
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            readyToTalk = false;
        }
        if (other.CompareTag("House"))
        {
            readyToTalk = false;
        }
        if (other.CompareTag("Pickable"))
        {
            readyToPick = false;
            if (!carryingSth) heldObject = null;
        }
        if (other.CompareTag("TeaShop"))
        {
            camChange = null;
            camChangeStep = 0;
        }
    }
    void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
                anim.SetBool("isWalking", true);
            }
            else anim.SetBool("isWalking", false);
        }
    }
    void Movement()
    {
        Vector3 moveInput = new Vector3(InputManager.instance.MoveInput.x, 0, InputManager.instance.MoveInput.y);

        if (moveInput.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);
            moveInput.Normalize();
            if(!carryingSth) rb.velocity = moveInput * moveSpeed + Vector3.up * rb.velocity.y;
            else
            {
                rb.velocity = moveInput * (slowSpeed) + Vector3.up * rb.velocity.y;
            }
            Quaternion toRotation = Quaternion.LookRotation(-moveInput, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, turnSpeed * Time.fixedDeltaTime));
        }
        else
        {
            anim.SetBool("isWalking", false);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.MoveRotation(rb.rotation);
        }
    }

}
