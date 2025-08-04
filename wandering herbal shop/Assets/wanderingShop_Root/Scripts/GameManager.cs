using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return instance;
        }
    }

    public string playerName;

    public Sprite[] herbs = new Sprite[10];
    public Camera[] cameras;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    // SOLO PUEDE HABER UNA CAJA DE MUGS EN EL JUEGO, ASI QUE CUANDO LLEGUE A LA NOCHE, SI NO LA HAS GASTADO TE LA ROBARAN O LE PASARÁN COSAS, NADA MÁS LA COMPRES SE AGOTARÁ EN LA TIENDA 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
