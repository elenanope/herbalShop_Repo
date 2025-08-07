using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    [SerializeField] RenderTexture pixelRenderTexture;
    [SerializeField] bool cameraForOrdering = true;
    [SerializeField] bool npcOver = false;
    [SerializeField] float xRotInitial = 11;
    [SerializeField] float xRotSecond = 32.15f;
    [SerializeField] Vector3 rotInitial;
    [SerializeField] Vector3 rotSecond;
    public bool orderTaken;

    private void Update()
    {
        if(changeTrigger)
        {
            ChangingCamera();
        }
        
    }
    private void Start()
    {
        rotInitial = new Vector3(xRotInitial, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.y, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.z);
        rotSecond = new Vector3(xRotSecond, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.y, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.z);
        Debug.Log(rotInitial);
        Debug.Log(rotSecond);
        drinkPanel.SetActive(false);
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
                RotateToTakeOrder();
            }
            GameManager.Instance.cameras[cameraToChoose].enabled = true;
            GameManager.Instance.cameras[cameraBefore].enabled = false;
            drinkPanel.SetActive(true);
            camChanged = true;
            changeTrigger = false;
            Debug.Log("cam changed to take order!");
        }
        else
        {
            GameManager.Instance.cameras[cameraToChoose].targetTexture = null;
            GameManager.Instance.cameras[cameraBefore].targetTexture = pixelRenderTexture;
            GameManager.Instance.cameras[cameraBefore].enabled = true;
            GameManager.Instance.cameras[cameraToChoose].enabled = false;
            drinkPanel.SetActive(false);
            camChanged = false;
            changeTrigger = false;
            Debug.Log("cam changed to outside view!");
        }
        
    }
    public void FocusOnHerbs()
    {
        Debug.Log("you can begin taken the order!");
        orderTaken = true;
        //GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles = rotSecond;

        GameManager.Instance.cameras[cameraToChoose].transform.rotation = Quaternion.Slerp(Quaternion.Euler(rotInitial), Quaternion.Euler(rotSecond), transitionSpeed);
        
        orderPanel.SetActive(false) ;
    }
    void RotateToTakeOrder()
    {
        GameManager.Instance.cameras[cameraToChoose].transform.rotation = Quaternion.Slerp(Quaternion.Euler(rotSecond), Quaternion.Euler(rotInitial), transitionSpeed);
        Debug.Log("Order taken!");
        
        //GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles = new Vector3(xRotInitial, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.y, GameManager.Instance.cameras[cameraToChoose].transform.eulerAngles.z);
        if(npcOver)orderPanel.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            npcOver = true;
            Debug.Log("npcOver");
            if (camChanged)
            {
                RotateToTakeOrder();
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (camChanged && other.gameObject.CompareTag("NPC"))
        {
            npcOver = false;
        }
    }
}
