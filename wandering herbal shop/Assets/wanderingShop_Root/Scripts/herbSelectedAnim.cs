using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class herbSelectedAnim : MonoBehaviour
{
    Transform initialPos;
    SpriteRenderer herbRenderer;
    [SerializeField] Transform[] ingredientAnimSteps = new Transform[3];
    [SerializeField] float ingredientAnimSpeed;
    [SerializeField] int index;
    public bool ingredientAnimNow;
    private bool resetIndex;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = gameObject.transform;
        herbRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ingredientAnimNow)
        {
            
            if (Vector3.Distance(transform.position, ingredientAnimSteps[index].position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, ingredientAnimSteps[index].position, ingredientAnimSpeed * Time.deltaTime);
            }
            else
            {
                if (ingredientAnimSteps.Length -1 != index) index++;
                else
                {
                    herbRenderer.enabled = false;
                    transform.position = initialPos.position;
                    Debug.Log("La planta ha sido devuelta silenciosamente");
                    index = 0;
                    ingredientAnimNow = false;
                }
            }
        }
            
        
    }
}
