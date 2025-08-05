using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
    public int money;
    public TMP_Text infoUpdatesText;
    [SerializeField] TMP_Text moneyText;
    public Sprite[] herbs = new Sprite[10];
    public Camera[] cameras;
    [SerializeField] Dictionary<string, int> payments = new Dictionary<string, int>(); // pagar el carrito, la tasa de circulación, otras tasas, alquiler, comida, agua para negocio, tazas
                                                                                       // TAMBIEN BENEFICIOS! como tu salario, propinas, dinero encontrado por la calle
    //hacer que todos estos impuestos puedan estar visibles en un menu que salga al final del dia de manera ordenada
    //que de igual si hay 3 que 8, si hay 8 que una barra como slider esté disponible para que recorras todos bajando el slider
    // tmb añadir que te haga operación del dinero total? y gastos en color distinto a ganancias?? quien sabe,...
    
    [SerializeField] float fadeDuration = 1.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    // SOLO PUEDE HABER UNA CAJA DE MUGS EN EL JUEGO, ASI QUE CUANDO LLEGUE A LA NOCHE, SI NO LA HAS GASTADO TE LA ROBARAN O LE PASARÁN COSAS, NADA MÁS LA COMPRES SE AGOTARÁ EN LA TIENDA 


    // Update is called once per frame
    void Update()
    {
        if(moneyText.text != "$" + money.ToString())
        {
            moneyText.text = "$" + money.ToString();
        }
    }
    public IEnumerator FadeWait()
    {
        infoUpdatesText.gameObject.SetActive(true);
        //money++;
        Color infoColor = infoUpdatesText.color;
        infoColor.a = 1f;
        infoUpdatesText.color = infoColor;
        yield return new WaitForSeconds(2); //WaitForSecondsRealtime?

        while (infoColor.a > 0f)
        {
            infoColor.a -= fadeDuration * Time.deltaTime; // es "menos" para FadeIn pq el panel tiene el alpha a tope 
            infoColor.a = Mathf.Clamp01(infoColor.a);
            infoUpdatesText.color = infoColor;

            yield return null;
        }
        infoUpdatesText.gameObject.SetActive(false);
    }
    
}
