using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;

public class CameraChange : MonoBehaviour
{
    public bool changeTrigger;
    [SerializeField] bool camChanged;
    [SerializeField] int cameraToChoose;
    [SerializeField] int cameraBefore;
    [SerializeField] float transitionSpeed;
    [SerializeField] GameObject transitionPanel;
    [SerializeField] GameObject drinkPanel;
    [SerializeField] GameObject orderPanel;
    [SerializeField] GameObject mapPanel;
    [SerializeField] TMP_Text replenishButton;
    [SerializeField] RenderTexture pixelRenderTexture;
    [SerializeField] bool cameraForOrdering = true;
    [SerializeField] int npcOver = 0;
    public bool rotate = false;
    //[SerializeField] float xRotInitial = 11;
    //[SerializeField] float xRotSecond = 32.15f;
    [SerializeField] Quaternion rotInitial;
    [SerializeField] Quaternion rotSecond;
    public bool orderTaken;

    private void Update()
    {
        if(changeTrigger)
        {
            ChangingCamera();
        }
        if(rotate)
        {
            RotateToTakeOrder();
        }
        else
        {
            if (orderTaken)
            {
                FocusOnHerbs();
            }
        }
    }
    private void Start()
    {
        Debug.Log(rotInitial);
        Debug.Log(rotSecond);
        drinkPanel.SetActive(false);
        mapPanel.SetActive(true);
        GameManager.Instance.cameras[cameraToChoose].targetTexture = null;
        GameManager.Instance.cameras[cameraBefore].targetTexture = pixelRenderTexture;
        GameManager.Instance.cameras[cameraBefore].enabled = true;
        GameManager.Instance.cameras[cameraToChoose].enabled = false;
    }
    void ChangingCamera()
    {
        if(!camChanged)
        {
            GameManager.Instance.cameras[cameraBefore].targetTexture = null;
            GameManager.Instance.cameras[cameraToChoose].targetTexture = pixelRenderTexture;
            if(!orderTaken)
            {
                if (npcOver != 0)
                {
                    GameManager.Instance.cameras[cameraToChoose].transform.rotation = rotInitial;
                    orderPanel.SetActive(true);
                    replenishButton.enabled = false;
                }
                else
                {
                    GameManager.Instance.cameras[cameraToChoose].transform.rotation = rotSecond;
                    replenishButton.enabled = true;
                }
                //rotate = true;//RotateToTakeOrder();
            }
            GameManager.Instance.cameras[cameraToChoose].enabled = true;
            GameManager.Instance.cameras[cameraBefore].enabled = false;
            mapPanel.SetActive(false );
            drinkPanel.SetActive(true);
            camChanged = true;
            changeTrigger = false;
            //Debug.Log("cam changed to take order!");
        }
        else
        {
            GameManager.Instance.cameras[cameraToChoose].targetTexture = null;
            GameManager.Instance.cameras[cameraBefore].targetTexture = pixelRenderTexture;
            GameManager.Instance.cameras[cameraBefore].enabled = true;
            GameManager.Instance.cameras[cameraToChoose].enabled = false;
            drinkPanel.SetActive(false);
            mapPanel.SetActive(true );
            orderPanel.SetActive(false);
            rotate = false;
            camChanged = false;
            changeTrigger = false;
            //Debug.Log("cam changed to outside view!");
        }
        
    }
    public void FocusOnHerbs()
    {
        if(rotate)rotate= false;
        //Debug.Log("you can begin taken the order!");
        if(!orderTaken)orderTaken = true;

        GameManager.Instance.cameras[cameraToChoose].transform.rotation = Quaternion.Slerp(GameManager.Instance.cameras[cameraToChoose].transform.rotation, rotSecond, transitionSpeed * Time.deltaTime);
        
        if(orderPanel.activeInHierarchy) orderPanel.SetActive(false); // esto es muy sudden
        if (GameManager.Instance.cameras[cameraToChoose].transform.rotation == rotSecond)
        {
            orderTaken = false;
            replenishButton.enabled = true;
        }
    }
    void RotateToTakeOrder()
    {
        replenishButton.enabled = false;
        GameManager.Instance.cameras[cameraToChoose].transform.rotation = Quaternion.Slerp(GameManager.Instance.cameras[cameraToChoose].transform.rotation, rotInitial, transitionSpeed * Time.deltaTime);
        //Debug.Log("Order taken!");
        if (npcOver != 0 && GameManager.Instance.cameras[cameraToChoose].transform.rotation == rotInitial)
        {
            if(camChanged)
            {
                orderPanel.SetActive(true);
                rotate = false;
            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            npcOver ++;
            //Debug.Log("npcOver");
            if (camChanged)
            {
                rotate = true;//RotateToTakeOrder();
            }
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (camChanged && other.gameObject.CompareTag("NPC"))
        {
            npcOver--;
        }
    }
}
