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

    private void Update()
    {
        if(changeTrigger)
        {
            ChangingCamera();
        }
    }
    void ChangingCamera()
    {
        if(!camChanged)
        {
            GameManager.Instance.cameras[cameraToChoose].gameObject.SetActive(true);
            GameManager.Instance.cameras[cameraBefore].gameObject.SetActive(false);
            camChanged = true;
            changeTrigger = false;
        }
        else
        {
            GameManager.Instance.cameras[cameraBefore].gameObject.SetActive(true);
            GameManager.Instance.cameras[cameraToChoose].gameObject.SetActive(false);
            camChanged = false;
            changeTrigger = false;
        }
        
    }
}
