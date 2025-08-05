using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public bool changeTrigger;
     bool camChanged;
    [SerializeField] int cameraToChoose;
    [SerializeField] int cameraBefore;
    [SerializeField] float transitionSpeed;
    [SerializeField] GameObject transitionPanel;
    [SerializeField] GameObject drinkPanel;
    [SerializeField] RenderTexture pixelRenderTexture;

    private void Update()
    {
        if(changeTrigger)
        {
            ChangingCamera();
        }
    }
    private void Start()
    {
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
            GameManager.Instance.cameras[cameraToChoose].enabled = true;
            GameManager.Instance.cameras[cameraBefore].enabled = false;
            drinkPanel.SetActive(true);
            camChanged = true;
            changeTrigger = false;
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
        }
        
    }
}
